using Client.Inventory;
using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;

namespace Client.Characters
{
    class ClothingItem
    {
        public uint ID { get; set; }
        public bool Gender { get; set; }
        public uint Component { get; set; }
        public uint Category { get; set; }
        public string ItemValue { get; set; }
        public uint Price { get; set; }
        public ClothingItem(uint id, bool gender, uint component, uint category, string itemValue, uint price)
        {
            ID = id;
            Gender = gender;
            Component = component;
            Category = category;
            ItemValue = itemValue;
            Price = price;
        }
    }

    internal class Clothing : Events.Script
    {
        RAGE.Ui.HtmlWindow ClothesCEF;

        public Clothing()
        {
            Events.Add("client:LoadClothingShop", LoadClothingShop);

            //
            Events.Add("client:DrawableToServer", DrawableToServer);
            Events.Add("client:TextureToServer", TextureToServer);
            Events.Add("client:ClothingShop", OpenClothingShop);
            Events.Add("client:CloseClothingShop", CloseClothingShop);
        }

        private void LoadClothingShop(object[] args)
        {
            List<ClothingItem> items = RAGE.Util.Json.Deserialize<List<ClothingItem>>(args[0].ToString());

        }

        private void CloseClothingShop(object[] args)
        {
            ClothesCEF.Active = false;
            ClothesCEF.Destroy();
            Events.CallRemote("server:CloseClothingShop");
        }

        private void OpenClothingShop(object[] args)
        {
            bool state = (bool)args[0];

            if (state)
            {
                ClothesCEF = new RAGE.Ui.HtmlWindow("package://frontend/clothing/shop.html");
                ClothesCEF.Active = true;
            }
            else
            {
                ClothesCEF.Active = false;
                ClothesCEF.Destroy();

            }
        }
        int slot = 0;
        int drawable = 0;
        int texture = 0;
        private void TextureToServer(object[] args)
        {
            slot = Convert.ToInt32(args[0]);
            texture = Convert.ToInt32(args[1]);
            if (slot == 3)
            {

            }

            Player.LocalPlayer.SetComponentVariation(slot, drawable, texture, 0);
        }

        private void DrawableToServer(object[] args)
        {
            
            slot = Convert.ToInt32(args[0]);
            drawable = Convert.ToInt32(args[1]);
            Player.LocalPlayer.SetComponentVariation(slot, drawable, texture, 0);
        }
    }
}
