using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Jobs
{
    internal class Taxi : Events.Script﻿
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



            

            //RAGE.Game.Ui.SetPauseMenuActive(true);







            /*
            
            uint GetHash = RAGE.Game.Misc.GetHashKey("FE_MENU_VERSION_EMPTY");
            
            
            float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();
            
            Vector3 clonePos = RAGE.Game.Entity.GetEntityCoords(hashClone, false);
            


            RAGE.Chat.Output($"heading: {heading}");
            RAGE.Chat.Output($"GetHash: {GetHash}");
            RAGE.Chat.Output($"hashClone: {hashClone}");
            RAGE.Chat.Output($"clonePos: {clonePos}");
            //GetHash = RAGE.Util.Joaat.Hash("FE_MENU_VERSION_EMPTY_NO_BACKGROUND");
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.GivePedToPauseMenu, hashClone, 1);
            
            RAGE.Game.Invoker.Invoke(0xB8A850F20A067EB6, 67, 67);
            //RAGE.Game.Invoker.Invoke<bool>(RAGE.Game.Natives.ActivateFrontendMenu, GetHash, 0, -1);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives._0x98215325A695E78A, 0);
            RAGE.Game.Utils.Wait(100);

            RAGE.Game.Entity.SetEntityCoords(hashClone, clonePos.X, clonePos.Y, clonePos.Z - 10f, false, false, false, true);
            RAGE.Game.Entity.FreezeEntityPosition(hashClone, true);
            RAGE.Game.Entity.SetEntityVisible(hashClone, false, false);
            RAGE.Game.Network.NetworkSetEntityVisibleToNetwork(hashClone, false);
            RAGE.Game.Utils.Wait(200);
            RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);



            RAGE.Game.Invoker.Invoke(0x98215325a695e78a, 0);
            */

            RAGE.Chat.Output("Show");

            /*
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
            */
        }
    }
}
