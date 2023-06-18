using RAGE;
using System;
using System.Collections.Generic;

namespace Client.Animator
{
    internal class AnimSelector : Events.Script
    {        
        public AnimSelector()
        {
            Events.Add("client:playAnimator", playAnimator);
            Events.Add("client:bindRightArrow", bindRightArrow);
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
            //RAGE.Chat.Output(player.GetSharedData("anim").ToString());
            /*if(player.GetSharedData("anim") == "true")
            {
                
            }*/            
            

        }               
        public void playAnimator(object[] args)
        {            
            List<string> animList = Animator.AllAnimations;
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
            string[] animDictName;
            Dictionary<string, List<string>> animDictAnimNameDick = new Dictionary<string,List<string>>();
            List<string> animNameList = new List<string>();

            foreach (var item in animList)
            {
                if(animDictAnimNameDick.ContainsKey(item.Split(' ')[0]))
                {
                    animNameList.Add(item.Split(' ')[1]);
                    animDictAnimNameDick[item.Split(' ')[0]] = animNameList;
                }
                else
                {
                    animNameList.Add(item.Split(' ')[1]);
                    animDictAnimNameDick.Add(item.Split(' ')[0], animNameList);
                }
                
            }
            string animDic = "";
            string animName = "";
            //Debug
            foreach (var item in animDictAnimNameDick)
            {
                RAGE.Chat.Output("Key: " + item.Key + " Value: " + item.Value);                
            }
          
        }

        public void bindRightArrow(object[] args)
        {
            Key.Bind(Keys.VK_RIGHT, true, () =>
            {
                Events.CallLocal("client:playAnimator");
                return 1;
            });
        }
    }
}
