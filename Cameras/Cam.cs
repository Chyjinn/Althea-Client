using RAGE;
using RAGE.NUI.PauseMenu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Cameras
{
    internal class Cam : Events.Script
    {
        int camera = 1;
        public Cam() {
            Events.Add("client:SkyCam", SkyCam);
            Events.Add("client:SetCamera", SetCamera);
            Events.Add("client:DeleteCamera", DeleteCamera);
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

        private bool CheckSkyCam()
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


        public void DeleteCamera(object[] args)
        {
            RAGE.Game.Cam.DestroyCam(camera, true);
            RAGE.Game.Cam.SetCamActive(camera, false);
            RAGE.Game.Cam.RenderScriptCams(false, false, 0, true, false, 0);
        }
    }
}
