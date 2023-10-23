using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.AnimPanel
{
    internal class AnimPanel : Events.Script
    {
        public AnimPanel()
        {
            Events.Add("clientAnimsToPanel", clientAnimsToPanel);
        }

        private void clientAnimsToPanel(object[] args)
        {
            
        }
    }
}