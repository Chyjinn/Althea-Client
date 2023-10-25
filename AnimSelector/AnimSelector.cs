using Client.AnimPanel;
using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.AnimSelector
{
    internal class AnimSelector : Events.Script
    {
        Anims animsClass = new Anims();
        HtmlWindow animSelectorWindow;
        int animIndex = 0;
        int animFlag = 0;
        public AnimSelector()
        {
            loadAndSortAllAnims();
            Events.Add("toggleSelectorWindow", toggleSelectorWindow);
            Events.Add("returnAnimtoJs", returnAnimtoJs);
            Events.Add("getFlagAndIdFromJs", getFlagAndIdFromJs);
            Events.Add("cefTest", cefTest);
            animSelectorWindow = new HtmlWindow("package://frontend/animselector/animselector.html");
            animSelectorWindow.Active = false;
            
        }

        private void cefTest(object[] args)
        {
            Chat.Output("Működik");
        }

        private void getFlagAndIdFromJs(object[] args)
        {
            animIndex = Convert.ToInt32(args[0]);
            animFlag = Convert.ToInt32(args[1]);
            Chat.Output(args[0] + " " + args[1]);
            Chat.Output("animIndex: " + animIndex + " animFlag: " + animFlag);

        }

        private void returnAnimtoJs(object[] args)
        {
            Dictionary<string, List<string>> animsDictionary = loadAndSortAllAnims();
            foreach (var anim in animsDictionary)
            {
                foreach (var animName in anim.Value)
                {
                    animSelectorWindow.ExecuteJs($"addAnimToContent(\"{animName}\",\"{anim.Key}\")");
                }

            }
        }


        private void toggleSelectorWindow(object[] args)
        {
            Chat.Output("animSelector");
            animSelectorWindow.Active = !animSelectorWindow.Active;
        }
        public Dictionary<string, List<string>> loadAndSortAllAnims()
        {
            List<string> animsList = animsClass.getAllAnims();
            Dictionary<string, List<string>> tempAnimsDictionary = new Dictionary<string, List<string>>();
            foreach (string anim in animsList)
            {
                if (tempAnimsDictionary.ContainsKey(anim.Split(' ')[0]))
                {
                    List<string> animNames = new List<string>
                    {
                        anim.Split(' ')[1]
                    };
                    tempAnimsDictionary[anim.Split(' ')[0]] = animNames;                    
                }
                else
                {
                    List<string> animNames = new List<string>
                    {
                        anim.Split(' ')[1]
                    };
                    tempAnimsDictionary.Add(anim.Split(' ')[0], animNames);                    
                }
            }
            return tempAnimsDictionary;
        }       

    }
}
