using RAGE;
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
            Events.Add("client:ItemListFromServer", ReloadItemList);

            Events.Add("client:MoveItemToClothing", MoveItemToClothing);
            
            Events.Add("client:AddItemToClothing", AddItemToClothing);

            Events.Add("client:RemoveItem", RemoveItem);//mindenhonnan töröl
            Events.Add("client:AddItemToInventory", AddItemToInventory);

            Events.Add("client:SwapItem", SwapItem);
            Events.Add("client:MoveItem", MoveItem);

            Events.Add("client:RefreshInventoryPreview", RefreshInventoryPreview);


            Events.Add("client:UseItem", UseItem);
            Events.Add("client:ChangeItemInUse", ChangeItemInUse);

            Events.Add("client:MoveItemInInventory", MoveItemInInventory);
            Events.Add("client:MoveItemToContainer", MoveItemToContainer);

            Events.Add("client:ItemUseToCEF", ItemUseToCEF);
            Events.OnClickWithRaycast += WorldClickToContainer;
        }

        private void ChangeItemInUse(object[] args)
        {
            uint dbid = Convert.ToUInt32(args[0]);
            string state = Convert.ToString(args[1]);
            InventoryCEF.ExecuteJs($"setItemUse(\"{dbid}\",\"{state}\")");
        }

        private void WorldClickToContainer(int x, int y, bool up, bool right, float relativeX, float relativeY, Vector3 worldPos, int entityHandle)
        {
            if (!up && right)
            {
                RAGE.Elements.Entity e = GetEntityFromRaycast(RAGE.Game.Cam.GetGameplayCamCoord(), worldPos, 0, -1);
                if(e.Type == RAGE.Elements.Type.Vehicle)
                {
                    Chat.Output("Típus: " + RAGE.Game.Vehicle.GetDisplayNameFromVehicleModel(e.Model));
                }
            } 
        }

        public static RAGE.Elements.Entity GetEntityFromRaycast(Vector3 fromCoords, Vector3 toCoords, int ignoreEntity, int flags)
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
            uint item1_dbid = Convert.ToUInt32(args[0]);
            uint owner_type = Convert.ToUInt32(args[1]);
            uint owner_id = Convert.ToUInt32(args[2]);
            Events.CallRemote("server:MoveItem", item1_dbid, owner_type, owner_id);
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
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y, RAGE.Elements.Player.LocalPlayer.Position.Z+15f,true, true, false, false);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);

                    RAGE.Task.Run(() =>
                    {
                        RAGE.Game.Graphics.TransitionToBlurred(300);
                        RAGE.Game.Ui.SetFrontendActive(true);
                        RAGE.Game.Ui.ActivateFrontendMenu(RAGE.Game.Misc.GetHashKey("FE_MENU_VERSION_RAGEBEAST"), true, -1);

                        RAGE.Task.Run(() =>
                        {
                            RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
                            RAGE.Game.Invoker.Invoke(0x3CA6050692BC61B0, true);
                            RAGE.Game.Invoker.Invoke(0xECF128344E9FF9F1, true);
                            RAGE.Game.Invoker.Invoke(0x98215325A695E78A, false);
                            RAGE.Ui.Cursor.ShowCursor(true, true);
                            InventoryCEF.Active = true;
                            RAGE.Game.Invoker.Invoke(Natives.DisablePedPainAudio, hashClone, 1);
                            RAGE.Game.Invoker.Invoke(Natives.SetBlockingOfNonTemporaryEvents, hashClone, 1);
                            RAGE.Game.Invoker.Invoke(Natives.StopPedSpeaking, hashClone, 1);
                            //RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                            //RAGE.Game.Entity.DeleteEntity(ref hashClone);
                            Events.Tick += DisablePauseMenu;
                        }, 100);

                    }, 100);

                }
                else
                {
                    RAGE.Game.Graphics.TransitionFromBlurred(300);
                    InventoryCEF.Active = false;
                    RAGE.Game.Ui.SetFrontendActive(false);
                    RAGE.Ui.Cursor.ShowCursor(false, false);
                    
                    RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                    RAGE.Game.Entity.DeleteEntity(ref hashClone);
                    //RAGE.Game.Entity.DeleteEntity(ref hashClone);
                    Events.Tick -= DisablePauseMenu;
                }
                timeout = DateTime.Now;
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
            List<Item> inventory = RAGE.Util.Json.Deserialize<Item[]>(args[0].ToString()).ToList();

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
                /*              
                                if (item.ItemID >= 1 && item.ItemID <= 12 && item.InUse)//ruha itemid és használatban van
                                {
                                    int target_id = -1;
                                    switch (item.ItemID)
                                    {
                                        case 1://kalap
                                            target_id = 0;
                                            break;
                                        case 2://maszk
                                            target_id = 6;
                                            break;
                                        case 3://nyaklánc
                                            target_id = 1;
                                            break;
                                        case 4://szemüveg
                                            target_id = 7;
                                            break;
                                        case 5://póló
                                            target_id = 2;
                                            break;
                                        case 6://fülbevaló
                                            target_id = 8;
                                            break;
                                        case 7://nadrág
                                            target_id = 3;
                                            break;
                                        case 8://karkötő
                                            target_id = 9;
                                            break;
                                        case 9://cipő
                                            target_id = 4;
                                            break;
                                        case 10://óra
                                            target_id = 10;
                                            break;
                                        case 11://táska
                                            target_id = 5;
                                            break;
                                        case 12://páncél
                                            target_id = 11;
                                            break;
                                        default:
                                            break;
                                    }
                                    if (target_id != -1)
                                    {
                                        Events.CallRemote("server:MoveItemToClothing", item.DBID, target_id);
                                        RAGE.Game.Utils.Wait(250);
                                        Chat.Output("EQUIP: " + item.DBID + ", " + target_id);
                                    }
                                }
                                else*/



                
                //dbid,itemid, itemname, itemdescription, weight, amount, itempicture, priority){
                //Chat.Output(GetItemSection(item.ItemID) + ", " + item.ItemSlot + ", " + item.ItemID + ", " + item.ItemAmount + ", " + GetItemPicture(item.ItemID));

            }
            Events.CallRemote("server:SetWornClothing");
            //HudCEF.ExecuteJs($"RefreshHealth(\"{hp - 100}\",\"{armor}\")");
            //\"{Convert.ToInt32(r.Next(0, 101))}\"
            //InventoryCEF.ExecuteJs($"loadInventory()");
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
