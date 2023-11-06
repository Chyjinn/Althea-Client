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
            Events.Add("client:playAnim", playAnim);
            Events.Add("client:uploadAnim", uploadAnim);
            animSelectorWindow = new HtmlWindow("package://frontend/animselector/animselector.html");
            animSelectorWindow.Active = false;
            
        }

        private void uploadAnim(object[] args)
        {
            //Player player, string cmd, string dict, string anim, int flag, string category
            string cmd = Convert.ToString(args[0]);
            string dict = Convert.ToString(args[1]);
            string anim = Convert.ToString(args[2]);
            int flag = Convert.ToInt32(args[3]);
            string category = Convert.ToString(args[4]);
            Events.CallRemote("server:AddAnimToDB", cmd, dict, anim, flag, category);
        }

        private void playAnim(object[] args)
        {
            string animDictPlay = Convert.ToString(args[0]);
            string animNamePlay = Convert.ToString(args[1]);
            int flag = Convert.ToInt32(args[2]);
            bool playPause = Convert.ToBoolean(args[3]);
            Events.CallRemote("server:playAnimation", animDictPlay, animNamePlay, flag, playPause);
            
        }

        private void getFlagAndIdFromJs(object[] args)
        {
            animIndex = Convert.ToInt32(args[0]);
            animFlag = Convert.ToInt32(args[1]);                        

        }

        private void returnAnimtoJs(object[] args)
        {            
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
            animSelectorWindow.ExecuteJs($"getAnimNameAndDictionary(\"{animDictionary}\", \"{animName}\")");
            
        }

        private void toggleSelectorWindow(object[] args)
        {            
            animSelectorWindow.Active = !animSelectorWindow.Active;
            Hud.NameTag.ChatCEF.Active = !Hud.NameTag.ChatCEF.Active;
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
