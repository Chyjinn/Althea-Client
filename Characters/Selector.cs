using Client.Hud;
using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Client.Characters
{ 

        internal class Selector : Events.Script
    {
        RAGE.Ui.HtmlWindow CharCEF;
        Character[] characters;
        DateTime nextUpdate = DateTime.Now;

        public Selector()
        {
            Events.Add("client:showCharScreen", ShowCharScreen);
            Events.Add("client:hideCharScreen", HideCharScreen);
            //Events.Add("client:CharWalkIn", CharacterWalkIn);
            Events.Add("client:CharChangeToServer", CharChangeToServer);
            Events.Add("client:NewCharToServer", NewCharToServer);
            Events.Add("client:CharEditToServer", CharEditToServer);
            Events.Add("client:SelectCharacter", CharSelected);
        }

        private void CharSelected(object[] args)
        {
            Events.CallRemote("server:CharSelect", Convert.ToInt32(args[0]));//karakter id
        }

        private void CharEditToServer(object[] args)
        {
            Events.CallRemote("server:CharEdit", Convert.ToUInt32(args[0]));//karakter id
        }

        private void NewCharToServer(object[] args)
        {
            Events.CallRemote("server:NewChar");//karakter id
        }

        private void CharChangeToServer(object[] args)
        {
            if (DateTime.Now > nextUpdate)
            {
                TimeSpan span = TimeSpan.FromSeconds(1);
                nextUpdate = DateTime.Now + span;
                string location = RAGE.Game.Gxt.Get(Zone.GetNameOfZone(characters[GetCharIndexById(Convert.ToInt32(args[0]))].posX, characters[GetCharIndexById(Convert.ToInt32(args[0]))].posY, characters[GetCharIndexById(Convert.ToInt32(args[0]))].posZ));
                string pob = characters[GetCharIndexById(Convert.ToInt32(args[0]))].POB;
                string dob = characters[GetCharIndexById(Convert.ToInt32(args[0]))].DOB.ToString("yyyy.MM.dd.", CultureInfo.CurrentCulture);
                Events.CallRemote("server:CharChange", args[0].ToString());//ID
                CharCEF.ExecuteJs($"RefreshCharData(\"{characters[GetCharIndexById(Convert.ToInt32(args[0]))].Name}\", \"{location}\", \"{pob}\", \"{dob}\")");
            }
        }

        private int GetCharIndexById(int id)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        /*
        private void CharacterWalkIn(object[] args)
        {
            float x = Convert.ToSingle(args[0]);
            float y = Convert.ToSingle(args[1]);
            float z = Convert.ToSingle(args[2]);
            float rot = Convert.ToSingle(args[3]);
            //p.TaskGoStraightToCoord(-815.4f, 176.9f, 76.74f, 0.1f, -1, 57f, 0.1f);
            p.TaskGoStraightToCoord(x, y, z, 0.3f, -1, rot, 0.1f);
            CharCEF = new RAGE.Ui.HtmlWindow("package://frontend/character/char.html");
            CharCEF.Active = false;
        }
        */


        private async void ShowCharScreen(object[] args)
        {
            RAGE.Game.Graphics.TransitionFromBlurred(2000);
            if (Cameras.Cam.CheckSkyCam())//ha a levegőben van még a kamera várunk 1 másodpercet és meghívjuk újra ezt a függvényt
            {
                RAGE.Task.Run(() =>
                {
                    ShowCharScreen(args);
                }, 1000);
            }
            else//már nincs a levegőben a kamera, megnyithatjuk a menüt
            {
                
                RAGE.Game.Ui.DisplayHud(false);
                RAGE.Ui.Cursor.ShowCursor(true, true);
                CharCEF = new RAGE.Ui.HtmlWindow("package://frontend/character/char.html");
                CharCEF.Active = false;
                characters = RAGE.Util.Json.Deserialize<Character[]>(args[0].ToString());
                for (int i = 0; i < characters.Length; i++)
                {
                    CharCEF.ExecuteJs($"AddCharacter(\"{characters[i].Id}\", \"{characters[i].Name}\")");
                }

                string location = RAGE.Game.Gxt.Get(Zone.GetNameOfZone(characters[0].posX, characters[0].posY, characters[0].posZ));
                string pob = characters[0].POB;
                string dob = characters[0].DOB.ToString("yyyy.MM.dd.", CultureInfo.CurrentCulture);
                CharCEF.ExecuteJs($"SetFirstCharId(\"{characters[0].Id}\")");
                CharCEF.ExecuteJs($"RefreshCharData(\"{characters[0].Name}\", \"{location}\", \"{pob}\", \"{dob}\")");
                CharCEF.Active = true;
            }


        }


        private void HideCharScreen(object[] args)
        {
            CharCEF.Active = false;
            CharCEF.Destroy();
            RAGE.Ui.Cursor.ShowCursor(false, false);
            RAGE.Game.Ui.DisplayHud(true);
            Compass.DrawCompass(true);
        }
    }
}
