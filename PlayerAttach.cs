using RAGE;
using RAGE.NUI;
using System;
using System.Collections.Generic;

namespace Client
{
    internal class PlayerAttach : Events.Script
    {
        private List<int> objList = new List<int>();
        Dictionary<RAGE.Elements.Player, RAGE.Game.Object> objDic = new Dictionary<RAGE.Elements.Player, RAGE.Game.Object>();
        public PlayerAttach()
        {
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;                        
            /*Key.Bind(Keys.VK_SPACE, true, () =>
            {
                var sharedData = player.GetSharedData("anim");
                if (sharedData != "")
                {
                    Events.CallRemote("server:stopAnim");
                }
                return 1;
            });*/

            Events.Add("client:getGroundHeight", getGroundHeight);
            Events.Add("client:getBoxDictionary", getBoxDictionary);         
        }        
                

        public void getGroundHeight(object[] args)
        {
            Vector3 argsToVector = (Vector3)args[0];
            float lastArgsToVectorZ = 0f;
            var playerHeightAboveGround = RAGE.Elements.Player.LocalPlayer.GetHeightAboveGround();
            //Chat.Output("Player above ground: " + playerHeightAboveGround);                
            argsToVector.Z -= playerHeightAboveGround;
            lastArgsToVectorZ = argsToVector.Z;
            //Chat.Output("Lefut a kliens" + lastArgsToVectorZ);
            Events.CallRemote("server:getGroundHeight", lastArgsToVectorZ);
        }

        public void attachBoxToPlayer(object[] args)
        {
            Events.CallRemote("server:getBoxDictonary");
        }

        public void getBoxDictionary(object[] args)
        {
            objDic.Add((RAGE.Elements.Player)args[0], (RAGE.Game.Object)args[1]);
            foreach (var item in objDic)
            {
                RAGE.Chat.Output("ObjDic kiíratás: " + item.Value);
            }
        }
                

    }
}