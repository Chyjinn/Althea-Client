﻿using RAGE;
using RAGE.Ui;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using Client.Characters;
using System.Linq;
using static System.Collections.Specialized.BitVector32;
using RAGE.Game;

namespace Client.Inventory
{
    public class Item
    {
        public uint DBID { get; set; }
        public uint OwnerID { get; set; }
        public int OwnerType { get; set; }
        public uint ItemID { get; set; }
        public string ItemValue { get; set; }//itemvalue, json
        public int ItemAmount { get; set; }
        public bool InUse { get; set; }
        public bool Duty { get; set; }
        public int Priority { get; set; }
        public Item(uint dbid, uint ownerid, int ownertype, uint itemid, string itemvalue, int itemamount, bool inuse, bool duty, int priority)
        {
            DBID = dbid;
            OwnerID = ownerid;
            OwnerType = ownertype;
            ItemID = itemid;
            ItemValue = itemvalue;
            ItemAmount = itemamount;
            Duty = duty;
            Priority = priority;
            InUse = inuse;
        }

        public bool IsContainer()
        {
            bool state = false;
            if (ItemID == 11)
            {
                state = true;
            }
            return state;
        }

    }


    public class Entry
    {
        public uint ItemID { get; set; }//itemid
        public string Name { get; set; }//item neve
        public string Description { get; set; }//leírás, ha van megjelenítjük
        public int ItemType { get; set; }//felhasználás kezeléséhez kell majd, pl Weapon akkor úgy kezeljük
        public string ItemImage { get; set; }//lehet local, pl. src/img.png, vagy url
        public uint ItemWeight { get; set; }
        public bool Stackable { get; set; }
        public Entry(uint id, string name, string desc, int type, uint weight, string itemimage, bool stack)
        {
            ItemID = id;
            Name = name;
            Description = desc;
            ItemType = type;
            ItemWeight = weight;
            ItemImage = itemimage;
            Stackable = stack;
        }

    }

    public class Items : Events.Script
    {
        static HtmlWindow InventoryCEF;
        static Entry[] itemList;
        public Items() { 
            InventoryCEF = new RAGE.Ui.HtmlWindow("package://frontend/inventory/inventory.html");
            InventoryCEF.Active = false;
            Events.Add("client:InventoryFromServer", ReloadInventory);
            Events.Add("client:ContainerFromServer", ReloadContainer);

            Events.Add("client:ItemListFromServer", ReloadItemList);

            Events.Add("client:MoveItemToClothing", MoveItemToClothing);
            
            Events.Add("client:AddItemToClothing", AddItemToClothing);

            Events.Add("client:RemoveItem", RemoveItem);//mindenhonnan töröl
            Events.Add("client:AddItemToInventory", AddItemToInventory);
            Events.Add("client:AddItemToContainer", AddItemToContainer);
            Events.Add("client:CloseContainer", CloseContainer);

            Events.Add("client:SwapItem", SwapItem);
            Events.Add("client:MoveItem", MoveItem);

            Events.Add("client:RefreshInventoryPreview", RefreshInventoryPreview);


            Events.Add("client:UseItem", UseItem);
            Events.Add("client:GetItemPriorities", GetItemPriorities);


            Events.Add("client:ChangeItemInUse", ChangeItemInUse);

            Events.Add("client:ItemUseToCEF", ItemUseToCEF);
            Events.OnClickWithRaycast += WorldClickToContainer;
        }

        private void CloseContainer(object[] args)
        {
            InventoryCEF.ExecuteJs($"HideContainer()");
        }

        private void AddItemToContainer(object[] args)
        {
            Item item = RAGE.Util.Json.Deserialize<Item>(args[0].ToString());
            InventoryCEF.ExecuteJs($"addItemToContainer(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{item.InUse}\")");
        }

        private void ReloadContainer(object[] args)
        {
            List<Item> container = RAGE.Util.Json.Deserialize<List<Item>>(args[0].ToString());
            ContainerToCEF(container);
        }

        private void ContainerToCEF(List<Item> inv)
        {
            InventoryCEF.ExecuteJs($"ClearContainer()");
            foreach (var item in inv)
            {
                InventoryCEF.ExecuteJs($"addItemToContainer(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{item.InUse}\")");
            }
            InventoryCEF.ExecuteJs($"ShowContainer()");
            ToggleInventory(true);
        }



        private void GetItemPriorities(object[] args)
        {
            string json = Convert.ToString(args[0]);
            
            Dictionary<uint, int> priorities = RAGE.Util.Json.Deserialize<Dictionary<uint, int>>(json);

            foreach (var item in priorities)
            {
                InventoryCEF.ExecuteJs($"setItemPrio(\"{item.Key}\",\"{item.Value}\")");//DB_ID és priority-t átküldjük minden létező itemre
            }
        }

