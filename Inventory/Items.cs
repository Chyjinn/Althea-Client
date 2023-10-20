using RAGE;
using RAGE.Ui;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using Client.Characters;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

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
        public bool Duty { get; set; }
        public int Priority { get; set; }
        public bool InUse { get; set; }
        public Item(uint dbid, uint ownerid, int ownertype, uint itemid, string itemvalue, int itemamount, bool duty, int priority)
        {
            DBID = dbid;
            OwnerID = ownerid;
            OwnerType = ownertype;
            ItemID = itemid;
            ItemValue = itemvalue;
            ItemAmount = itemamount;
            Duty = duty;
            Priority = priority;
            InUse = false;
        }
    }


    public class Entry
    {
        public uint ItemID { get; set; }//itemid
        public string Name { get; set; }//item neve
        public string Description { get; set; }//leírás, ha van megjelenítjük
        public int ItemType { get; set; }//felhasználás kezeléséhez kell majd, pl Weapon akkor úgy kezeljük
        public int ItemSection { get; set; }
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



    internal class Items : Events.Script
    {
        HtmlWindow InventoryCEF;
        static Entry[] itemList;
        static List<Item> inventory;
        public Items() { 
            InventoryCEF = new RAGE.Ui.HtmlWindow("package://frontend/inventory/inventory.html");
            InventoryCEF.Active = false;
            Events.Add("client:InventoryFromServer", ReloadInventory);
            Events.Add("client:ItemListFromServer", ReloadItemList);


            Events.Add("client:UseItemToServer", UseItem);

            Events.Add("client:MoveItemInInventory", MoveItemInInventory);
            Events.Add("client:MoveItemToContainer", MoveItemToContainer);

            Events.Add("client:ItemUseToCEF", ItemUseToCEF);
            RAGE.Input.Bind(73, true, ToggleInventory);
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

        public void ToggleInventory()
        {
            InventoryCEF.Active = !InventoryCEF.Active;
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

        public void ReloadInventory(object[] args)
        {
            InventoryCEF.ExecuteJs($"clearInventory()");
            inventory = RAGE.Util.Json.Deserialize<Item[]>(args[0].ToString()).ToList();
            Chat.Output(args[0].ToString());
            //Chat.Output("OWNER: " + inventory[0].OwnerID.ToString());
            InventoryToCEF();
            //megkaptuk szervertől az inventory-t, át kell küldeni CEF-re.
        }


        private void InventoryToCEF()
        {
            InventoryCEF.ExecuteJs($"clearInventory()");
            foreach (var item in inventory)
            {
                //dbid,itemid, itemname, itemdescription, weight, amount, itempicture, priority){
                //Chat.Output(GetItemSection(item.ItemID) + ", " + item.ItemSlot + ", " + item.ItemID + ", " + item.ItemAmount + ", " + GetItemPicture(item.ItemID));
                Chat.Output(item.DBID+","+item.ItemID);
                InventoryCEF.ExecuteJs($"addNewItem(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\")");
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

        public static int GetItemSection(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemSection;
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
