using RAGE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Client.Hud
{
    internal class NameTag : Events.Script
    {
        const float maxDistance = 200f;
        const float width = 0.03f;
        const float height = 0.0065f;
        const float border = 0.001f;
        public NameTag() {
            Chat.Output("Nametag cucc");
            RAGE.Nametags.Enabled = false;
            SetNameTagEnabled(true);
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
