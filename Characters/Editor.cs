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

        //RAGE.Input.Unbind(leftbindid);
        //nem jó mert csak 1x hívja meg a függvényt
        RAGE.Ui.HtmlWindow EditorCEF;
        float camHeightOffset = 0f;
        Vector3 camStartPositions;
        float camZoom = 60f;
        public Editor()
        {
            Events.Add("client:CharEdit",CharacterEditor);
            //Events.Add("client:camtest", SetCamera);

            Events.Add("client:AttributeToServer", AttributeToServer);
            Events.Add("client:FinishEditing", FinishEditing);
        }

        private void FinishEditing(object[] args)
        {
            Events.CallRemote("server:FinishEditing");
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
            float nx = pos.X + (1.7f * (float)Math.Sin(radians));
            float ny = pos.Y + (1.7f * (float)Math.Cos(radians));

            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", nx, ny, pos.Z+0.3f, 0f, 0f, 0f, 60f, true, 2);
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, true, 500, true, false, 0);
            camStartPositions = new Vector3(nx, ny, pos.Z + 0.3f);
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
                camHeightOffset = 0f;
                camZoom = 60f;

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
                if (camHeightOffset < 1f)
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
                if (camZoom > 15f)
                {
                    camZoom -= 0.3f;
                }

            }
            else if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.Q))
            {
                if (camZoom < 70f)
                {
                    camZoom += 0.3f;
                }
            }
            SetCamPosition();
        }

        private void SetCamPosition()
        {
            Vector3 pos = Player.LocalPlayer.Position;
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z + camHeightOffset);


            Vector3 campos = RAGE.Game.Cam.GetCamCoord(camera);
            RAGE.Game.Cam.SetCamCoord(camera, camStartPositions.X, camStartPositions.Y, camStartPositions.Z + camHeightOffset);

            RAGE.Game.Cam.SetCamFov(camera, camZoom);
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
