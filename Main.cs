using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using RAGE;
using RAGE.Elements;
using RAGE.Game;
using RAGE.NUI;
using RAGE.Ui;

namespace Client
{
    public class Main : Events.Script
    {
        RAGE.Ui.HtmlWindow YTCEF;

        public Main() 
        {

            //Events.Tick += OnTick;
            Interior.EnableInteriorProp(166657, "V_Michael_bed_tidy");
            Interior.EnableInteriorProp(166657, "V_Michael_M_items");
            Interior.EnableInteriorProp(166657, "V_Michael_D_items");
            Interior.EnableInteriorProp(166657, "V_Michael_S_items");
            Interior.EnableInteriorProp(166657, "V_Michael_L_Items");
            Interior.RefreshInterior(166657);
            Events.Add("client:YTtest", YoutubeTest);
            Events.Add("client:LoadIPL", LoadIPL);
            Events.Add("client:SetWind", SetWind);
        }


        private void SetWind(object[] args)
        {
            float dir = Convert.ToSingle(args[0]);
            float speed = Convert.ToSingle(args[1]);
            RAGE.Game.Misc.SetWindDirection(dir);
            //RAGE.Game.Misc.SetWindSpeed(speed);
            RAGE.Game.Misc.SetWind(speed);

        }

        private void YoutubeTest(object[] args)
        {
            
            YTCEF = new HtmlWindow("package://frontend/loadscreen/loadscreen.html");
            YTCEF.Active = true;
            int x = 1920;
            int y = 1080;
            
            RAGE.Game.Graphics.GetActiveScreenResolution(ref x,ref y);
            YTCEF.ExecuteJs($"SetResolution(\"{x}\", \"{y}\")");
            RAGE.Task.Run(() =>
            {
                YTCEF.Active = false;
                YTCEF.Destroy();
            }, 5000);

        }

        public void LoadIPL(object[] args)
        {
            string name = Convert.ToString(args[0]);
            RAGE.Game.Streaming.RequestIpl(name);
            RAGE.Chat.Output(name + " IPL státusz:" + RAGE.Game.Streaming.IsIplActive(name).ToString());
        }
    }
}
