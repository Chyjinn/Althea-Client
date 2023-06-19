using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Cameras
{
    internal class Cam : Events.Script
    {
        int camera = 1;
        public Cam() {
            Events.Add("client:SetCamera", SetCamera);
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
    }
}
