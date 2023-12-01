using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Client.Vehicles
{
    internal class Shop : Events.Script
    {

        int camera = -1;
        float camHeightOffset = 0f;
        Vector3 camStartPositions;
        float camZoom = 65f;
        Vehicle v = null;
        int tempid = 0;
        public Shop() {
            Events.Add("client:OpenDealership", OpenDealership);
        }

        private void OpenDealership(object[] args)
        {
            ushort vehid = Convert.ToUInt16(args[0]);
            tempid = Convert.ToInt32(args[1]);
            v = RAGE.Elements.Entities.Vehicles.Streamed.Where((v) => v.RemoteId == vehid).FirstOrDefault();
            Chat.Output(v.Model.ToString().ToUpper());
            //megkapjuk majd egy listában az elérhető kocsikat és azokat feldolgozva jelenítjük meg sorban
            //-44.7f, -1098.1f, 26.2f, 150f
            v.FreezePosition(true);
            v.SetEngineOn(true, true, false);
            RevVehicle(true);
            DealershipCamera();
        }

        private void RevVehicle(bool state)
        {
            if (state)
            {
                Events.Tick += Rev;
            }
            else
            {
                Events.Tick -= Rev;
            }
        }

        //alapjárat 0.2f
        private void Rev(List<Events.TickNametagData> nametags)
        {
            v.Rpm = 1.1f;
        }
        float degree = 0f;
        private void RotateCam(List<Events.TickNametagData> nametags)
        {
            Vector3 pos = v.Position;
            float radians = -v.GetHeading() + degree * (float)Math.PI / 180f;
            float nx = pos.X + (7f * (float)Math.Sin(radians));
            float ny = pos.Y + (7f * (float)Math.Cos(radians));
            RAGE.Game.Cam.SetCamCoord(camera, nx, ny, pos.Z + 0.8f);
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z);

            degree += 0.1f;
            if (degree >= 360f)
            {
                degree = 0f;
            }
        }

        private void DealershipCamera()
        {
            Vector3 pos = Player.LocalPlayer.Vehicle.Position;
            camHeightOffset = 0f;
            camZoom = 65f;
            degree = 0f;
            float radians = -v.GetHeading() + degree * (float)Math.PI / 180f;
            float nx = pos.X + (7f * (float)Math.Sin(radians));
            float ny = pos.Y + (7f * (float)Math.Cos(radians));

            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", nx, ny, pos.Z + 0.8f, 0f, 0f, 0f, 60f, true, 2);
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, true, 500, true, false, 0);
            camStartPositions = new Vector3(nx, ny, pos.Z + 0.8f);
            Events.Tick += RotateCam;
        }


        private void TickDealership(List<Events.TickNametagData> nametags)
        {
            if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.D))
            {
                RotateVehicleLeft();
            }
            else if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.A))
            {
                RotateVehicleRight();
            }

            if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.W))
            {
                if (camHeightOffset < 1f)
                {
                    camHeightOffset += 0.01f;
                }

            }

            else if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.S))
            {
                if (camHeightOffset > -0.6f)
                {
                    camHeightOffset -= 0.01f;
                }
            }

            if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.E))
            {
                if (camZoom > 10f)
                {
                    camZoom -= 0.3f;
                }

            }
            else if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.Q))
            {
                if (camZoom < 90f)
                {
                    camZoom += 0.3f;
                }
            }
            SetCamToVehicle();
        }
        private void RotateVehicleRight()
        {
            Events.CallRemote("server:RotateVehicleRight");
        }

        private void RotateVehicleLeft()
        {

            Events.CallRemote("server:RotateVehicleLeft");
        }



        private void SetCamToVehicle()
        {
            Vector3 pos = Player.LocalPlayer.Vehicle.Position;
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z + camHeightOffset);


            Vector3 campos = RAGE.Game.Cam.GetCamCoord(camera);
            RAGE.Game.Cam.SetCamCoord(camera, camStartPositions.X, camStartPositions.Y, camStartPositions.Z + camHeightOffset);

            RAGE.Game.Cam.SetCamFov(camera, camZoom);
        }

    }
}
