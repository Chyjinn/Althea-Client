using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Jobs
{
    internal class Taxi : Events.Script
    {
        public Taxi()
        {
            Events.Add("client:TaxiLight", TaxiLight);
            Events.Add("client:CallTaxi", CallTaxi);
        }

        private void TaxiLight(object[] args)
        {
            bool state = RAGE.Elements.Player.LocalPlayer.Vehicle.IsTaxiLightOn();
            RAGE.Elements.Player.LocalPlayer.Vehicle.SetTaxiLights(!state);
        }


        private void CallTaxi(object[] args)
        {

            var position = RAGE.Elements.Player.LocalPlayer.Position;
            var tempStreet = 0;
            var tempCrossing = 0;
            var tempZone = "";
            Pathfind.GetStreetNameAtCoord(position.X, position.Y, position.Z, ref tempStreet, ref tempCrossing);
            tempZone = RAGE.Game.Zone.GetNameOfZone(position.X, position.Y, position.Z);

            string street = Ui.GetStreetNameFromHashKey((uint)tempStreet);
            string zone = Ui.GetLabelText(tempZone);
            Chat.Output("Taxi hívás kliens oldalon " + street + " - " + zone);
            Events.CallRemote("server:CallTaxi", street, zone);
        }
    }
}
