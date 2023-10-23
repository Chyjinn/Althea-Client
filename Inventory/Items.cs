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


            Events.Add("client:UseItemToServer", UseItem);

            Events.Add("client:MoveItemInInventory", MoveItemInInventory);
            Events.Add("client:MoveItemToContainer", MoveItemToContainer);

            Events.Add("client:ItemUseToCEF", ItemUseToCEF);
            RAGE.Input.Bind(73, true, ToggleInventory);
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
            bool inuse = Convert.ToBoolean(args[2]);
            Events.CallRemote("server:SwapItem", item1_dbid, item2_dbid, inuse);
        }

        private void AddItemToClothing(object[] args)
        {
            Item item = RAGE.Util.Json.Deserialize<Item>(Convert.ToString(args[0]));
            
            int item_slot = Convert.ToInt32(args[1]);

            InventoryCEF.ExecuteJs($"addItemToSlot(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{Convert.ToInt32(item_slot)}\")");
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
            uint target_id = Convert.ToUInt32(args[1]);
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
            int section = Convert.ToInt32(args[0]);
            int slot = Convert.ToInt32(args[1]);
            Events.CallRemote("server:UseItem", section, slot);
        }
        int hashClone = -1;
        public void ToggleInventory()
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
                    RAGE.Game.Graphics.TransitionToBlurred(500);
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
                        RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                        RAGE.Game.Entity.DeleteEntity(ref hashClone);
                    }, 100);

                }, 100);

            }
            else
            {
                RAGE.Game.Graphics.TransitionFromBlurred(500);
                InventoryCEF.Active = false;
                RAGE.Game.Ui.SetFrontendActive(false);
                RAGE.Ui.Cursor.ShowCursor(false, false);
                RAGE.Game.Entity.DeleteEntity(ref hashClone);
            }

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
            }, 100);

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
            }, 100);

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
            Chat.Output(args[0].ToString());
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
                //dbid,itemid, itemname, itemdescription, weight, amount, itempicture, priority){
                //Chat.Output(GetItemSection(item.ItemID) + ", " + item.ItemSlot + ", " + item.ItemID + ", " + item.ItemAmount + ", " + GetItemPicture(item.ItemID));
                InventoryCEF.ExecuteJs($"addItemToInventory(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\")");
            }

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
