using RAGE;
using System.Collections.Generic;

namespace Client.Hud
{
    internal class HealthBar : Events.Script
    {
        //RAGE.Ui.HtmlWindow CEF;
        public HealthBar()
        {

            //CEF = new RAGE.Ui.HtmlWindow("package://hp/test.html");
            //"C:\RAGEMP\server-files\client_packages\health\test.html"
            //CEF.Active = true;
            Events.Tick += UpdateHealth;
        }

        private void UpdateHealth(List<Events.TickNametagData> nametags)
        {
            RAGE.Game.Ui.HideHudComponentThisFrame(6);
            RAGE.Game.Ui.HideHudComponentThisFrame(7);
            RAGE.Game.Ui.HideHudComponentThisFrame(8);
            RAGE.Game.Ui.HideHudComponentThisFrame(9);
            RAGE.Game.Ui.HideHudComponentThisFrame(20);
            //CEF.ExecuteJs($"Update(\"{RAGE.Elements.Player.LocalPlayer.GetHealth()}\")");
        }

    }
}
