using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Client.Characters
{
    class Editor : Events.Script
    {
        //int leftbindid = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Left, true, RotateCharLeft);
        //int rightbindid = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Right, true, RotateCharRight);
        //RAGE.Input.Unbind(leftbindid);
        //nem jó mert csak 1x hívja meg a függvényt
        RAGE.Ui.HtmlWindow EditorCEF;
        RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
        DateTime nextUpdate = DateTime.Now;
        public Editor()
        {
            Events.Add("client:CharEdit",CharacterEditor);

            Events.Add("client:AttributeToServer", AttributeToServer);
        }

        private void AttributeToServer(object[] args)
        {
            Events.CallRemote("server:EditAttribute", Convert.ToInt32(args[0]), Convert.ToString(args[1]));//attribute id, value
        }
        int camera = -1;
        public void SetCamera()
        {
            Vector3 pos = Player.LocalPlayer.Position;

            float radians = -Player.LocalPlayer.GetHeading() * (float)Math.PI / 180f;
            float nx = pos.X + (1.2f * (float)Math.Sin(radians));
            float ny = pos.Y + (1.2f * (float)Math.Cos(radians));

            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", nx, ny, pos.Z+0.7f, 0f, 0f, 0f, 60f, true, 2);
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, true, 500, true, false, 0);
        }

        public void DeleteCamera()
        {
            RAGE.Game.Cam.DestroyCam(camera, true);
            RAGE.Game.Cam.SetCamActive(camera, false);
            RAGE.Game.Cam.RenderScriptCams(false, true, 500, true, false, 0);
        }


        public void CharacterEditor(object[] args)
        {
            bool state = (bool)args[0];

            if (state)
            {
                EditorCEF = new RAGE.Ui.HtmlWindow("package://frontend/editor/charedit.html");
                EditorCEF.Active = true;
                SetCamera();
                Events.Tick += Tick;
            }
            else
            {
                EditorCEF.Active = false;
                EditorCEF.Destroy();
                DeleteCamera();
                Events.Tick -= Tick;
            }


        }


        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.Right))
            {
                RotateCharRight();
            }
            else if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.Left))
            {
                RotateCharLeft();
            }
        }

        

    private void RotateCharRight()
        {
            Events.CallRemote("server:RotateCharRight");
        }

        private void RotateCharLeft()
        {

            Events.CallRemote("server:RotateCharLeft");
        }
    }
}
