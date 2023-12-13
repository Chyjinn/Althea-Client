using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Minigames
{
    internal class WireGame : Events.Script
    {
        static HtmlWindow MinigameCEF;
        public WireGame()
        {
            Events.Add("client:ShowWireGame", ShowWireGame);
            Events.Add("client:CompleteMinigame", CompleteMinigame);
            Events.Add("client:FailMinigame", FailMinigame);

        }

        private void FailMinigame(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.SetToRagdoll(5000, 10000, 0, false, false, false);
            Chat.Output("Megrázott az áram!");
            MinigameCEF.Active = false;
            MinigameCEF.Destroy();
        }

        private void CompleteMinigame(object[] args)
        {
            Chat.Output("Gratulálok, teljesítetted a minigamet!");
            MinigameCEF.Active = false;
            MinigameCEF.Destroy();
        }

        private void ShowWireGame(object[] args)
        {
            MinigameCEF.Active = false;
            MinigameCEF.Destroy();
            MinigameCEF = new RAGE.Ui.HtmlWindow("package://frontend/wireGame/wireGame.html");
            MinigameCEF.Active = true;
        }

        private void HideWireGame()
        {
            MinigameCEF.Active = false;
        }
    }
}
