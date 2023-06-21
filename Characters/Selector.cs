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
        int[] disabledControls = new int[32]{ 30, // A & D
        31, // W & S
        21, // Left Shift
        36, // Left Ctrl
        22, // Space
        24, // Attack
        44, // Q
        38, // E
        71, // W - Vehicle
        72, // S - Vehicle
        59, // A & D - Vehicle
        60, // L Shift & L CTRL - Vehicle
        42, // D PAD Up || ]
        43, // D PAD Down || [
        85,
        86,
        15, // Mouse Wheel Up
        14, // Mouse Wheel Down
        228,
        229,
        172,
        173,
        37,
        44,
        178,
        244,
        220,
        221,
        218,
        219,
        16,
    17 };


        RAGE.Ui.HtmlWindow CharCEF;
        RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
        Character[] characters;
        DateTime nextUpdate = DateTime.Now;

        public Selector()
        {
            Events.Add("client:showCharScreen", ShowCharScreen);
            Events.Add("client:CharWalkIn", CharacterWalkIn);
            Events.Add("client:CharWalkOut", CharacterWalkOut);
            Events.Add("client:ChatStopWalk", CharacterStopWalk);
            Events.Add("client:CharChangeToServer", CharChangeToServer);
        }

        private void CharChangeToServer(object[] args)
        {
            if (DateTime.Now > nextUpdate)
            {
                TimeSpan span = TimeSpan.FromSeconds(3);
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

        private void CharacterStopWalk(object[] args)
        {
            p.ClearTasksImmediately();
        }

        private void CharacterWalkIn(object[] args)
        {
            //p.TaskGoStraightToCoord(-815.4f, 176.9f, 76.74f, 0.1f, -1, 57f, 0.1f);
            p.TaskGoStraightToCoord(-811.62f, 175.17f, 76.75f, 0.3f, -1, 107.7f, 0f);
        }

        private void CharacterWalkOut(object[] args)
        {
            p.TaskGoStraightToCoord(-815.4f, 176.9f, 76.75f, 0.3f, -1, 57f, 0f);
        }

        private void ShowCharScreen(object[] args)
        {
            CharCEF = new RAGE.Ui.HtmlWindow("package://frontend/character/char.html");
            CharCEF.Active = false;
            characters = RAGE.Util.Json.Deserialize<Character[]>(args[0].ToString());
            for (int i = 0; i < characters.Length; i++)
            {
                CharCEF.ExecuteJs($"AddCharacter(\"{characters[i].Id}\", \"{characters[i].Name}\")");
            }
            CharCEF.Active = true;
            Events.Tick += CharScreenControl;
        }

        private void CharScreenControl(List<Events.TickNametagData> nametags)
        {
            for (int i = 0; i < disabledControls.Length; i++)
            {
                Pad.DisableControlAction(0, disabledControls[i], true);
            }
        }
    }
}
