using RAGE;
using System.Collections.Generic;

namespace Client.Hud
{
    internal class HealthBar : Events.Script
    {
        //RAGE.Ui.HtmlWindow CEF;
        int MapScaleform;
        public HealthBar()
        {
            Minimap();
            //CEF = new RAGE.Ui.HtmlWindow("package://hp/test.html");
            //"C:\RAGEMP\server-files\client_packages\health\test.html"
            //CEF.Active = true;
            Events.Tick += UpdateHealth;
        }

        public async void Minimap()
        {
            MapScaleform = RAGE.Game.Graphics.RequestScaleformMovie("minimap");

            RAGE.Game.Ui.SetRadarBigmapEnabled(true, false); // Enables Bigmap ( Is Needed )
            await RAGE.Game.Invoker.WaitAsync(1000); //Timeout 1 Sec
            RAGE.Game.Ui.SetRadarBigmapEnabled(false, false); // Disable Bigmap
                                                              // N Direction Blip
            var northRadarBlip = RAGE.Game.Ui.Unknown._0x3F0CF9CB7E589B88(); // NATIVE UI _0x3F0CF9CB7E589B88
            RAGE.Game.Ui.SetBlipAlpha(northRadarBlip, 0);
        }

        private void UpdateHealth(List<Events.TickNametagData> nametags)
        {
            RAGE.Game.Ui.HideHudComponentThisFrame(6);
            RAGE.Game.Ui.HideHudComponentThisFrame(7);
            RAGE.Game.Ui.HideHudComponentThisFrame(8);
            RAGE.Game.Ui.HideHudComponentThisFrame(9);
            RAGE.Game.Ui.HideHudComponentThisFrame(20);
            RAGE.Game.Graphics.PushScaleformMovieFunction(MapScaleform, "SETUP_HEALTH_ARMOUR");
            RAGE.Game.Graphics.PushScaleformMovieFunctionParameterInt(3);
            RAGE.Game.Graphics.PopScaleformMovieFunction();
            //CEF.ExecuteJs($"Update(\"{RAGE.Elements.Player.LocalPlayer.GetHealth()}\")");
        }

    }
}
