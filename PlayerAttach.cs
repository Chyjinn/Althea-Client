using RAGE;
using System;
using System.Collections.Generic;

namespace Client
{
    internal class PlayerAttach : Events.Script
    {
        private List<int> objList = new List<int>();
        public PlayerAttach()
        {
            Events.Add("client:getGroundHeight", getGroundHeight);
            Events.Add("client:createBox", createBox);
            Events.OnPlayerQuit += playerQuit;
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

        public void createBox(object[] args)
        {
            var player = (RAGE.Elements.Player)args[0];
            var obj = RAGE.Game.Object.CreateObject(2930714276, player.Position.X, player.Position.Y, player.Position.Z, false, false, false);
            //RAGE.Chat.Output("Var obj kiíratás: " + obj);
            objList.Add(obj);


        }

        public void playerQuit(RAGE.Elements.Player client)
        {            
            if (client.RemoteId == RAGE.Elements.Player.LocalPlayer.RemoteId)
            {
                foreach (var item in objList)
                {
                    int refInt = item;
                    RAGE.Game.Object.DeleteObject(ref refInt);
                }
            }
        }

    }
}