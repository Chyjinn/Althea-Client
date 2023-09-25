using RAGE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Client.Hud
{
    internal class Compass : Events.Script
    {
        private struct Config
        {
            public bool Show;
            public Vector3 Position;
            public bool Centered;
            public float Width;
            public float FOV;
            public bool FollowGameplayCam;
            public float TicksBetweenCardinals;
            public Color TickColour;
            public Vector3 TickSize;

            public struct CardinalConfig
            {
                public float TextSize;
                public float TextOffset;
                public Color TextColour;
                public bool TickShow;
                public Vector3 TickSize;
                public Color TickColour;
            }

            public CardinalConfig Cardinal;

            public struct IntercardinalConfig
            {
                public bool Show;
                public bool TextShow;
                public float TextSize;
                public float TextOffset;
                public Color TextColour;
                public bool TickShow;
                public Vector3 TickSize;
                public Color TickColour;
            }

            public IntercardinalConfig Intercardinal;

            public struct BackgroundConfig
            {
                public float X;
                public float Width;
                public float Height;
                public Color Color;
            }

            public BackgroundConfig Background;
        }

        private static Config compass;
        public Compass()
        {
            compass = new Config();
            compass.Show = true;
            compass.Position = new Vector3(0.5f, 0.02f,0f);
            compass.Centered = true;
            compass.Width = 0.25f;
            compass.FOV = 180f;
            compass.FollowGameplayCam = true;
            compass.TicksBetweenCardinals = 9.0f;
            compass.TickColour = Color.White;
            compass.TickSize = new Vector3(0.001f, 0.003f, 0f);

            compass.Cardinal.TextSize = 0.5f;
            compass.Cardinal.TextOffset = -0.01f;
            compass.Cardinal.TextColour = Color.White;
            compass.Cardinal.TickShow = true;
            compass.Cardinal.TickSize = new Vector3(0.001f, 0.012f, 0f);
            compass.Cardinal.TickColour = Color.White;

            compass.Intercardinal.Show = true;
            compass.Intercardinal.TextShow = false;
            compass.Intercardinal.TextSize = 0.5f;
            compass.Intercardinal.TextOffset = 0f;
            compass.Intercardinal.TextColour = Color.White;
            compass.Intercardinal.TickShow = true;
            compass.Intercardinal.TickSize = new Vector3(0.001f, 0.006f, 0f);
            compass.Intercardinal.TickColour = Color.White;

            compass.Background.X = 0.125f;
            compass.Background.Width = 0.26f;
            compass.Background.Height = 0.025f;
            compass.Background.Color = Color.Transparent;

            if (compass.Centered)
            {
                compass.Position.X = compass.Position.X - compass.Width / 2;
            }

        }

        public static void DrawCompass(bool state)
        {
            if (state)
            {
                Events.Tick += Tick;
            }
            else
            {
                Events.Tick -= Tick;
            }

        }


        private static string DegressToIntercardinalDirection(float deg)
        {
            deg = deg % 360;

            if (deg >= 0f && deg < 22.5f || deg >= 337.5f)
            {
                return "N";
            }
            else if (deg >= 22.5f && deg < 67.5f)
            {
                return "NE";
            }
            else if (deg >= 67.5f && deg < 112.5f)
            {
                return "E";
            }
            else if (deg >= 122.5f && deg < 157.5f)
            {
                return "SE";
            }
            else if (deg >= 157.5f && deg < 202.5f)
            {
                return "S";
            }
            else if (deg >= 202.5f && deg < 247.5f)
            {
                return "SW";
            }
            else if (deg >= 247.5f && deg < 292.5f)
            {
                return "W";
            }
            else if (deg >= 292.5f && deg < 337.5f)
            {
                return "NW";
            }

            return "";
        }
        private static void Tick(List<Events.TickNametagData> nametags)
        {

            if (compass.Show)
            {
                float pxDegree = compass.Width / compass.FOV;
                float playerHeadingDegrees = 0;

                if (compass.FollowGameplayCam)
                {
                    Vector3 camRot = RAGE.Game.Cam.GetGameplayCamRot(0);
                    playerHeadingDegrees = 360.0f - ((camRot.Z + 360.0f) % 360.0f);
                }
                else
                {
                    playerHeadingDegrees = 360.0f - RAGE.Elements.Player.LocalPlayer.GetHeading();
                }

                float tickDegree = playerHeadingDegrees - compass.FOV / 2;
                float tickDegreeRemainder = compass.TicksBetweenCardinals - (tickDegree % compass.TicksBetweenCardinals);
                float tickPosition = compass.Position.X + tickDegreeRemainder * pxDegree;

                tickDegree += tickDegreeRemainder;

                RAGE.Game.Graphics.DrawRect(compass.Position.X + compass.Background.X, compass.Position.Y, compass.Background.Width, compass.Background.Height, compass.Background.Color.R, compass.Background.Color.G, compass.Background.Color.B, compass.Background.Color.A, 0);

                while (tickPosition < compass.Position.X + compass.Width)
                {
                    if ((tickDegree % 90.0f) == 0)
                    {
                        // Draw cardinal
                        if (compass.Cardinal.TickShow)
                        {
                            RAGE.Game.Graphics.DrawRect(tickPosition, compass.Position.Y+0.01f, compass.Cardinal.TickSize.X + 0.01f,, compass.Cardinal.TickSize.Y, 0, 0, 0, 255, 0);
                            RAGE.Game.Graphics.DrawRect(tickPosition, compass.Position.Y, compass.Cardinal.TickSize.X, compass.Cardinal.TickSize.Y, compass.Background.Color.R, compass.Background.Color.G, compass.Background.Color.B, compass.Background.Color.A, 0);

                        }
                        Point pos = new Point();
                        pos.X = Convert.ToInt32(tickPosition*1280);
                        pos.Y = Convert.ToInt32((compass.Position.Y + compass.Cardinal.TextOffset)*720);
                        RAGE.Game.UIText.Draw(DegressToIntercardinalDirection(tickDegree), pos, compass.Cardinal.TextSize, compass.Cardinal.TextColour, RAGE.Game.Font.Pricedown, true);
                    }
                    else if ((tickDegree % 45.0f) == 0 || compass.Intercardinal.Show)
                    {
                        // Draw intercardinal
                        if (compass.Intercardinal.TickShow)
                        {
                            RAGE.Game.Graphics.DrawRect(tickPosition, compass.Position.Y, compass.Intercardinal.TickSize.X, compass.Intercardinal.TickSize.Y, compass.Intercardinal.TextColour.R, compass.Intercardinal.TextColour.G, compass.Intercardinal.TextColour.B, compass.Intercardinal.TextColour.A, 0);
                        }

                        if (compass.Intercardinal.TextShow)
                        {
                            Point pos = new Point();
                            pos.X = Convert.ToInt32(tickPosition * 1280);
                            pos.Y = Convert.ToInt32((compass.Position.Y + compass.Intercardinal.TextOffset) * 720);
                            RAGE.Game.UIText.Draw(DegressToIntercardinalDirection(tickDegree), pos, compass.Intercardinal.TextSize, compass.Intercardinal.TextColour, RAGE.Game.Font.Pricedown, true);
                        }
                    }
                    else
                    {
                        RAGE.Game.Graphics.DrawRect(tickPosition, compass.Position.Y, compass.TickSize.X, compass.TickSize.Y, compass.TickColour.R, compass.TickColour.G, compass.TickColour.B, compass.TickColour.A, 0);
                  
                    }

                    tickDegree += compass.TicksBetweenCardinals;
                    tickPosition += pxDegree * compass.TicksBetweenCardinals;
                }
            }
        }
    }
}