        private void ChangeItemInUse(object[] args)
        {
            uint dbid = Convert.ToUInt32(args[0]);
            bool state = Convert.ToBoolean(args[1]);
            InventoryCEF.ExecuteJs($"setItemUse(\"{dbid}\",\"{state}\")");
        }

        private void WorldClickToContainer(int x, int y, bool up, bool right, float relativeX, float relativeY, Vector3 worldPos, int entityHandle)
        {
            if (!up && right && !InventoryCEF.Active)//jobb klikket lenyomta és zárva van az inventory
            {
                RAGE.Elements.Entity e = GetEntityFromRaycast(RAGE.Game.Cam.GetGameplayCamCoord(), worldPos, 0, -1);
                if (e != null)//entity-re klikkeltünk
                {
                    if (e.Type == RAGE.Elements.Type.Vehicle && RAGE.Elements.Player.LocalPlayer.Vehicle == null)//járműre klikkeltünk és nem ülünk járműben
                    {
                        //megnyitjuk a jármű inventory-ját -> szerver oldali kérés
                        Vector3 playerpos = RAGE.Elements.Player.LocalPlayer.Position;
                        Vector3 vehiclepos = e.Position;
                        float distance = RAGE.Game.Misc.GetDistanceBetweenCoords(playerpos.X, playerpos.Y, playerpos.Z, vehiclepos.X, vehiclepos.Y, vehiclepos.Z, true);
                        if(distance < 3f)
                        {
                            Events.CallRemote("server:OpenVehicleTrunk", e.RemoteId);
                        }
                    }
                    else if(e.Type == RAGE.Elements.Type.Vehicle && RAGE.Elements.Player.LocalPlayer.Vehicle != null)
                    {
                        if (e == RAGE.Elements.Player.LocalPlayer.Vehicle)
                        {
                            Events.CallRemote("server:OpenVehicleGloveBox");
                        }
                    }
                }
            } 
        }

        private static RAGE.Elements.Entity GetEntityFromRaycast(Vector3 fromCoords, Vector3 toCoords, int ignoreEntity, int flags)
        {
            bool hit = false;
            Vector3 endCoords = new Vector3();
            Vector3 surfaceNormal = new Vector3();
            RAGE.Elements.Entity EntityHit = null;
            int materialHash = -1;
            int elementHitHandle = -1;
            RAGE.Elements.Entity entityHit = null;
            int ray = RAGE.Game.Shapetest.StartShapeTestRay(fromCoords.X, fromCoords.Y, fromCoords.Z, toCoords.X, toCoords.Y, toCoords.Z, flags, ignoreEntity, 0);

            int curTemp = 0;

            int shapeResult = RAGE.Game.Shapetest.GetShapeTestResultEx(ray, ref curTemp, endCoords, surfaceNormal, ref materialHash, ref elementHitHandle);

            // I think GetAtHandle is still broken so:

            if (elementHitHandle > 0)
            {
                int entityType = RAGE.Game.Entity.GetEntityType(elementHitHandle);
                // 0 = nothing, probably something in the world.
                // 1 = Ped or Player.
                // 2 = Vehicle
                // 3 = Object
                if (entityType == 1)
                {
                    entityHit = RAGE.Elements.Entities.Players.All.FirstOrDefault(x => x.Handle == elementHitHandle);
                }
                else if (entityType == 2)
                {
                    entityHit = RAGE.Elements.Entities.Vehicles.All.FirstOrDefault(x => x.Handle == elementHitHandle);
                }
                else if (entityType == 3)
                {
                    entityHit = RAGE.Elements.Entities.Objects.All.FirstOrDefault(x => x.Handle == elementHitHandle);
                }
            }

            return entityHit;
        }

        private void MoveItem(object[] args)
        {
            uint item_dbid = Convert.ToUInt32(args[0]);
            uint target_inv = Convert.ToUInt32(args[1]);
            Events.CallRemote("server:MoveItem", item_dbid, target_inv);
        }

        private void SwapItem(object[] args)
        {
            uint item1_dbid = Convert.ToUInt32(args[0]);
            uint item2_dbid = Convert.ToUInt32(args[1]);
            Events.CallRemote("server:SwapItem", item1_dbid, item2_dbid);
        }

