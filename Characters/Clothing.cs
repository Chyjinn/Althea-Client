﻿using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;

namespace Client.Characters
{
    internal class Clothing : Events.Script
    {
        RAGE.Ui.HtmlWindow ClothesCEF;

        public Clothing()
        {
            Events.Add("client:DrawableToServer", DrawableToServer);
            Events.Add("client:TextureToServer", TextureToServer);
            Events.Add("client:ClothingShop", OpenClothingShop);
            Events.Add("client:CloseClothingShop", CloseClothingShop);
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
