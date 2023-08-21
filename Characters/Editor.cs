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
            //CAM -813.95, 174.2, 76.78, 0, 0, -69
            //kamera megvan, karakter is a helyén
            //CEF-et megnyitni
        }
    }
}
