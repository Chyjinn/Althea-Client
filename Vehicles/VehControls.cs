using RAGE.Game;
using RAGE;
using System;
using System.Collections.Generic;
using RAGE.Elements;
using System.Linq;
using System.Security.Principal;

namespace Client.Vehicles
{
    class VehControls : Events.Script
    {
        RAGE.Ui.HtmlWindow SpeedCam;
        RAGE.Elements.Vehicle LastVehicle = null;
        float TopSpeed = 0f;

        public VehControls()
        {
            Events.Add("client:RadarGun", ToggleRadarGun);
            Events.AddDataHandler("vehicle:Siren", VehicleSiren);
            Events.AddDataHandler("vehicle:IndicatorLeft", IndicatorsLeft);
            Events.AddDataHandler("vehicle:IndicatorRight", IndicatorsRight);
            int leftbindid = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Left, true, IndicatorLeft);
            int rightbindid = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Right, true, IndicatorRight);
            Events.OnEntityStreamIn += OnEntityStreamIn;
        }
        public void IndicatorLeft()
        {
            Events.CallRemote("server:VehicleIndicator", false);
        }

        public void IndicatorRight()
        {
            Events.CallRemote("server:VehicleIndicator", true);
        }

        public void IndicatorsLeft(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            bool state = (bool)arg;
            RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtRemote(entity.RemoteId);
            v.SetIndicatorLights(1, state);
        }

        public void IndicatorsRight(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            bool state = (bool)arg;
            RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtRemote(entity.RemoteId);
            v.SetIndicatorLights(0, state);
        }

        private void ToggleRadarGun(object[] args)
        {
            bool state = Convert.ToBoolean(args[0]);

            if (state)
            {
                Events.Tick += Tick;
                SpeedCam = new RAGE.Ui.HtmlWindow("package://frontend/radar-gun/radar.html");
                SpeedCam.Active = true;
            }
            else
            {
                Events.Tick -= Tick;
                SpeedCam.Active = false;
                SpeedCam.Destroy();
            }
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            int endEntity = -1;
            RAGE.Game.Player.GetEntityPlayerIsFreeAimingAt(ref endEntity);
            int entityType = RAGE.Game.Entity.GetEntityType(endEntity);
            
            if (entityType == 1)
            {
                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtHandle(endEntity);
                if (p.Vehicle != null)
                {
                    RAGE.Elements.Vehicle v = p.Vehicle;
                    if (v != LastVehicle)
                    {
                        RAGE.Game.Audio.PlaySoundFrontend(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
                        LastVehicle = v;
                        TopSpeed = 0f;
                    }
                    float speed = v.GetSpeed();
                    if (speed > TopSpeed)
                    {
                        TopSpeed = speed;
                    }
                    int kmh = Convert.ToInt32(TopSpeed * 3.6);

                    Vector3 ppos = RAGE.Elements.Player.LocalPlayer.Position;
                    Vector3 vpos = v.Position;



                    float dist = RAGE.Game.Misc.GetDistanceBetweenCoords(ppos.X, ppos.Y, ppos.Z, vpos.X, vpos.Y, vpos.Z, true);
                    string speedres = kmh.ToString();
                    string speedpadded = speedres.PadLeft(3, '0');


                    
                    string distres = Convert.ToInt32(dist).ToString();
                    string distpadded = distres.PadLeft(3, '0');

                    SpeedCam.ExecuteJs($"updateSpeed('{speedpadded}')");
                    SpeedCam.ExecuteJs($"updateDist('{distpadded}')");
                    //Chat.Output(RAGE.Game.Vehicle.GetDisplayNameFromVehicleModel(v.Model).ToString() + " - " + speed + " M/S; " + kmh + " km/h; " + mph + " MPH");
                }

            }
            else if (entityType == 2)
            {
                RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtHandle(endEntity);
                if (v != LastVehicle)
                {
                    RAGE.Game.Audio.PlaySoundFrontend(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
                    LastVehicle = v;
                    TopSpeed = 0f;
                }
                float speed = v.GetSpeed();
                if (speed > TopSpeed)
                {
                    TopSpeed = speed;
                }
                int kmh = Convert.ToInt32(TopSpeed * 3.6);

                Vector3 ppos = RAGE.Elements.Player.LocalPlayer.Position;
                Vector3 vpos = v.Position;



                float dist = RAGE.Game.Misc.GetDistanceBetweenCoords(ppos.X, ppos.Y, ppos.Z, vpos.X, vpos.Y, vpos.Z, true);
                string speedres = kmh.ToString();
                string speedpadded = speedres.PadLeft(3, '0');



                string distres = Convert.ToInt32(dist).ToString();
                string distpadded = distres.PadLeft(3, '0');

                SpeedCam.ExecuteJs($"updateSpeed('{speedpadded}')");
                SpeedCam.ExecuteJs($"updateDist('{distpadded}')");
                //Chat.Output(RAGE.Game.Vehicle.GetDisplayNameFromVehicleModel(v.Model).ToString() + " - " + speed + " M/S; "+ kmh + " km/h; " + mph + " MPH");

            }
            /*
            Vector3 headPos = RAGE.Elements.Player.LocalPlayer.GetBoneCoords(31086, 0.0f, 0.0f, 0.0f);
            Vector3 offsetPos = RAGE.Game.Cam.GetGameplayCamRot(0);
                //RAGE.Game.Entity.GetOffsetFromEntityInWorldCoords(RAGE.Elements.Player.LocalPlayer.Handle, 0.0f, 2.0f, 0f);
            RAGE.Game.Graphics.DrawLine(headPos.X, headPos.Y, headPos.Z, offsetPos.X, offsetPos.Y, offsetPos.Z, 255, 0, 0, 255);
            int resultShape = RAGE.Game.Shapetest.StartShapeTestRay(headPos.X, headPos.Y, headPos.Z, offsetPos.X, offsetPos.Y, offsetPos.Z, 1, RAGE.Elements.Player.LocalPlayer.Handle, 7);

            int hit = -1;
            Vector3 endCoords = new Vector3();
            Vector3 surfaseNormal = new Vector3();
            int endEndidty = -1;
            
            int result = RAGE.Game.Shapetest.GetShapeTestResult(resultShape, ref hit, endCoords, surfaseNormal, ref endEndidty);

            RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtHandle(endEndidty);
            if (result != 0)
            {
                RAGE.Game.Graphics.DrawMarker(28, endCoords.X, endCoords.Y, endCoords.Z, 0.0f, 0.0f, 0.0f, 0.0f, 180.0f, 0.0f, 0.2f, 0.2f, 0.2f, 255, 128, 0, 50, false, true, 2, false, null, null, false);
                Chat.Output(v.Model.ToString());
            }*/
                
        }
/*
        public static RAGE.Elements.Player GetPlayerFromRaycast(Vector3 fromCoords, Vector3 toCoords, int ignoreEntity, int flags)
        {
            bool hit = false;
            Vector3 endCoords = new Vector3();
            Vector3 surfaceNormal = new Vector3();
            RAGE.Elements.Entity EntityHit = null;
            int materialHash = -1;
            int elementHitHandle = -1;

            int ray = RAGE.Game.Shapetest.StartShapeTestRay(fromCoords.X, fromCoords.Y, fromCoords.Z, toCoords.X, toCoords.Y, toCoords.Z, flags, ignoreEntity, 0);

            int curTemp = 0;

            int shapeResult = RAGE.Game.Shapetest.GetShapeTestResultEx(ray, ref curTemp, endCoords, surfaceNormal, ref materialHash, ref elementHitHandle);

            // I think GetAtHandle is still broken so:

            if (elementHitHandle > 0)
            {
                int entityType = RAGE.Game.Entity.GetEntityType(elementHitHandle);
                // 0 = nothing, probably something in the world.
                // 1 = Ped or Player.
                // 2 = Vehicle
                // 3 = Object
                if (entityType == 1)
                {
                    EntityHit = RAGE.Elements.Entities.Players.All.FirstOrDefault(x => x.Handle == elementHitHandle);
                }
            }

            return EntityHit;
        }
*/

        private void VehicleSiren(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            string siren = Convert.ToString(arg);
            RAGE.Chat.Output("SZIRÉNA: " + siren);

            RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtRemote(entity.RemoteId);
            //bool state = Convert.ToBoolean(arg);
            if (siren != "-")
                {
                    RAGE.Game.Audio.PlaySoundFromEntity(1, siren, v.Handle, "", true, 0);
                }
                else
                {
                    RAGE.Game.Audio.StopSound(1);
                }
        }



        public void OnEntityStreamIn(RAGE.Elements.Entity entity)
        {
            if (entity.Type == RAGE.Elements.Type.Vehicle)
            {
                
                RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtRemote(entity.RemoteId);

                if (v.GetSharedData("vehicle:IndicatorLeft") != null)
                {
                    bool state = (bool)v.GetSharedData("vehicle:IndicatorLeft");
                    v.SetIndicatorLights(1, state);
                }

                if (v.GetSharedData("vehicle:IndicatorRight") != null)
                {
                    bool state = (bool)v.GetSharedData("vehicle:IndicatorRight");
                    v.SetIndicatorLights(0, state);
                }

                if (v.GetSharedData("vehicle:Siren") != null)
                {
                    string siren = Convert.ToString(v.GetSharedData("vehicle:Siren"));
                    
                    if (siren != "-")
                    {
                        RAGE.Game.Audio.PlaySoundFromEntity(1, siren, v.Handle, "", true, 0);
                    }
                    else
                    {
                        RAGE.Game.Audio.StopSound(1);
                    }
                    
                }
            }



        }
    }
}
