using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Character
{
    internal class CharacterScreen : Events.Script
    {
        int[] disabledControls = new int[31]{ 30, // A & D
        31, // W & S
        21, // Left Shift
        36, // Left Ctrl
        22, // Space
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

        public CharacterScreen()
        {
            Events.Add("client:showCharScreen", ShowCharScreen);
            Events.Add("client:PedTest", PedTest);
            Events.Add("client:CharWalkIn", CharacterWalkIn);
            Events.Add("client:CharWalkOut", CharacterWalkOut);
            Events.Add("client:ChatStopWalk", CharacterStopWalk);
        }


        private void CharacterStopWalk(object[] args)
        {
            RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
            p.ClearTasksImmediately();
        }
        private void CharacterWalkIn(object[] args)
        {
            RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
            //p.TaskGoStraightToCoord(-815.4f, 176.9f, 76.74f, 0.1f, -1, 57f, 0.1f);
            p.TaskGoStraightToCoord(-811.62f, 175.17f, 76.75f, 0.3f, -1, 107.7f, 0f);
        }

        private void CharacterWalkOut(object[] args)
        {
            RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
            p.TaskGoStraightToCoord(-815.4f, 176.9f, 76.75f, 0.3f, -1, 57f, 0f);
            
        }

        private async void PedTest(object[] args)
        {
            RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
            switch ((int)args[0])
            {
                case 0:
                    p.TaskGoStraightToCoord(-813.36f, 173.24f, 76.74f, 0.1f, -1, -37f, 0.1f);
                    break;
                    case 1:
                    
                    break;
                    case 2:
                    
                    break;

                default:
                    break;
            }
            //-813.36, 173.24, 76.74, 0,0,-37.27 SPAWN
            //-811.76, 175.14, 76.74, 0, 0, 107.7 FŐ
            //-815.4f, 176.9f, 76.74f, 0, 0, 57 KISÉTÁLT

            
            //p.TaskWanderInArea(-814.6f, 177.9f, 76.74f, 0.2f, 1f, 1f);
            Chat.Output("Ped Test");
            //Vector3 v = new Vector3(-811.8f, 175.06f, 76.75f);
            //RAGE.Elements.Ped ped = new RAGE.Elements.Ped(0x9C9EFFD8, v);
            //KINT -814.6, 177.9, 76.74, 0, 0, -95
            //BENT -813.5, 173.2, 76.74, 0, 0, -167
           //ped.SetToRagdoll(5000, 5000, 2, true, true, true);
            //ped.TaskWanderInArea(-814.6f, 177.9f, 76.74f, 0.2f, 1f, 1f);
            //ped.TaskGoStraightToCoord(-814.6f, 177.9f, 76.74f, 2f, -1, 10f, 10f);
        }

        private void ShowCharScreen(object[] args)
        {
            CharCEF = new RAGE.Ui.HtmlWindow("package://frontend/character/char.html");
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


        private void ProcessCharacters(object[] args)
        {

        }
    }
}
