using RAGE;
using RAGE.Game;
using RAGE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Client
{
    internal class PlayerAttach : Events.Script
    {
        public PlayerAttach()
        {
            Events.Add("client:getGroundHeight", getGroundHeight);
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

    }
}