        private void AddItemToClothing(object[] args)
        {
            Item item = RAGE.Util.Json.Deserialize<Item>(Convert.ToString(args[0]));
            
            InventoryCEF.ExecuteJs($"addItemToSlot(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\")");
            //InventoryCEF.ExecuteJs($"addItemToSlot(1,1,\"Kesztyű\",\"\",500,50,\"https://pngimg.com/d/mma_gloves_PNG25.png\",5,0)");
            //addItemToSlot(1,1,"Kesztyű","",500,50,"https://pngimg.com/d/mma_gloves_PNG25.png",5,0)
        }

        private void RemoveItem(object[] args)
        {
            InventoryCEF.ExecuteJs($"RemoveItem(\"{Convert.ToUInt32(args[0])}\")");//DB_ID alapján törölje
        }

        private void RemoveItem(uint db_id)
        {
            InventoryCEF.ExecuteJs($"RemoveItem(\"{db_id}\")");//DB_ID alapján törölje
        }

        private void MoveItemToClothing(object[] args)
        {
            uint db_id = Convert.ToUInt32(args[0]);
            int target_id = Convert.ToInt32(args[1]);
            Events.CallRemote("server:MoveItemToClothing", db_id,target_id);
        }

        private void MoveItemToContainer(object[] args)
        {
            int source_dbid = Convert.ToInt32(args[0]);
            int target_id = Convert.ToInt32(args[1]);
            int target_dbid = Convert.ToInt32(args[2]);
            Events.CallRemote("server:MoveItemToContainer", source_dbid, target_id, target_dbid);
        }

        private void MoveItemInInventory(object[] args)
        {
            uint source_dbid = Convert.ToUInt32(args[0]);
            uint target_dbid = Convert.ToUInt32(args[1]);
            Events.CallRemote("server:MoveItemInInventory",source_dbid,target_dbid);
        }



        private void ItemUseToCEF(object[] args)
        {
            int section = Convert.ToInt32(args[0]);
            int slot = Convert.ToInt32(args[1]);
            int state = Convert.ToInt32(args[2]);
            InventoryCEF.ExecuteJs($"itemActive(\"{section}\",\"{slot}\",\"{state}\")");
        }

        private void UseItem(object[] args)
        {
            uint target_item_dbid = Convert.ToUInt32(args[0]);
            Events.CallRemote("server:UseItem", target_item_dbid);
        }


