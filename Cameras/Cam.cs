using RAGE;
using RAGE.NUI.PauseMenu;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RAGE.NUI;

namespace Client.Cameras
{
    internal class Cam : Events.Script
    {
        int camera = -1;

        float camHeightOffset = 0f;
        Vector3 camStartPositions;
        float camZoom = 65f;
        float revs = -1f;
        public Cam() {
            Events.Add("client:SkyCam", SkyCam);
            Events.Add("client:SetCamera", SetCamera);
            Events.Add("client:EditorCamera", EditorCamera);
            Events.Add("client:DeleteCamera", DeleteCamera);

            Events.Add("client:InfrontCamera", InfrontCamera);

            Events.Add("client:DealershipCamera", DealershipCamera);
            Events.Add("client:rev", RevVehicle);
            Events.Add("client:SetDOF", SetDOF);
        }

        private void SetDOF(object[] args)
        {
            RAGE.Game.Cam.SetCamUseShallowDofMode(camera, Convert.ToBoolean(args[0]));
            RAGE.Game.Cam.SetCamNearDof(camera, Convert.ToSingle(args[1]));
            RAGE.Game.Cam.SetCamFarDof(camera, Convert.ToSingle(args[2]));
            RAGE.Game.Cam.SetCamDofStrength(camera, 1); 
        }

        private void RevVehicle(object[] args)
        {
            revs = Convert.ToSingle(args[0]);
            if (revs != -1f)
            {
                Events.Tick += Rev;
            }
            else
            {
                Events.Tick -= Rev;
            }
            
        }

        private void Rev(List<Events.TickNametagData> nametags)
        {
            if (revs != -1f)
            {
                Player.LocalPlayer.Vehicle.Rpm = revs;
            }
        }

        private void DealershipCamera(object[] args)
        {
            Vector3 pos = Player.LocalPlayer.Vehicle.Position;
            camHeightOffset = 0f;
            camZoom = 65f;

            float radians = -Player.LocalPlayer.GetHeading() * (float)Math.PI / 180f;
            float nx = pos.X + (7f * (float)Math.Sin(radians));
            float ny = pos.Y + (7f * (float)Math.Cos(radians));

            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", nx, ny, pos.Z + 0.8f, 0f, 0f, 0f, 60f, true, 2);
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, true, 500, true, false, 0);
            camStartPositions = new Vector3(nx, ny, pos.Z + 0.8f);
            Events.Tick += TickDealership;
        }

        private void SkyCam(object[] args)
        {
            
            RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
            bool state = Convert.ToBoolean(args[0]);
            if (state)
                {
                    RAGE.Game.Invoker.Invoke(0xAAB3200ED59016BC, p.Handle, 0, 1);
                }
                else
                {
                    RAGE.Game.Invoker.Invoke(0xD8295AF639FD9CB8, p.Handle, 0, 1);
                }
        }

