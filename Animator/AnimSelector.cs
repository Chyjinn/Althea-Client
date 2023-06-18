using RAGE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.Animator
{
    internal class AnimSelector : Events.Script
    {        
        private int counter = 0;
        public AnimSelector()
        {
            Events.Add("client:playAnimator", playAnimator);
            Events.Add("client:bindRightArrow", bindRightArrow);
            Events.Add("client:unbindRightArrow", unbindRightArrow);            
            Events.Add("client:playAnimatorBackwards", playAnimatorBackwards);            
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;         
        }
        public void playAnimator(object[] args)
        {
            Dictionary<string, List<string>> animDictAnimNameDick = new Dictionary<string, List<string>>();
            List<string> animList = Animator.AllAnimations;
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
            string[] animDictName;

            foreach (var item in animList)
            {
                if (animDictAnimNameDick.ContainsKey(item.Split(' ')[0]))
                {
                    List<string> animNameList = new List<string>
                    {
                        item.Split(' ')[1]
                    };
                    animDictAnimNameDick[item.Split(' ')[0]] = animNameList;
                }
                else
                {
                    List<string> animNameList = new List<string>
                    {
                        item.Split(' ')[1]
                    };
                    animDictAnimNameDick.Add(item.Split(' ')[0], animNameList);
                }

            }
            string animDic = "";
            string animName = "";                 ;
            if (counter < animDictAnimNameDick.Count)
            {                
                animDic = animDictAnimNameDick.ElementAt(counter).Key;
                foreach (var item in animDictAnimNameDick[animDic])
                {
                    animName = item;                    
                }
                RAGE.Chat.Output("animDic:" + animDic + " animName: " + animName);
                Events.CallRemote("server:playAnim", animDic, animName);
                counter++;
                RAGE.Chat.Output("Counter: " + counter);
            }
            else if(counter >= animDictAnimNameDick.Count)
            {
                counter = 0;
            }
        }
        public void playAnimatorBackwards(object[] args)
        {
            Dictionary<string, List<string>> animDictAnimNameDick = new Dictionary<string, List<string>>();
            List<string> animList = Animator.AllAnimations;
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
            string[] animDictName;

            foreach (var item in animList)
            {
                if (animDictAnimNameDick.ContainsKey(item.Split(' ')[0]))
                {
                    List<string> animNameList = new List<string>
                    {
                        item.Split(' ')[1]
                    };
                    animDictAnimNameDick[item.Split(' ')[0]] = animNameList;
                }
                else
                {
                    List<string> animNameList = new List<string>
                    {
                        item.Split(' ')[1]
                    };
                    animDictAnimNameDick.Add(item.Split(' ')[0], animNameList);
                }

            }
            string animDic = "";
            string animName = "";
            if (counter < animDictAnimNameDick.Count && counter > 0)
            {
                counter--;
                RAGE.Chat.Output("Counter: " + counter);
                animDic = animDictAnimNameDick.ElementAt(counter).Key;
                foreach (var item in animDictAnimNameDick[animDic])
                {
                    animName = item;                    
                }
                RAGE.Chat.Output("animDic:" + animDic + " animName: " + animName);
                Events.CallRemote("server:playAnim", animDic, animName);
                
            }
            else if(counter >= animDictAnimNameDick.Count)
            {
                counter = 0;
            }
        }


        public void bindRightArrow(object[] args)
        {
            Key.Bind(Keys.VK_RIGHT, true, () =>
            {
                Events.CallRemote("server:stopAnim");
                Events.CallLocal("client:playAnimator");
                return 1;
            });
            Key.Bind(Keys.VK_UP, true, () =>
            {
                Events.CallRemote("server:stopAnim");
                Events.CallLocal("client:playAnimator");
                return 1;
            });

            Key.Bind(Keys.VK_SPACE, true, () =>
            {
                Events.CallRemote("server:stopAnim");
                return 1;
            });
            Key.Bind(Keys.VK_DOWN, true, () =>
            {
                Events.CallRemote("server:stopAnim");
                Events.CallLocal("client:playAnimatorBackwards");
                return 1;
            });
            Key.Bind(Keys.VK_LEFT, true, () =>
            {
                Events.CallRemote("server:stopAnim");
                Events.CallLocal("client:playAnimatorBackwards");
                return 1;
            });

        }

        public void unbindRightArrow(object[] args)
        {
            Key.Unbind(Keys.VK_RIGHT);
            Key.Unbind(Keys.VK_SPACE);
            Key.Unbind(Keys.VK_LEFT);
            Key.Unbind(Keys.VK_UP);
            counter = 0;
        }
    }
}
