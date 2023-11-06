using Client.Inventory;
using RAGE;
using RAGE.Elements;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace Client.Hud
{
    public class Minimap
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float LeftX { get; set; }
        public float BottomY { get; set; }
        public float RightX { get; set; }
        public float TopY { get; set; }
    }

    internal class NameTag : Events.Script
    {
        const float maxDistance = 200f;
        const float width = 0.03f;
        const float height = 0.0065f;
        const float border = 0.001f;

        static HtmlWindow NametagCEF;
        HtmlWindow ChatCEF;
        HtmlWindow VersionCEF;

        public NameTag() {

            RAGE.Nametags.Enabled = false;
            SetNameTagEnabled(true);
            
            Events.Add("client:Chat", ShowChat);
            Events.Add("client:BindKeys", BindKeys);
            Events.Add("client:NametagTest", NametagTest);

            RAGE.Chat.Show(false);
            ChatCEF = new HtmlWindow("package://frontend/chat/index.html");
            ChatCEF.Active = false;

            NametagCEF = new HtmlWindow("package://frontend/nametag/nametag.html");
            NametagCEF.Active = true;

            VersionCEF = new HtmlWindow("package://frontend/version/ver.html");
            VersionCEF.Active = true;
            ChatCEF.MarkAsChat();
        }

        private void BindKeys(object[] args)
        {
            bool state = (bool)args[0];

            if (state)
            {
                Binds.Binds.bindKeys();
            }
            else
            {
                Binds.Binds.unbindKeys();
            }
        }

        private void ShowChat(object[] args)
        {
            bool state = (bool)args[0];

            if (state)
            {
                ChatCEF.Active = true;
            }
            else
            {
                ChatCEF.Active = false;
            }
        }



        public static void SetNameTagEnabled(bool status)
        {
            if (status)
            {
               
                RAGE.Game.Graphics.RequestStreamedTextureDict("3dtextures", true);
            }
            else
            {
                Events.Tick -= Render;
            }
        }


        static List<Ped> peds = new List<Ped>();
        private static void NametagTest(object[] args)
        {
            for (int i = -50; i < +50; i++)
            {
                Ped p = new Ped(RAGE.Game.Misc.GetHashKey("mp_m_freemode_01"), new Vector3(RAGE.Elements.Player.LocalPlayer.Position.X + i, RAGE.Elements.Player.LocalPlayer.Position.Y + i, RAGE.Elements.Player.LocalPlayer.Position.Z), 0f, 0);
                peds.Add(p);
            }
            Events.Tick += Render;


        }

        private static void Render(List<Events.TickNametagData> nametags)
        {
            NametagCEF.ExecuteJs($"clearNametags()");
            
            if (nametags != null)
            {
                foreach (var item in nametags)
                {
                    int screenX = 2560;
                    int screenY = 1440;
                    float x = 0;
                    float y = 0;

                    //RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(item.Position.X, item.Position.Y, item.Position.Z, ref x, ref y);
                    //Chat.Output(item.Id+" X:" + x + " Y:" + y);
                    if (x > -1f && x < 1f && y > -1f && y < 1f)
                    {
                        NametagCEF.ExecuteJs($"renderNametag(\"{item.Player.Name}\",\"{item.Player.Id}\",\"{Convert.ToInt32(item.ScreenX)}\",\"{Convert.ToInt32(item.ScreenY)}\",\"{0.6f}\")");
                    }
                }


                    //string name = Convert.ToString(p.GetSharedData("player:VisibleName"));
                    //name = name.Replace('_', ' ');
                    //int id = p.RemoteId;
                    //RAGE.NUI.UIResText.Draw(name + " (" + id + ")", Convert.ToInt32(screenX * x), Convert.ToInt32(screenY * y), RAGE.Game.Font.ChaletComprimeCologne, scale, nametagcolor, RAGE.NUI.UIResText.Alignment.Centered, true, true, 300);
                    //RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                    //RAGE.Game.Graphics.DrawSprite("3dtextures", "mpgroundlogo_bikers", screenX * x, screenY * y, 0.1f, 0.1f, p.GetHeading(), 255, 0, 255, 120, 255);

                    //Chat.Output("X:" + Convert.ToInt32(screenX * x) + "Y:" + Convert.ToInt32(screenY * y) + "SCALE:" + scale);
                   
                    //NametagCEF.ExecuteJs($"renderNametag(\"{0}\",\"{name}\",\"{Convert.ToInt32(screenX * x)}\",\"{Convert.ToInt32(screenY * y)}\",\"{scale}\")");

                

            }

            
            // InventoryCEF.ExecuteJs($"addItemToSlot(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\")");

        }
    }
}
