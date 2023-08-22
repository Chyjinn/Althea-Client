using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Characters
{
    class Editor : Events.Script
    {
        public Editor()
        {
            Events.Add("client:CharEdit",CharacterEditor);
        }

        public void CharacterEditor(object[] args)
        {
            Events.Tick += Tick;
            RAGE.Input.TakeScreenshot("valami.png", 1, 100, 100);
            //CAM -813.95, 174.2, 76.78, 0, 0, -69
            //kamera megvan, karakter is a helyén
            //CEF-et megnyitni
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
