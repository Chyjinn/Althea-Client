using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;

namespace Client.Character
{
    class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AppearanceID { get; set; }
        public Appearance Appearance { get; set; }
        public Character(int charID, string charName, int charApperanceID)
        {
            Id = charID;
            Name = charName;
            AppearanceID = charApperanceID;
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
        internal class CharacterScreen : Events.Script
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

        public CharacterScreen()
        {
            Events.Add("client:showCharScreen", ShowCharScreen);
            Events.Add("client:CharWalkIn", CharacterWalkIn);
            Events.Add("client:CharWalkOut", CharacterWalkOut);
            Events.Add("client:ChatStopWalk", CharacterStopWalk);
            Events.Add("client:CharChangeToServer", CharChangeToServer);
        }
        private void CharChangeToServer(object[] args)
        {
            Chat.Output("Kliens megkapja");
            Events.CallRemote("server:CharChange", (string)args[0]);//ID
            CharCEF.ExecuteJs($"RefreshCharData(\"{characters[Convert.ToInt32(args[0])].Name}\", \"{characters[Convert.ToInt32(args[0])].AppearanceID}\")");
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
                CharCEF.ExecuteJs($"AddCharacter(\"{i}\", \"{characters[i].Name}\")");
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
