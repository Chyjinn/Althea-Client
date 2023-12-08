using Client.Inventory;
using RAGE;
using RAGE.Elements;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Client.Characters
{
    public class Clothing
    {
        public int Drawable { get; set; }
        public int Texture { get; set; }
        public Clothing(int drawable, int texture)
        {
            this.Drawable = drawable;
            this.Texture = texture;
        }
    }

    public class Top : Clothing
    {
        public int UndershirtDrawable { get; set; }
        public int UndershirtTexture { get; set; }
        public int Torso { get; set; }

        public Top(int drawable, int texture, int undershirtdraw, int undershirttext, int torso) : base(drawable, texture)
        {
            this.UndershirtDrawable = undershirtdraw;
            this.UndershirtTexture = undershirttext;
            this.Torso = torso;
        }
    }

    class ClothingItem
    {
        public uint ID { get; set; }
        public bool Gender { get; set; }
        public string Name { get; set; }
        public uint Component { get; set; }
        public uint Category { get; set; }
        public string ItemValue { get; set; }
        public uint Price { get; set; }
        public string Image { get; set; }
        public ClothingItem(uint id, bool gender, string name, uint component, uint category, string itemValue, uint price, string image)
        {
            ID = id;
            Gender = gender;
            Name = name;
            Component = component;
            Category = category;
            ItemValue = itemValue;
            Price = price;
            Image = image;
        }
    }

    internal class ClothingShop : Events.Script
    {
        static Dictionary<int, int> MaleClothingOffsets = new Dictionary<int, int>
        {
            //COMPONENET ID - OFFSET
            { 0, 45 },
            { 1, 225 },
            { 2, 80 },
            { 3, 210 },
            { 4, 176 },
            { 5, 110 },
            { 6, 134 },
            { 7, 174 },
            { 8, 198 },
            { 9, 57 },
            { 10, 173 },
            { 11, 494 }
        };

        static Dictionary<int, int> MaleAccessoryOffsets = new Dictionary<int, int>
        {
            //COMPONENET ID - OFFSET
            { 0, 194 },
            { 1, 52 },
            { 2, 41 },
            { 6, 46 },
            { 7, 13 }
        };

        static Dictionary<int, int> FemaleAccessoryOffsets = new Dictionary<int, int>
        {
            //COMPONENET ID - OFFSET
            { 0, 193 },
            { 1, 54 },
            { 2, 22 },
            { 6, 35 },
            { 7, 20 }
        };

        static Dictionary<int, int> FemaleClothingOffsets = new Dictionary<int, int>
        {
            //COMPONENET ID - OFFSET
            { 0, 45 },
            { 1, 226 },
            { 2, 84 },
            { 3, 244 },
            { 4, 190 },
            { 5, 110 },
            { 6, 141 },
            { 7, 144 },
            { 8, 244 },
            { 9, 57 },
            { 10, 190 },
            { 11, 533 }
        };

        public void SetClothingOffset(bool gender, int component, int offset)
        {
            if (gender)//férfi
            {
                MaleClothingOffsets[component] = offset;
            }
            else
            {
                FemaleClothingOffsets[component] = offset;
            }
        }

        public void SetAccessoryOffset(bool gender, int component, int offset)
        {
            if (gender)//férfi
            {
                MaleAccessoryOffsets[component] = offset;
            }
            else
            {
                FemaleAccessoryOffsets[component] = offset;
            }
        }

        public static int GetCorrectClothing(bool gender, int component, int drawable)//ha negatív szám akkor visszaadja a jó drawablet
        {
            if (gender)//férfi
            {
                if (drawable < 0)//negatív, modolt -> offset + abszolút érték
                {
                    return MaleClothingOffsets[component] + Math.Abs(drawable);
                }
                else
                {
                    return drawable;
                }
            }
            else
            {
                if (drawable < 0)//negatív, modolt -> offset + abszolút érték
                {
                    return FemaleClothingOffsets[component] + Math.Abs(drawable);
                }
                else
                {
                    return drawable;
                }
            }
        }

        public static int GetCorrectAccessory(bool gender, int component, int drawable)//ha negatív szám akkor visszaadja a jó drawablet
        {
            /*
            sapka 12
            szemcsi 13
            füles 14
            óra 15
            karkötő 16
            */
            switch (component)
            {
                case 12:
                    component = 0;
                    break;
                case 13:
                    component = 1;
                    break;
                case 14:
                    component = 2;
                    break;
                case 15:
                    component = 6;
                    break;
                case 16:
                    component = 7;
                    break;
            }

            if (gender)//férfi
            {
                if (drawable < 0)//negatív, modolt -> offset + abszolút érték
                {
                    return MaleAccessoryOffsets[component] + Math.Abs(drawable);
                }
                else
                {
                    return drawable;
                }
            }
            else
            {
                if (drawable < 0)//negatív, modolt -> offset + abszolút érték
                {
                    return FemaleAccessoryOffsets[component] + Math.Abs(drawable);
                }
                else
                {
                    return drawable;
                }
            }
        }

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


        public ClothingShop()
        {
            Events.Add("client:LoadClothingShop", LoadClothingShop);
            Events.Add("client:CloseClothingShop", CloseClothingShop);

            Events.Add("client:SelectClothingCategory", SelectClothingCategory);
            Events.Add("client:SelectClothing", SelectClothing);

            //
            //Events.Add("client:DrawableToServer", DrawableToServer);
            //Events.Add("client:TextureToServer", TextureToServer);
            //Events.Add("client:ClothingShop", OpenClothingShop);
            ClothesCEF = new RAGE.Ui.HtmlWindow("package://frontend/clothing/shop.html");
            ClothesCEF.Active = false;
        }

        private void SelectClothing(object[] args)
        {
            uint id = Convert.ToUInt32(args[0]);//kiválasztott ruha ID-je
            Chat.Output("RUHA ID: " + id);
            bool gender = true;//férfi
            if (RAGE.Elements.Player.LocalPlayer.Model == RAGE.Game.Misc.GetHashKey("mp_f_freemode_01"))//nő
            {
                gender = false;
            }
            else if (RAGE.Elements.Player.LocalPlayer.Model == RAGE.Game.Misc.GetHashKey("mp_m_freemode_01"))//férfi
            {
                gender = true;
            }
            else
            {
                return;
            }

            foreach (var item in Clothes)
            {
                if (item.ID == id)
                {
                    if (item.Component == 11)//ha top
                    {
                        //póló
                        Top t = RAGE.Util.Json.Deserialize<Top>(item.ItemValue);
                        
                        int correctDrawable = GetCorrectClothing(gender, Convert.ToInt32(item.Component), t.Drawable);
                        int correctUndershirtDrawable = GetCorrectClothing(gender, 8, t.UndershirtDrawable);

                        RAGE.Elements.Player.LocalPlayer.SetComponentVariation(Convert.ToInt32(item.Component), correctDrawable, t.Texture, 0);
                        RAGE.Elements.Player.LocalPlayer.SetComponentVariation(8, correctUndershirtDrawable, t.Texture, 0);
                        RAGE.Elements.Player.LocalPlayer.SetComponentVariation(3, t.Torso, 0, 0);
                        Chat.Output("PÓLÓ BEÁLLÍTVA");
                    }
                    else if(item.Component >= 0 && item.Component <= 10)
                    {
                        Clothing c = RAGE.Util.Json.Deserialize<Clothing>(item.ItemValue);
                        int correctDrawable = GetCorrectClothing(gender, Convert.ToInt32(item.Component), c.Drawable);

                        RAGE.Elements.Player.LocalPlayer.SetComponentVariation(Convert.ToInt32(item.Component), correctDrawable, c.Texture, 0);
                        Chat.Output("RUHA BEÁLLÍTVA");
                        //ruhadarab
                    }
                    else if(item.Component>=12 && item.Component <= 16)
                    {
                        Clothing c = RAGE.Util.Json.Deserialize<Clothing>(item.ItemValue);
                        int correctDrawable = GetCorrectAccessory(gender, Convert.ToInt32(item.Component), c.Drawable);
                        RAGE.Elements.Player.LocalPlayer.SetPropIndex(Convert.ToInt32(item.Component), correctDrawable, c.Texture, true);
                        Chat.Output("KIEG BEÁLLÍTVA");
                        //kiegészítő
                    }
                        
                    //megszerezzük a ruha itemvalue-ját és beállítjuk
                    //Ha találtunk megfelelő ruhadarbot ID alapján akkor beállítjuk, ha nem akkor baj van
                    break;
                }
            }
        }

        private List<ClothingItem> GetItemsByComponent(uint component)
        {
            List<ClothingItem> l = new List<ClothingItem>();
            foreach (var item in Clothes)
            {
                if (item.Component == component)
                {
                    l.Add(item);
                }
            }
            return l;
        }

        private void SelectClothingCategory(object[] args)
        {
            ClothesCEF.ExecuteJs($"clearClothes()");
            string cat = Convert.ToString(args[0]);
            List<ClothingItem> clothesToDisplay = new List<ClothingItem>();
            uint component = 0;
            Chat.Output(cat);
            //hozzáadjuk a kategóriát
            switch (cat)
            {
                case "mask":
                    component = 1;
                    break;
                case "legs":
                    component = 4;
                    break;
                case "bags":
                    component = 5;
                    break;
                case "shoes":
                    component = 6;
                    break;
                case "accessories":
                    component = 7;
                    break;
                case "armor":
                    component = 9;
                    break;
                case "decal":
                    component = 10;
                    break;
                case "shirt":
                    component = 11;
                    break;
                case "hat":
                    component = 12;
                    break;
                case "glasses":
                    component = 13;
                    break;
                case "ears":
                    component = 14;
                    break;
                case "watch":
                    component = 15;
                    break;
                case "bracelet":
                    component = 16;
                    break;
                default:
                    break;
            }
            Chat.Output("KOMPONENS: " + component);
            clothesToDisplay = GetItemsByComponent(component);

            foreach (var item in clothesToDisplay)
            {
                ClothesCEF.ExecuteJs($"addClothing(\"{item.ID}\",\"{item.Name}\",\"{item.Price}\",\"{item.Image}\")");
                Chat.Output(item.ID.ToString());
            }

            
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
                DisplayCategories[item.Component] = true;
                //minden ruhánál true-ra rakhatjuk hiszen akkor végül true lesz ami van benne
            }


            ClothesCEF = new RAGE.Ui.HtmlWindow("package://frontend/clothing/shop.html");
            ClothesCEF.Active = true;
            ClothesCEF.ExecuteJs($"clearCategories()");
            ClothesCEF.ExecuteJs($"clearClothes()");
            //kitöröljük az összes kategóriát
            foreach (var item in DisplayCategories)
            {
                if (item.Value == true)//ha az adott kategória van a boltban
                {
                    string category = "shirt";
                    //hozzáadjuk a kategóriát
                    switch (item.Key)
                    {
                        case 1:
                            category = "mask";
                            break;
                        case 4:
                            category = "legs";
                            break;
                        case 5:
                            category = "bags";
                            break;
                        case 6:
                            category = "shoes";
                            break;
                        case 7:
                            category = "accessories";
                            break;
                        case 9:
                            category = "armor";
                            break;
                        case 10:
                            category = "decal";
                            break;
                        case 11:
                            category = "shirt";
                            break;
                        case 12:
                            category = "hat";
                            break;
                        case 13:
                            category = "glasses";
                            break;
                        case 14:
                            category = "ears";
                            break;
                        case 15:
                            category = "watch";
                            break;
                        case 16:
                            category = "bracelet";
                            break;
                        default:
                            break;
                    }
                    ClothesCEF.ExecuteJs($"addCategory(\"{category}\")");
                    //majd a kiválaszott kategóriát ugyan így megfeleltetjük
                    //és az összes ruhából kiválasztjuk a megfelelőket (low-med-high összes, ár szerinti sorrendben adjuk majd át)
                    //itt még be kell dobni őt ruhaboltba/szerkesztő kamerába
                }
            }
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
