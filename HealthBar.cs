using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Client
{
    internal class HealthBar: Events.Script
    {
        RAGE.Ui.HtmlWindow CEF;
        public HealthBar()
        {
            Events.Tick += UpdateHealth;
            CEF = new RAGE.Ui.HtmlWindow("package:\\health\test.html");
            //"C:\RAGEMP\server-files\client_packages\health\test.html"
            CEF.Active = true;
        }

        private void UpdateHealth(List<Events.TickNametagData> nametags)
        {
            CEF.ExecuteJs($"Update(\"{Player.LocalPlayer.GetHealth()}\")");

        }

    }
}
