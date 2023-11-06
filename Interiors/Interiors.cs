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

        private void CPGetZ(object[] args)
        {
            float height = RAGE.Elements.Player.LocalPlayer.Position.Z - RAGE.Elements.Player.LocalPlayer.GetHeightAboveGround();
            Events.CallRemote("server:SetCPHeight", height);
        }
    }
}
