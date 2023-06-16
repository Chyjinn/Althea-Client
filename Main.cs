using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using RAGE;
using RAGE.Elements;
using RAGE.Game;
using RAGE.NUI;
using RAGE.Ui;

namespace Client
{
    public class Main : Events.Script
    {

        public Main() 
        {
            Interior.EnableInteriorProp(166657, "V_Michael_bed_tidy");
            Interior.EnableInteriorProp(166657, "V_Michael_M_items");
            Interior.EnableInteriorProp(166657, "V_Michael_D_items");
            Interior.EnableInteriorProp(166657, "V_Michael_S_items");
            Interior.EnableInteriorProp(166657, "V_Michael_L_Items");
            Interior.RefreshInterior(166657);

            Binds.bindKeys();
        }



        public void LoadIPL(object[] args)
        {
            string name = Convert.ToString(args[0]);
            RAGE.Game.Streaming.RequestIpl(name);
            RAGE.Chat.Output(RAGE.Game.Streaming.IsIplActive(name).ToString());
        }
    }
}
