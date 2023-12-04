using RAGE;
using RAGE.NUI.PauseMenu;
using System;
using System.Collections.Generic;

namespace Client.Cameras
{
    internal class Cam : Events.Script
    {
        int camera = -1;

        float camHeightOffset = 0f;
        Vector3 camStartPositions;
        float camZoom = 65f;
        DateTime idleTime = DateTime.Now;

        public Cam() {
            Events.Add("client:SkyCam", SkyCam);
            Events.Add("client:SetCamera", SetCamera);
            Events.Add("client:EditorCamera", EditorCamera);
            Events.Add("client:DeleteCamera", DeleteCamera);
            
            Events.Add("client:InfrontCamera", InfrontCamera);

            Events.Add("client:SetDOF", SetDOF);

            Events.Add("client:SetFPSFov", SetFPSFov);
            Events.Tick += CountIdle;
        }

        public void CountIdle(List<Events.TickNametagData> nametags)
        {
            TimeSpan diff = DateTime.Now - idleTime;
            if (diff.Seconds > 29)
            {
                RAGE.Game.Invoker.Invoke(0xF4F2C0D4EE209E20);
                idleTime = DateTime.Now;
            }
        }

    private void SetFPSFov(object[] args)
        {
            Vector3 cam = RAGE.Game.Cam.GetGameplayCamCoords();
            Vector3 camrot = RAGE.Game.Cam.GetGameplayCamRot(2);
            fov = Convert.ToSingle(args[0]);
            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", cam.X, cam.Y, cam.Z, camrot.X, camrot.Y, camrot.Z, fov, true, 2);
            RAGE.Game.Cam.RenderScriptCams(true, true, 500, true, false, 0);
            Events.Tick += Render;
        }

        float fov = 40f;
        private void Render(List<Events.TickNametagData> nametags)
        {

            if (RAGE.Game.Cam.GetFollowPedCamViewMode() == 4 && RAGE.Game.Cam.IsCamActive(camera))//ha first person és aktív a kamera
            {
                Vector3 cam = RAGE.Game.Cam.GetGameplayCamCoord();
                Vector3 camrot = RAGE.Game.Cam.GetGameplayCamRot(2);

                RAGE.Game.Cam.SetCamCoord(camera, cam.X, cam.Y, cam.Z+0.01f);
                RAGE.Game.Cam.SetCamRot(camera, camrot.X, camrot.Y, camrot.Z, 2);
                RAGE.Elements.Player.LocalPlayer.SetRotation(0f, 0f, camrot.Z, 2, false);
            }
            else//nem first person vagy nem aktív a kamera
            {
                if (RAGE.Game.Cam.IsCamActive(camera))//ha aktív a kamera (tehát nem first person) akkor töröljük
                {
                    RAGE.Game.Cam.SetCamActive(camera, false);
                    RAGE.Game.Cam.DestroyCam(camera, true);
                    RAGE.Game.Cam.RenderScriptCams(false, true, 500, true, false, 0);
                    Events.Tick -= Render;
                    RAGE.Elements.Player.LocalPlayer.SetComponentVariation(0, 0, 0, 0);
                }
            }
        }

        private void SetDOF(object[] args)
        {
            RAGE.Game.Cam.SetCamUseShallowDofMode(camera, Convert.ToBoolean(args[0]));
            RAGE.Game.Cam.SetCamNearDof(camera, Convert.ToSingle(args[1]));
            RAGE.Game.Cam.SetCamFarDof(camera, Convert.ToSingle(args[2]));
            RAGE.Game.Cam.SetCamDofStrength(camera, 1); 
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
            Vector3 pos = RAGE.Elements.Player.LocalPlayer.Position;
            camHeightOffset = 0f;
            camZoom = 65f;

            float radians = -RAGE.Elements.Player.LocalPlayer.GetHeading() * (float)Math.PI / 180f;
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
            Vector3 pos = RAGE.Elements.Player.LocalPlayer.Position;
            camHeightOffset = 0f;
            camZoom = 80f;
 
            float radians = -RAGE.Elements.Player.LocalPlayer.GetHeading() * (float)Math.PI / 180f;
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




        private void Tick(List<Events.TickNametagData> nametags)
        {
            RAGE.Game.Cam.SetUseHiDof();
            RAGE.Elements.Player.LocalPlayer.ClearSecondaryTask();
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

        private void RotateCharRight()
        {
            Events.CallRemote("server:RotateCharRight");
        }

        private void RotateCharLeft()
        {

            Events.CallRemote("server:RotateCharLeft");
        }


        private void SetCamPosition()
        {
            Vector3 pos = RAGE.Elements.Player.LocalPlayer.Position;
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z + camHeightOffset);

            Vector3 campos = RAGE.Game.Cam.GetCamCoord(camera);
            
            RAGE.Game.Cam.SetCamCoord(camera, camStartPositions.X, camStartPositions.Y, camStartPositions.Z + camHeightOffset);

            RAGE.Game.Cam.SetCamFov(camera, camZoom);
        }
    }
}
