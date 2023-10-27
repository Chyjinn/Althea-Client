using Client.AnimPanel;
using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Events.Add("client:playAnim", playAnim);
            animSelectorWindow = new HtmlWindow("package://frontend/animselector/animselector.html");
            animSelectorWindow.Active = false;
            
        }

        private void playAnim(object[] args)
        {
            throw new NotImplementedException();
        }

        private void cefTest(object[] args)
        {
            Chat.Output("Működik");
        }

        private void getFlagAndIdFromJs(object[] args)
        {
            animIndex = Convert.ToInt32(args[0]);
            animFlag = Convert.ToInt32(args[1]);                        

        }

        private void returnAnimtoJs(object[] args)
        {
            Chat.Output("Triggereli a kliens oldali returnAnimToJs-t");
            int animIndex = 0;
            int animFlag = 0;
            animIndex = Convert.ToInt32(args[0]);
            animFlag = Convert.ToInt32(args[1]);
            string animDictionary = "Faszomat";            
            string animName = "Bele";                                    
            List<string> animList = animsClass.getAllAnims();
            string[] animsArr = animList[animIndex].Split(' ');
            animDictionary = animsArr[0];
            animName = animsArr[1];
            Chat.Output("AnimIndex " + animIndex + " AnimFlag: " + animFlag);
            Chat.Output("AnimDictionary: " + animDictionary + " AnimName: " + animName);
            animSelectorWindow.ExecuteJs($"getAnimNameAndDictionary(\"{animDictionary}\", \"{animName}\")");
            
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