        static int hashClone = -1;
        static DateTime timeout = DateTime.Now;
        static TimeSpan span = TimeSpan.FromMilliseconds(500);
        public static void ToggleInventory()
        {
            if(DateTime.Now > timeout+span)
            {
                if (!InventoryCEF.Active && !RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat)
                {
                    float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();

                    hashClone = RAGE.Elements.Player.LocalPlayer.Clone(heading, true, true);
                    RAGE.Game.Graphics.TransitionToBlurred(300);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 265f, true, true, false, false);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);
                    RAGE.Task.Run(() =>
                    {
                        RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);                        
                        RAGE.Game.Ui.SetFrontendActive(true);
                        RAGE.Game.Ui.ActivateFrontendMenu(RAGE.Game.Misc.GetHashKey("FE_MENU_VERSION_RAGEBEAST"), true, -1);
                        
                        RAGE.Task.Run(() =>
                        {
                            RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
                            RAGE.Game.Invoker.Invoke(0x3CA6050692BC61B0, true);
                            RAGE.Game.Invoker.Invoke(0xECF128344E9FF9F1, true);
                            RAGE.Game.Invoker.Invoke(0x98215325A695E78A, false);
                            RAGE.Ui.Cursor.ShowCursor(true, true);
                            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 265f, true, true, false, false);

                            //RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                            //RAGE.Game.Entity.DeleteEntity(ref hashClone);
                            InventoryCEF.Active = true;
                            Events.Tick += DisablePauseMenu;
                        }, 100);

                    }, 50);

                }
                else
                {
                    InventoryCEF.ExecuteJs($"HideContainer()");
                    RAGE.Game.Graphics.TransitionFromBlurred(300);
                   
                    RAGE.Game.Ui.SetFrontendActive(false);
                    
                    RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                    RAGE.Game.Entity.DeleteEntity(ref hashClone);
                    //RAGE.Game.Entity.DeleteEntity(ref hashClone);
                    InventoryCEF.Active = false;
                    Events.Tick -= DisablePauseMenu;

                    RAGE.Ui.Cursor.ShowCursor(false, false);
                    Events.CallRemote("server:ClosedContainer");
                }
                timeout = DateTime.Now;
            }
        }

        public static void ToggleInventory(bool state)
        {
                if (state)
                {
                    if (!InventoryCEF.Active)
                    {
                    float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();

                    hashClone = RAGE.Elements.Player.LocalPlayer.Clone(heading, true, true);
                    RAGE.Game.Graphics.TransitionToBlurred(300);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 265f, true, true, false, false);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);
                    RAGE.Task.Run(() =>
                    {
                        RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);
                        RAGE.Game.Ui.SetFrontendActive(true);
                        RAGE.Game.Ui.ActivateFrontendMenu(RAGE.Game.Misc.GetHashKey("FE_MENU_VERSION_RAGEBEAST"), true, -1);

                        RAGE.Task.Run(() =>
                        {
                            RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
                            RAGE.Game.Invoker.Invoke(0x3CA6050692BC61B0, true);
                            RAGE.Game.Invoker.Invoke(0xECF128344E9FF9F1, true);
                            RAGE.Game.Invoker.Invoke(0x98215325A695E78A, false);
                            RAGE.Ui.Cursor.ShowCursor(true, true);
                            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 265f, true, true, false, false);

                            //RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                            //RAGE.Game.Entity.DeleteEntity(ref hashClone);
                            InventoryCEF.Active = true;
                            Events.Tick += DisablePauseMenu;
                        }, 100);

                    }, 50);
                }
                }
                else
                {
                    if (InventoryCEF.Active)
                    {
                        InventoryCEF.ExecuteJs($"HideContainer()");
                        RAGE.Game.Graphics.TransitionFromBlurred(300);
                   
                        RAGE.Game.Ui.SetFrontendActive(false);
                    
                        RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                        RAGE.Game.Entity.DeleteEntity(ref hashClone);
                        //RAGE.Game.Entity.DeleteEntity(ref hashClone);
                        InventoryCEF.Active = false;
                        Events.Tick -= DisablePauseMenu;

                        RAGE.Ui.Cursor.ShowCursor(false, false);
                        Events.CallRemote("server:ClosedContainer");
                }
                }
            
        }


        private static void DisablePauseMenu(List<Events.TickNametagData> nametags)
        {
            RAGE.Game.Pad.DisableControlAction(32, 200, true);
        }

        private void RefreshInventoryPreview()
        {
            float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();

            hashClone = RAGE.Elements.Player.LocalPlayer.Clone(heading, true, true);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y, RAGE.Elements.Player.LocalPlayer.Position.Z - 10f, true, true, false, false);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);
            RAGE.Task.Run(() =>
            {
                RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
            }, 250);

        }

        private void RefreshInventoryPreview(object[] args)
        {
            float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();

            hashClone = RAGE.Elements.Player.LocalPlayer.Clone(heading, true, true);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y, RAGE.Elements.Player.LocalPlayer.Position.Z - 10f, true, true, false, false);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);
            RAGE.Task.Run(() =>
            {
                RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
            }, 250);

        }

        public string GetItemPicture(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemImage;
                }
            }
            return "";
        }

        private void ReloadItemList(object[] args)
        {
            itemList = RAGE.Util.Json.Deserialize<Entry[]>(args[0].ToString());
        }

        private void ClearInventory(object[] args)
        {
            InventoryCEF.ExecuteJs($"ClearInventory()");
        }

        public static void ClearInventory()
        {
            InventoryCEF.ExecuteJs($"ClearInventory()");
        }

        public void ReloadInventory(object[] args)
        {
            InventoryCEF.ExecuteJs($"ClearInventory()");
            List<Item> inventory = RAGE.Util.Json.Deserialize<List<Item>>(args[0].ToString());

            //Chat.Output("OWNER: " + inventory[0].OwnerID.ToString());
            InventoryToCEF(inventory);
            //megkaptuk szervertől az inventory-t, át kell küldeni CEF-re.
        }

        private void AddItemToInventory(object[] args)
        {
            Item item = RAGE.Util.Json.Deserialize<Item>(args[0].ToString());
            InventoryCEF.ExecuteJs($"addItemToInventory(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{Convert.ToString(item.InUse)}\")");
        }

        private void InventoryToCEF(List<Item> inv)
        {
            foreach (var item in inv)
            {
                InventoryCEF.ExecuteJs($"addItemToInventory(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{Convert.ToString(item.InUse)}\")");
            }
            Events.CallRemote("server:SetWornClothing");
        }


        public static int GetItemType(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemType;
                }
            }
            return 0;
        }





        public static string GetItemNameById(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.Name;
                }
            }
            return "Nem létező item.";
        }

        public static string GetItemDescriptionById(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.Description;
                }
            }
            return "";
        }

        public static uint GetItemWeightById(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemWeight;
                }
            }
            return 0;
        }


        //CEF.Active = true;
    }
}
