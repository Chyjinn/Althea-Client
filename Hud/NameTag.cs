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

        
        HtmlWindow ChatCEF;
        HtmlWindow VersionCEF;

        public NameTag() {

            RAGE.Nametags.Enabled = false;
            SetNameTagEnabled(true);
            
            Events.Add("client:Chat", ShowChat);
            Events.Add("client:BindKeys", BindKeys);


            RAGE.Chat.Show(false);
            ChatCEF = new HtmlWindow("package://frontend/chat/index.html");
            ChatCEF.Active = false;

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
                Events.Tick += Render;
                RAGE.Game.Graphics.RequestStreamedTextureDict("3dtextures", true);
            }
            else
            {
                Events.Tick -= Render;
            }
        }

        private static void Render(List<Events.TickNametagData> nametags)
        {
            
            if (nametags != null)
            {
                foreach (var item in nametags)
                {
                    if (item.Distance <= maxDistance)
                    {
                        RAGE.Elements.Player p = item.Player;
                        
                        Color nametagcolor = Color.White;
                        float scale = (maxDistance / item.Distance) / 5;
                        int screenX = 1920;
                        int screenY = 1080;
                        float x = item.ScreenX;
                        float y = item.ScreenY;


                            bool state = Convert.ToBoolean(p.GetSharedData("player:AdminDuty"));
                            if (state)
                            {
                                nametagcolor = Color.Orange;
                            if (RAGE.Game.Graphics.HasStreamedTextureDictLoaded("3dtextures"))
                            {
                                Chat.Output("be van töltve he");
                            }
                                RAGE.Game.Graphics.DrawSprite("helicopterhud", "hudarrow", screenX * x, screenY * y, 0.1f, 0.1f, p.GetHeading(), 255, 0, 0, 255, 50);
                            }

                        if (scale > 0.6f)
                        {
                            scale = 0.6f;
                        }
                        y -= scale * (0.005f * (screenY / 1080)); 
                        
                        
                        string name = Convert.ToString(p.GetSharedData("player:VisibleName"));
                        name = name.Replace('_', ' ');
                        int id = p.RemoteId;
                        RAGE.NUI.UIResText.Draw(name+" ("+id+")", Convert.ToInt32(screenX * x),Convert.ToInt32(screenY * y),  RAGE.Game.Font.ChaletComprimeCologne, scale, nametagcolor, RAGE.NUI.UIResText.Alignment.Centered, true, true, 300);
                        RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                        RAGE.Game.Graphics.DrawSprite("3dtextures", "mpgroundlogo_bikers", screenX * x, screenY * y, 0.1f, 0.1f, p.GetHeading(), 255, 0, 255, 120, 255);

                    }
                }
            }
        }
    }
}
