using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Interiors
{
    internal class Interiors : Events.Script
    {
        public Interiors()
        {
            Events.Add("client:GetCPHeight", CPGetZ);
            
        }

        public void CreateInterior(object[] args)
        {
            //float groundZ = RAGE.Game.Misc.GetGroundZFor3dCoord(objCoords.X, objCoords.Y, objCoords.Z, ref groundZ, false);
        }

        private void CPGetZ(object[] args)
        {
            float height = RAGE.Elements.Player.LocalPlayer.Position.Z - RAGE.Elements.Player.LocalPlayer.GetHeightAboveGround();
            Events.CallRemote("server:SetCPHeight", height);
        }
    }
}
