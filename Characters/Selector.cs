using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Client.Character
{
    class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string POB { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public int AppearanceID { get; set; }
        public Appearance Appearance { get; set; }
        public Character(int Id, string Name, DateTime DOB, string POB, int AppearanceID, float posX, float posY, float posZ)
        {
            this.DOB = DOB;
            this.POB = POB;
            this.Id = Id;
            this.Name = Name;
            this.AppearanceID = AppearanceID;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
        }
    }

    public class Appearance
    {
        public int Id { get; set; }
        public bool Gender { get; set; }
        public byte EyeColor { get; set; }
        public byte HairColor { get; set; }
        public byte HairHighlight { get; set; }
        public byte Parent1Face { get; set; }
        public byte Parent2Face { get; set; }
        public byte Parent3Face { get; set; }
        public byte Parent1Skin { get; set; }
        public byte Parent2Skin { get; set; }
        public byte Parent3Skin { get; set; }
        public byte FaceMix { get; set; }
        public byte SkinMix { get; set; }
        public byte OverrideMix { get; set; }
        public sbyte NoseWidth { get; set; }
        public sbyte NoseHeight { get; set; }
        public sbyte NoseLength { get; set; }
        public sbyte NoseBridge { get; set; }
        public sbyte NoseTip { get; set; }
        public sbyte NoseBroken { get; set; }
        public sbyte BrowHeight { get; set; }
        public sbyte BrowWidth { get; set; }
        public sbyte CheekboneHeight { get; set; }
        public sbyte CheekboneWidth { get; set; }
        public sbyte CheekWidth { get; set; }
        public sbyte Eyes { get; set; }
        public sbyte Lips { get; set; }
        public sbyte JawWidth { get; set; }
        public sbyte JawHeight { get; set; }
        public sbyte ChinLength { get; set; }
        public sbyte ChinPosition { get; set; }
        public sbyte ChinWidth { get; set; }
        public sbyte ChinShape { get; set; }
        public sbyte NeckWidth { get; set; }
    }
        internal class Selector : Events.Script
    {
        RAGE.Ui.HtmlWindow CharCEF;
        RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
        Character[] characters;
        DateTime nextUpdate = DateTime.Now;

        public Selector()
        {
            Events.Add("client:showCharScreen", ShowCharScreen);
            Events.Add("client:hideCharScreen", HideCharScreen);
            Events.Add("client:CharWalkIn", CharacterWalkIn);
            Events.Add("client:CharChangeToServer", CharChangeToServer);
            Events.Add("client:SelectCharacter", CharSelected);
        }

        private void CharSelected(object[] args)
        {
            Events.CallRemote("server:CharSelect", Convert.ToInt32(args[0]));//karakter id
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

        private void CharacterWalkIn(object[] args)
        {
            float x = Convert.ToSingle(args[0]);
            float y = Convert.ToSingle(args[1]);
            float z = Convert.ToSingle(args[2]);
            float rot = Convert.ToSingle(args[3]);
            //p.TaskGoStraightToCoord(-815.4f, 176.9f, 76.74f, 0.1f, -1, 57f, 0.1f);
            p.TaskGoStraightToCoord(x, y, z, 0.3f, -1, rot, 0f);
            CharCEF = new RAGE.Ui.HtmlWindow("package://frontend/character/char.html");
            CharCEF.Active = false;
        }

        private void ShowCharScreen(object[] args)
        {
            characters = RAGE.Util.Json.Deserialize<Character[]>(args[0].ToString());
            for (int i = 0; i < characters.Length; i++)
            {
                CharCEF.ExecuteJs($"AddCharacter(\"{characters[i].Id}\", \"{characters[i].Name}\")");
            }
            CharCEF.Active = true;
        }


        private void HideCharScreen(object[] args)
        {
            CharCEF.Active = false;
            CharCEF.Destroy();
        }
    }
}
