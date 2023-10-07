using RAGE;
using RAGE.Ui;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using Client.Characters;
using System.Linq;

namespace Client.Inventory
{
    public class Item
    {
        public uint DBID { get; set; }
        public int OwnerID { get; set; }
        public int OwnerType { get; set; }
        public uint ItemID { get; set; }
        public string ItemValue { get; set; }//itemvalue, json
        public int ItemAmount { get; set; }
        public bool Duty { get; set; }
        public int ItemSlot { get; set; }
        public bool InUse { get; set; }
        public Item(uint dbid, int ownerid, int ownertype, uint itemid, string itemvalue, int itemamount, bool duty, int itemslot)
        {
            DBID = dbid;
            OwnerID = ownerid;
            OwnerType = ownertype;
            ItemID = itemid;
            ItemValue = itemvalue;
            ItemAmount = itemamount;
            Duty = duty;
            ItemSlot = itemslot;
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
        public int MaxStack { get; set; }
        public Entry(uint id, string name, string desc, int type, int section, string itemimage, int stack)
        {
            ItemID = id;
            Name = name;
            Description = desc;
            ItemType = type;
            ItemSection = section;
            ItemImage = itemimage;
            MaxStack = stack;
        }

    }



    internal class Items : Events.Script
    {
        HtmlWindow InventoryCEF;
        static Entry[] itemList;
        static List<Item> inventory;
        public Items() { 
            InventoryCEF = new RAGE.Ui.HtmlWindow("package://frontend/inventory/inv.html");
            InventoryCEF.Active = false;
            Events.Add("client:InventoryFromServer", ReloadInventory);
            Events.Add("client:ItemListFromServer", ReloadItemList);
            Events.Add("client:UseItemToServer", UseItem);
            Events.Add("client:MoveItemInInventoryToServer", MoveItemInInv);
            Events.Add("client:ItemUseToCEF", ItemUseToCEF);
            RAGE.Input.Bind(73, true, ToggleInventory);
        }

        private void MoveItemInInv(object[] args)
        {
            int section = Convert.ToInt32(args[0]);
            int startslot = Convert.ToInt32(args[1]);
            int endslot = Convert.ToInt32(args[2]);
            Events.CallRemote("server:MoveItemInInventory", section, startslot, endslot);
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
                Chat.Output(GetItemSection(item.ItemID) + ", " + item.ItemSlot + ", " + item.ItemID + ", " + item.ItemAmount + ", " + GetItemPicture(item.ItemID));
                InventoryCEF.ExecuteJs($"addNewItem(\"{GetItemSection(item.ItemID)}\",\"{item.ItemSlot}\",\"{item.ItemID}\",\"{GetItemName(item.ItemID)}\",\"{GetItemDescription(item.ItemID)}\",\"{0}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\")");
            }

            //HudCEF.ExecuteJs($"RefreshHealth(\"{hp - 100}\",\"{armor}\")");
            //\"{Convert.ToInt32(r.Next(0, 101))}\"
            InventoryCEF.ExecuteJs($"loadInventory()");
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


        public static string GetItemName(uint itemid)
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
        public static string GetItemDescription(uint itemid)
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


        //CEF.Active = true;
    }
}
