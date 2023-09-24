using RAGE;
using RAGE.Elements;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

namespace Client.Hud
{
    internal class HealthBar : Events.Script
    {
       RAGE.Ui.HtmlWindow CEF;
        static HtmlWindow HudCEF;
        int MapScaleform;
        public HealthBar()
        {
            Minimap();
            Events.Add("client:HUD", SetHudVisible);
            //CEF = new RAGE.Ui.HtmlWindow("package://frontend/inventory/inventory.html");
            //CEF.Active = true;
            Events.Tick += UpdateHealth;
            HudCEF = new RAGE.Ui.HtmlWindow("package://frontend/hud/hud.html");
            HudCEF.Active = false;
            Events.OnPlayerEnterVehicle += PlayerEnterVehicle;
            Events.OnPlayerLeaveVehicle += PlayerLeaveVehicle;
            Events.AddDataHandler("vehicle:IndicatorRight", ToggleIndicator);
            Events.AddDataHandler("vehicle:IndicatorLeft", ToggleIndicator);
        }

        private void ToggleIndicator(Entity entity, object arg, object oldArg)
        {
            if (entity == RAGE.Elements.Player.LocalPlayer.Vehicle)
            {
                if ((bool)arg == true)
                {
                    HudCEF.ExecuteJs($"ToggleBlinker(1)");
                }
                else
                {
                    HudCEF.ExecuteJs($"ToggleBlinker(0)");
                }
            }
        }

        private void PlayerLeaveVehicle(Vehicle vehicle, int seatId)
        {
            HudCEF.ExecuteJs($"EnableVehicleGauges(0)");
        }

        private void PlayerEnterVehicle(Vehicle vehicle, int seatId)
        {
            HudCEF.ExecuteJs($"EnableVehicleGauges(1)");
        }

        public static Minimap GetMinimapAnchor()
        {
            float sfX = 1.0f / 20.0f;
            float sfY = 1.0f / 20.0f;

            // You need to replace these with the actual C# functions for retrieving safeZone, aspectRatio, and resolution.
            float safeZone = RAGE.Game.Graphics.GetSafeZoneSize();
            float aspectRatio = RAGE.Game.Graphics.GetAspectRatio(false);
            int resolutionX = 0; int resolutionY = 0;
            RAGE.Game.Graphics.GetActiveScreenResolution(ref resolutionX, ref resolutionY);

            float scaleX = 1.0f / resolutionX;
            float scaleY = 1.0f / resolutionY;

            Minimap minimap = new Minimap
            {
                Width = resolutionX * (scaleX * (resolutionX / (4 * aspectRatio))),
                Height = resolutionY * (scaleY * (resolutionY / 5.674f)),
                ScaleX = resolutionX * scaleX,
                ScaleY = resolutionY * scaleY,
                LeftX = resolutionX * (scaleX * (resolutionX * (sfX * (Math.Abs(safeZone - 1.0f) * 10f)))),
                BottomY = resolutionY * (1.0f - scaleY * (resolutionY * (sfY * (Math.Abs(safeZone - 1.0f) * 10f)))),
            };

            minimap.RightX = minimap.LeftX + minimap.Width;
            minimap.TopY = minimap.BottomY - minimap.Height;
            //Chat.Output(minimap.Width + ", " + minimap.Height + ", " + minimap.LeftX + ", " + minimap.RightX + ", " + minimap.TopY + ", " + minimap.BottomY);
            return minimap;
        }

        public async void Minimap()
        {
            MapScaleform = RAGE.Game.Graphics.RequestScaleformMovie("minimap");

            RAGE.Game.Ui.SetRadarBigmapEnabled(true, false); // Enables Bigmap ( Is Needed )
            await RAGE.Game.Invoker.WaitAsync(1000); //Timeout 1 Sec
            RAGE.Game.Ui.SetRadarBigmapEnabled(false, false); // Disable Bigmap
                                                              // N Direction Blip
            var northRadarBlip = RAGE.Game.Ui.Unknown._0x3F0CF9CB7E589B88(); // NATIVE UI _0x3F0CF9CB7E589B88
            RAGE.Game.Ui.SetBlipAlpha(northRadarBlip, 0);
        }


        private void SetHudVisible(object[] args)
        {
            bool state = (bool)args[0];

            if (state)
            {
                HudCEF.Active = true;
            }
            else
            {
                HudCEF.Active = false;
            }
        }
        Random r = new Random();
        DateTime nextUpdate = DateTime.Now;
        private void UpdateHealth(List<Events.TickNametagData> nametags)
        {
            if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
            {
                float speed = RAGE.Elements.Player.LocalPlayer.Vehicle.GetSpeed();
                int gear = RAGE.Elements.Player.LocalPlayer.Vehicle.Gear;
                int mph = Convert.ToInt32(speed * 2.236936);
                float rpm = RAGE.Elements.Player.LocalPlayer.Vehicle.Rpm;
                HudCEF.ExecuteJs($"RefreshSpeed(\"{Convert.ToInt32(mph)}\",\"{Math.Round(rpm*100,0)}\",\"{gear}\")");

                if (DateTime.Now > nextUpdate)
                {
                    TimeSpan span = new TimeSpan(5000000);
                    nextUpdate = DateTime.Now + span;
                    HudCEF.ExecuteJs($"RefreshFuel(\"{Convert.ToInt32(r.Next(0, 101))}\")");
                }
                
                //Chat.Output(speed + " m/s -  " + mph + " mph - " + rpm + " rpm");
            }
            Minimap map = GetMinimapAnchor();
            //Chat.Output(map.Width + "," + map.Height + "," + map.LeftX + "," + map.RightX + "," + map.TopY + "," + map.BottomY);
            RAGE.Game.Player.SetPlayerHealthRechargeMultiplier(0f);
            RAGE.Game.Ui.HideHudComponentThisFrame(6);
            RAGE.Game.Ui.HideHudComponentThisFrame(7);
            RAGE.Game.Ui.HideHudComponentThisFrame(8);
            RAGE.Game.Ui.HideHudComponentThisFrame(9);
            RAGE.Game.Ui.HideHudComponentThisFrame(20);
            RAGE.Game.Graphics.PushScaleformMovieFunction(MapScaleform, "SETUP_HEALTH_ARMOUR");
            RAGE.Game.Graphics.PushScaleformMovieFunctionParameterInt(3);
            RAGE.Game.Graphics.PopScaleformMovieFunction();
            int hp = RAGE.Elements.Player.LocalPlayer.GetHealth();
            int armor = RAGE.Elements.Player.LocalPlayer.GetArmour();
            HudCEF.ExecuteJs($"RefreshHealth(\"{hp - 100}\",\"{armor}\")");
            
            //HudCEF.ExecuteJs($" RefreshHealthBarPosition(\"{Convert.ToInt32(map.Width)}\", \"{Convert.ToInt32(map.Height)}\", \"{Convert.ToInt32(map.LeftX)}\", \"{Convert.ToInt32(map.RightX+20)}\", \"{Convert.ToInt32(map.TopY+10)}\", \"{Convert.ToInt32(map.BottomY)}\")");
            //CEF.ExecuteJs($"Update(\"{RAGE.Elements.Player.LocalPlayer.GetHealth()}\")");
        }

    }
}
