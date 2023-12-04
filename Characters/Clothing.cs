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
        List<ClothingItem> Clothes = new List<ClothingItem>();
        Dictionary<uint, bool> DisplayCategories = new Dictionary<uint, bool>
        {
            { 1, false },
            { 4, false },
            { 5, false },
            { 6, false },
            { 7, false },
            { 9, false },
            { 10, false },
            { 11, false },
            { 12, false },
            { 13, false },
            { 14, false },
            { 15, false },
            { 16, false }
        };
        public Clothing()
        {
            Events.Add("client:LoadClothingShop", LoadClothingShop);
            Events.Add("client:CloseClothingShop", CloseClothingShop);
            //
            //Events.Add("client:DrawableToServer", DrawableToServer);
            //Events.Add("client:TextureToServer", TextureToServer);
            //Events.Add("client:ClothingShop", OpenClothingShop);
            ClothesCEF = new RAGE.Ui.HtmlWindow("package://frontend/clothing/shop.html");
            ClothesCEF.Active = false;
        }

        public void ResetCategories()
        {
            DisplayCategories = new Dictionary<uint, bool>
            {
                { 1, false },
                { 4, false },
                { 5, false },
                { 6, false },
                { 7, false },
                { 9, false },
                { 10, false },
                { 11, false },
                { 12, false },
                { 13, false },
                { 14, false },
                { 15, false },
                { 16, false }
            };
        }

        public List<ClothingItem> GetClothes(int category)
        {
            //gender a local player model alapján
            List<ClothingItem> res = new List<ClothingItem>();

            foreach (ClothingItem item in Clothes)
            {
                if (item.Category == category)
                {
                    res.Add(item);
                }
            }

            return res;
        }

        private void LoadClothingShop(object[] args)
        {
            DisplayCategories.Clear();
            ResetCategories();
            Clothes.Clear();
            Clothes = RAGE.Util.Json.Deserialize<List<ClothingItem>>(args[0].ToString());
            //megkaptuk az összes ruhát, meg akarjuk jeleníteni az összes kategóriát ami van
            foreach (var item in Clothes)
            {
                DisplayCategories[item.Category] = true;
                //minden ruhánál true-ra rakhatjuk hiszen akkor végül true lesz ami van benne
            }


            ClothesCEF = new RAGE.Ui.HtmlWindow("package://frontend/clothing/shop.html");
            ClothesCEF.Active = true;

            //megvan hogy melyik kategóriákat kell megjelenítenünk, ezt meg kell feleltetni JS-ben majd
            //ha rányom a kategóriára akkor pedig visszaküldjük neki az összes adott kategóriás ruhát
        }

        private void CloseClothingShop(object[] args)
        {
            ClothesCEF.Active = false;
            ClothesCEF.Destroy();
        }

        /*
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
        }*/
    }
}
