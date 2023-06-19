using RAGE;
using RAGE.Elements;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

namespace Client.BoxCarrier
{
    internal class BoxCarrier : Events.Script
    {
        List<int> objects = new List<int>();
        Dictionary<RAGE.Elements.Player, List<int>> playerObjects = new Dictionary<RAGE.Elements.Player, List<int>>();
        public BoxCarrier()
        {
            Events.Add("client:attachBox", attachBox);
            Events.Add("client:destroyBoxes", destroyBoxes);
            Events.Add("client:destroyObjByHandle", destroyObjByHandle);
        }

        private void destroyObjByHandle(object[] args)
        {           
            int obj = Convert.ToInt32(args[0]);
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
            RAGE.Chat.Output("obj: " + obj + " Obj type" + obj.GetType());
            //RAGE.Game.Entity.SetEntityCoords(obj, player.Position.X, player.Position.Y, player.Position.Z, false, false, false, false);
            float rotX = Convert.ToSingle(args[1]);
            float rotY = Convert.ToSingle(args[2]);
            float rotZ = Convert.ToSingle(args[3]);
            //Ehhez a dobozhoz és animhoz kb. a megfelelő rot X-Y-Z: -100 -10 20
            RAGE.Game.Entity.AttachEntityToEntity(obj, player.Handle, RAGE.Game.Ped.GetPedBoneIndex(player.Handle, 0xdead), 0.1f, -0.05f, -0.25f, rotX, rotY, rotZ, false, false, false, false, 0, true);
            /*var objGame = RAGE.Elements.Entities.Objects.GetAtHandle(obj);            
            objGame.Position = RAGE.Elements.Player.LocalPlayer.Position;*/
            //RAGE.Game.Object.DeleteObject(ref obj);
        }

        private void destroyBoxes(object[] args)
        {
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
            //Events.CallRemote("server:LogChat", "Lelépett a szerverről, valszeg megbaszta a jó édes kurva anyját.");

            foreach (var item in playerObjects)
            {
                if (item.Key == player)
                {
                    foreach (var tempObj in objects)
                    {
                        int refObj = tempObj;
                        //Events.CallRemote("server:LogChat", "Lelépett a szerverről, valszeg megbaszta a jó édes kurva anyját.");
                        RAGE.Game.Object.DeleteObject(ref refObj);
                    }
                }
            }
        }

        private void attachBox(object[] args)
        {
            RAGE.Chat.Output("Kliens kérés lefut");
            RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
            Events.CallRemote("server:boxCreation");
            Key.Bind(Keys.VK_RIGHT, true, () =>
            {
                /*var valami = player.GetSharedData("objectEntity");
                var obj = RAGE.Elements.Entities.Objects.GetAt(Convert.ToUInt16(valami));                                
                Chat.Output("Működik a bind");
                if (valami != null)
                {
                    Chat.Output("Van valami az objectben" + valami.GetType());
                    Chat.Output("Object" + obj.ToString());                    
                
                }*/
                //...                
                var obj = RAGE.Game.Object.CreateObject(2930714276, player.Position.X, player.Position.Y, player.Position.Z, false, false, false);                
                Chat.Output("Object handle: " + obj);
                objects.Add(obj);
                if (playerObjects.ContainsKey(player))
                {                    
                    playerObjects[player] = objects;
                    Events.CallRemote("server:LogChat", "Ha már létrehozta");
                }
                else
                {
                    playerObjects.Add(player, objects);
                    Events.CallRemote("server:LogChat", "Ha még nem hozta létre a kulcsot");
                }
                /*RAGE.Chat.Output("obj:" + obj);
                RAGE.Chat.Output("Player Handle:" + player.Handle);
                RAGE.Chat.Output("player boneindex:" + player.GetBoneIndexByName("attach_male"));
                RAGE.Chat.Output("Player pos x - y -z:" + player.Position.X + " " + player.Position.Y + " " + " " + player.Position.Z);
                RAGE.Chat.Output("Player Rotation X - Y -Z :" + player.GetRotation(5).X + " " + player.GetRotation(5).Y + " " + player.GetRotation(5).Z);
                */
                //RAGE.Chat.Output("Object?: " + RAGE.Game.Entity.IsEntityAnObject(obj)); -- Megmondta, hogy egy object, NAGYON ÜGYES EZ A SZAR                                
                //gameObject.Position = player.Position;
                //RAGE.Chat.Output("Fasztudja miez" + RAGE.Game.Entity.GetWorldPositionOfEntityBone(obj, 0)); -- Azóta se tudom
                //RAGE.Game.Entity.AttachEntityToEntity(player.Handle, obj, 24818, player.Position.X, player.Position.Y, player.Position.Z, player.GetRotation(5).X, player.GetRotation(5).Y, player.GetRotation(5).Z, false, false, true, true, 2, true);
                //var gameObject = RAGE.Game.Object.GetClosestObjectOfType(player.Position.X, player.Position.Y, player.Position.Z, 1, 2930714276, false, false, false);  --Ide nem de ATM-hez jó.                
                //RAGE.Game.Entity.AttachEntityToEntityPhysically(obj, player.Handle, ,31086)                
                
                

                return 1;

            });

            Key.Bind(Keys.VK_DOWN, true, () =>
            {
                Events.CallLocal("client:destroyBoxes");
                return 1;
            });



        }

    }
}