        public static bool CheckSkyCam()
        {
            RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
            if (RAGE.Game.Invoker.Invoke<bool>(0xD9D2CFFF49FAB35F, p.Handle, 0, 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public void SetCamera(object[] args)
        {
            float posX = Convert.ToSingle(args[0]);
            float posY = Convert.ToSingle(args[1]);
            float posZ = Convert.ToSingle(args[2]);
            float rotX = Convert.ToSingle(args[3]);
            float rotY = Convert.ToSingle(args[4]);
            float rotZ = Convert.ToSingle(args[5]);
            float fov = Convert.ToSingle(args[6]);
            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", posX, posY, posZ, rotX, rotY, rotZ, fov, true, 2);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
        }

        public void EditorCamera(object[] args)
        {
            Vector3 pos = Player.LocalPlayer.Position;
            camHeightOffset = 0f;
            camZoom = 65f;

            float radians = -Player.LocalPlayer.GetHeading() * (float)Math.PI / 180f;
            float nx = pos.X + (1.7f * (float)Math.Sin(radians));
            float ny = pos.Y + (1.7f * (float)Math.Cos(radians));

            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", nx, ny, pos.Z + 0.3f, 0f, 0f, 0f, 60f, true, 2);
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, true, 500, true, false, 0);

            camStartPositions = new Vector3(nx, ny, pos.Z + 0.3f);

            RAGE.Game.Cam.SetCamUseShallowDofMode(camera, true);
            RAGE.Game.Cam.SetCamNearDof(camera, 0.3f);
            RAGE.Game.Cam.SetCamFarDof(camera, 1.7f);
            RAGE.Game.Cam.SetCamDofStrength(camera, 1);
            RAGE.Elements.Player.LocalPlayer.TaskLookAtCoord(nx, ny, pos.Z + 0.3f, -1, 0, 2);
            Events.Tick += Tick;
        }

        public void InfrontCamera(object[] args)
        {
            Vector3 pos = Player.LocalPlayer.Position;
            camHeightOffset = 0f;
            camZoom = 80f;
 
            float radians = -Player.LocalPlayer.GetHeading() * (float)Math.PI / 180f;
            float nx = pos.X + (2.7f * (float)Math.Sin(radians));
            float ny = pos.Y + (2.7f * (float)Math.Cos(radians));

            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", nx, ny, pos.Z + 0.1f, 0f, 0f, 0f, 50f, true, 2);
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, true, 500, true, false, 0);
            camStartPositions = new Vector3(nx, ny, pos.Z + 0.3f);

            RAGE.Game.Cam.SetCamUseShallowDofMode(camera, true);
            RAGE.Game.Cam.SetCamNearDof(camera, 0.5f);
            RAGE.Game.Cam.SetCamFarDof(camera, 2.8f);
            RAGE.Game.Cam.SetCamDofStrength(camera, 1f);

            
            Events.Tick += SetDOF;
        }

        private void SetDOF(List<Events.TickNametagData> nametags)
        {
            RAGE.Game.Cam.SetUseHiDof();
        }




        public void DeleteCamera(object[] args)
        {
            RAGE.Game.Cam.SetCamActive(camera, false);
            RAGE.Game.Cam.DestroyCam(camera, true);
            RAGE.Game.Cam.RenderScriptCams(false, true, 500, true, false, 0);
            Events.Tick -= Tick;
            Events.Tick -= SetDOF;
            RAGE.Elements.Player.LocalPlayer.ClearTasks();
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



        private void Tick(List<Events.TickNametagData> nametags)
        {
            RAGE.Game.Cam.SetUseHiDof();
            if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.D))
            {
                RotateCharRight();
            }
            else if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.A))
            {
                RotateCharLeft();
            }

            if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.W))
            {
                if (camHeightOffset < 0.8f)
                {
                    camHeightOffset += 0.005f;
                }
            }

            else if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.S))
            {
                if (camHeightOffset > -1f)
                {
                    camHeightOffset -= 0.005f;
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
            
            SetCamPosition();
        }

        /*private void RotateCharRight()
        {
            Vector3 rot = Player.LocalPlayer.GetRotation(0);
            Player.LocalPlayer.SetRotation(rot.X, rot.Y, rot.Z + 1f, 0, true);
        }
        KLIENS OLDALI NEM JÓ, MOZOG A KARAKTER ÖSSZE-VISSZA
        private void RotateCharLeft()
        {
            Vector3 rot = Player.LocalPlayer.GetRotation(0);
            Player.LocalPlayer.SetRotation(rot.X, rot.Y, rot.Z - 1f, 0, true);
        }*/
        private void RotateVehicleRight()
        {
            Events.CallRemote("server:RotateVehicleRight");
        }

        private void RotateVehicleLeft()
        {

            Events.CallRemote("server:RotateVehicleLeft");
        }


        private void RotateCharRight()
        {
            Events.CallRemote("server:RotateCharRight");
        }

        private void RotateCharLeft()
        {

            Events.CallRemote("server:RotateCharLeft");
        }

        private void SetCamToVehicle()
        {
            Vector3 pos = Player.LocalPlayer.Vehicle.Position;
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z + camHeightOffset);


            Vector3 campos = RAGE.Game.Cam.GetCamCoord(camera);
            RAGE.Game.Cam.SetCamCoord(camera, camStartPositions.X, camStartPositions.Y, camStartPositions.Z + camHeightOffset);

            RAGE.Game.Cam.SetCamFov(camera, camZoom);
        }

        private void SetCamPosition()
        {
            Vector3 pos = Player.LocalPlayer.Position;
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z + camHeightOffset);

            Vector3 campos = RAGE.Game.Cam.GetCamCoord(camera);
            
            RAGE.Game.Cam.SetCamCoord(camera, camStartPositions.X, camStartPositions.Y, camStartPositions.Z + camHeightOffset);

            RAGE.Game.Cam.SetCamFov(camera, camZoom);
        }
    }
}
