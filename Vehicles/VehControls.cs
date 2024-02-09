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
            Events.Add("client:SpeedCam", ToggleSpeedCam);

            Events.Add("client:DamageVehicle", DamageVehicle);

            Events.AddDataHandler("vehicle:Siren", VehicleSiren);
            Events.AddDataHandler("vehicle:IndicatorLeft", IndicatorsLeft);
            Events.AddDataHandler("vehicle:IndicatorRight", IndicatorsRight);
            Events.AddDataHandler("vehicle:Doors", SetVehicleDoors);

            Events.AddDataHandler("vehicle:EngineHealth", SetVehicleEngineHealth);
            Events.AddDataHandler("vehicle:BodyHealth", SetVehicleBodyHealth);

            int leftbindid = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Left, true, IndicatorLeft);
            int rightbindid = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Right, true, IndicatorRight);
            Events.OnEntityStreamIn += OnEntityStreamIn;
            Events.Add("client:SetHandling", SetTune);
        }


        private void DamageVehicle(object[] args)
        {
            Dictionary<Vector3, float> damages = GetVehicleDeformation(RAGE.Elements.Player.LocalPlayer.Vehicle);
            RAGE.Elements.Player.LocalPlayer.Vehicle.SetDeformationFixed();
            SetVehicleDeformation(RAGE.Elements.Player.LocalPlayer.Vehicle, damages);
            /*
            Dictionary<Vector3, float> deformations = new Dictionary<Vector3, float>();
            for (float x = -1f; x <= 1f; x+=0.5f)
            {
                for (float y = -1f; y <= 1f; y+=0.5f)
                {
                    for (float z = -1f; z <= 1f; z+=0.5f)
                    {
                        Vector3 damages = RAGE.Elements.Player.LocalPlayer.Vehicle.GetDeformationAtPos(x, y, z);
                        deformations[new Vector3(x, y, z)] = damages.X*damages.Y;
                    }
                }
            }
            RAGE.Elements.Player.LocalPlayer.Vehicle.SetDeformationFixed();
            foreach (var item in deformations)
            {
                RAGE.Elements.Player.LocalPlayer.Vehicle.SetDamage(item.Key.X, item.Key.Y, item.Key.Z, item.Value*100f, 1f, true);
            }
            */
            /*
            
            Vector3 offset = new Vector3(Convert.ToSingle(args[0]), Convert.ToSingle(args[1]), Convert.ToSingle(args[2]));
            float damage = Convert.ToSingle(args[3]);
            float radius = Convert.ToSingle(args[4]);
            bool focusonmodel = Convert.ToBoolean(args[5]);

            */

            /*
            Chat.Output("DAMAGE APPLIED ON PLAYER VEHICLE");
            Chat.Output("BODY: " + RAGE.Elements.Player.LocalPlayer.Vehicle.GetBodyHealth() + " BODY2: " + RAGE.Elements.Player.LocalPlayer.Vehicle.GetBodyHealth2(0, 0, 0, 0, 0, 0) + " ENGINE: " + RAGE.Elements.Player.LocalPlayer.Vehicle.GetEngineHealth());
            */

        }

        public float getMaxElement(Vector3 v)
        {
            return Math.Max(v.X, v.Y);
        }

        public Dictionary<Vector3, float> GetVehicleDeformation(RAGE.Elements.Vehicle v)
        {
            Dictionary<Vector3, float> deformations = new Dictionary<Vector3, float>();
            List<Vector3> offsets = GetVehicleOffsets(v);
            foreach (var item in offsets)
            {
                
                float dmg = getMaxElement(v.GetDeformationAtPos(item.X, item.Y, item.Z) * 1000f) / 1000f;
                if (dmg > 0.03f)
                {
                    deformations[item] = dmg;
                }
            }

            foreach (var item in deformations)
            {
                Chat.Output(item.Key + " DMG: " + item.Value);
            }
            return deformations;
        }

        public void SetVehicleDeformation(RAGE.Elements.Vehicle v, Dictionary<Vector3, float> deformations)
        {

            float fDeformationDamageMult = v.GetHandlingFloat("fDeformationDamageMult");
            float damageMult = 100f;
            if (fDeformationDamageMult <= 0.55f)
            {
                damageMult = 1000f;
            }
            else if (fDeformationDamageMult <= 0.65f)
            {
                damageMult = 400f;
            }
            else if (fDeformationDamageMult <= 0.75f)
            {
                damageMult = 200f;
            }

            bool deform = true;
            int iteration = 0;
            foreach (var item in deformations)
            {
                while(deform && iteration < 500000)
                {
                    if ((getMaxElement(v.GetDeformationAtPos(item.Key.X, item.Key.Y, item.Key.Z) * 1000f) / 1000f) < item.Value)
                    {
                        v.SetDamage(item.Key.X * 2, item.Key.Y * 2, item.Key.Z * 2, item.Value * damageMult, 1000f, true);
                        deform = true;
                    }
                    else
                    {
                        deform = false;
                    }
                    iteration++;
                }

                
            }


        }

        public List<Vector3> GetVehicleOffsets(RAGE.Elements.Vehicle v)
        {
            Vector3 min = new Vector3(0f, 0f, 0f);
            Vector3 max = new Vector3(0f, 0f, 0f);
            RAGE.Game.Misc.GetModelDimensions(v.Model, min, max);

            float X = Convert.ToSingle(Math.Round((max.X - min.X) * 0.5, 2));
            float Y = Convert.ToSingle(Math.Round((max.Y - min.Y) * 0.5, 2));
            float Z = Convert.ToSingle(Math.Round((max.Z - min.Z) * 0.5, 2));
            float halfY = Convert.ToSingle(Math.Round(Y * 0.5, 2));

            List<Vector3> offsets = new List<Vector3>
            {
                new Vector3(-X, Y, 0f),
                new Vector3(-X, Y, Z),

                new Vector3(0f,Y, 0f),
                new Vector3(0f, Z, Z),

                new Vector3(-X, Y,  0.0f),
                new Vector3(-X, Y,  Z),

                new Vector3(0.0f, Y,  0.0f),
                new Vector3(0.0f, Y,  Z),

                new Vector3(X, Y,  0.0f),
                new Vector3(X, Y, Z),


                new Vector3(-X, halfY,  0.0f),
                new Vector3(-X, halfY, Z),

                new Vector3(0.0f, halfY,  0.0f),
                new Vector3(0.0f, halfY,  Z),

                new Vector3(X, halfY,  0.0f),
                new Vector3(X, halfY, Z),


                new Vector3(-X, 0.0f,  0.0f),
                new Vector3(-X, 0.0f,  Z),

                new Vector3(0.0f, 0.0f,  0.0f),
                new Vector3(0.0f, 0.0f,  Z),

                new Vector3(X, 0.0f,  0.0f),
                new Vector3(X, 0.0f,  Z),


                new Vector3(-X, -halfY,  0.0f),
                new Vector3(-X, -halfY, Z),

                new Vector3(0.0f, -halfY,  0.0f),
                new Vector3(0.0f, -halfY,  Z),

                new Vector3(X, -halfY,  0.0f),
                new Vector3(X, -halfY, Z),


                new Vector3(-X, -Y,  0.0f),
                new Vector3(-X, -Y, Z),

                new Vector3(0.0f, -Y,  0.0f),
                new Vector3(0.0f, -Y,  Z),

                new Vector3(X, -Y,  0.0f),
                new Vector3(X, -Y, Z)
            };

            return offsets;
        }

        private void SetVehicleBodyHealth(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Vehicle)
            {
                RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtRemote(entity.RemoteId);
                v.SetBodyHealth(Convert.ToSingle(arg));
            }
        }

        private void SetVehicleEngineHealth(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Vehicle)
            {
                RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtRemote(entity.RemoteId);
                v.SetEngineHealth(Convert.ToSingle(arg));
            }
        }

        private void ToggleSpeedCam(object[] args)
        {
            bool state = Convert.ToBoolean(args[0]);

            if (state)
            {
                Events.Tick += Tick2;
                //SpeedCam = new RAGE.Ui.HtmlWindow("package://frontend/radar-gun/radar.html");
                //SpeedCam.Active = true;
            }
            else
            {
                Events.Tick -= Tick2;
                //SpeedCam.Active = false;
                //SpeedCam.Destroy();
            }
        }
        private static RAGE.Elements.Entity GetEntityFromRaycast(Vector3 fromCoords, Vector3 toCoords, int ignoreEntity, int flags)
        {
            bool hit = false;
            Vector3 endCoords = new Vector3();
            Vector3 surfaceNormal = new Vector3();
            RAGE.Elements.Entity EntityHit = null;
            int materialHash = -1;
            int elementHitHandle = -1;
            RAGE.Elements.Entity entityHit = null;
            int ray = RAGE.Game.Shapetest.StartShapeTestCapsule(fromCoords.X, fromCoords.Y, fromCoords.Z, toCoords.X, toCoords.Y, toCoords.Z, 3f, flags, ignoreEntity, 0);

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
                if (entityType == 2)
                {
                    entityHit = RAGE.Elements.Entities.Vehicles.All.FirstOrDefault(x => x.Handle == elementHitHandle);
                }
            }

            return entityHit;
        }
        float FrontMaxSpeed = 0f;
        float BackMaxSpeed = 0f;
        RAGE.Elements.Vehicle LastVehFront = null;
        RAGE.Elements.Vehicle LastVehBack = null;
        private void Tick2(List<Events.TickNametagData> nametags)
        {
            Vector3 Forward = RAGE.Elements.Player.LocalPlayer.Vehicle.GetOffsetFromInWorldCoords(0f,16f,0f);
            Vector3 Backward = RAGE.Elements.Player.LocalPlayer.Vehicle.GetOffsetFromInWorldCoords(0f, -16f, 0f);

            RAGE.Elements.Entity fw = GetEntityFromRaycast(RAGE.Elements.Player.LocalPlayer.Vehicle.Position, Forward, RAGE.Elements.Player.LocalPlayer.Vehicle.Handle, 10);
            RAGE.Elements.Entity back = GetEntityFromRaycast(RAGE.Elements.Player.LocalPlayer.Vehicle.Position, Backward, RAGE.Elements.Player.LocalPlayer.Vehicle.Handle, 10);
            if (fw != null)
            {
                if (fw.Type == RAGE.Elements.Type.Vehicle)
                {
                    RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtRemote(fw.RemoteId);
                    if (v != LastVehFront)
                    {
                        RAGE.Game.Audio.PlaySoundFrontend(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
                        LastVehFront = v;
                        TopSpeed = 0f;
                    }
                    float speed = v.GetSpeed();
                    if (speed > FrontMaxSpeed)
                    {
                        FrontMaxSpeed = speed;
                    }
                    int kmh = Convert.ToInt32(FrontMaxSpeed * 3.6);
                    //frissítjük a frontendet
                    Chat.Output("FRONT: " + v.GetNumberPlateText() + " TOP SPEED: " + FrontMaxSpeed);
                }
            }

            if (back != null)
            {
                if (back.Type == RAGE.Elements.Type.Vehicle)
                {
                    RAGE.Elements.Vehicle v = RAGE.Elements.Entities.Vehicles.GetAtRemote(back.RemoteId);
                    if (v != LastVehBack)
                    {
                        RAGE.Game.Audio.PlaySoundFrontend(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
                        LastVehBack = v;
                        TopSpeed = 0f;
                    }
                    float speed = v.GetSpeed();
                    if (speed > BackMaxSpeed)
                    {
                        BackMaxSpeed = speed;
                    }
                    int kmh = Convert.ToInt32(BackMaxSpeed * 3.6);
                    //frissítjük a frontendet
                    Chat.Output("BACK: " + v.GetNumberPlateText() + " TOP SPEED: " + BackMaxSpeed);
                }
            }

        }

        private void SetVehicleDoors(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            //8 indexes tömb, megfeltetve az értékeknek
            throw new NotImplementedException();
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
            Dictionary<int, int> drawables = new Dictionary<int, int>();//drawable - lehetséges textúrák száma
            Dictionary<int, int> props = new Dictionary<int, int>();//drawable - lehetséges textúrák száma


            for (int i = 0; i <= 11; i++)
            {
                drawables[i] = RAGE.Elements.Player.LocalPlayer.GetNumberOfDrawableVariations(i);//lekérjük a drawable-k számát

                for (int j = 0; j < drawables[i]; j++)
                {
                    int textura = RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations(i, j);
                    if (textura <= 1)
                    {
                        RAGE.Elements.Player.LocalPlayer.SetComponentVariation(i, drawables[i], textura, 0);
                        RAGE.Game.Utils.Wait(250);
                        Chat.Output("COMPONENT: " + i + " DRAWABLE: " + drawables[i] + " TEXTURE: " + textura);
                    }
                }
                

            }
            /*
            Chat.Output("KALAP DRAWABLE NUMBER: " + );
            Chat.Output("SZEMÜVEG DRAWABLE NUMBER: " + RAGE.Elements.Player.LocalPlayer.GetNumberOfPropDrawableVariations(1));
            Chat.Output("FÜLBEVALÓ DRAWABLE NUMBER: " + RAGE.Elements.Player.LocalPlayer.GetNumberOfPropDrawableVariations(2));
            Chat.Output("ÓRA DRAWABLE NUMBER: " + RAGE.Elements.Player.LocalPlayer.GetNumberOfPropDrawableVariations(6));
            Chat.Output("KARKÖTŐ DRAWABLE NUMBER: " + RAGE.Elements.Player.LocalPlayer.GetNumberOfPropDrawableVariations(8));
            
            //9 - ARMOR - 26 -> 9 db textúra
            Chat.Output("JÓ ARMOR TEXTÚRA: " + RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations(9, 26));
            Chat.Output("ROSSZ (MODOLT - NEM LÉTEZŐ) ARMOR TEXTÚRA: " + RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations(9, 35));

            Chat.Output("JÓ HAJ TEXTÚRA: " + RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations(2, 5));
            Chat.Output("ROSSZ (LÉTEZŐ MODOLT) HAJ TEXTÚRA: " + RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations(2, 159));
            */
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

        private void SetTune(object[] args)
        {
            float torque = Convert.ToSingle(args[0]);
            float power = Convert.ToSingle(args[1]);
            bool drift = Convert.ToBoolean(args[2]);
            RAGE.Elements.Player.LocalPlayer.Vehicle.SetEngineTorqueMultiplier(torque);
            RAGE.Elements.Player.LocalPlayer.Vehicle.SetEnginePowerMultiplier(power);
            RAGE.Elements.Player.LocalPlayer.Vehicle.SetReduceGrip(drift);
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
                    int mph = Convert.ToInt32(TopSpeed * 2.236936); 
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

                if (v.GetSharedData("vehicle:EngineHealth") != null)
                {
                    float hp = (float)v.GetSharedData("vehicle:EngineHealth");
                    v.SetEngineHealth(hp);
                }

                if (v.GetSharedData("vehicle:BodyHealth") != null)
                {
                    float hp = (float)v.GetSharedData("vehicle:BodyHealth");
                    v.SetBodyHealth(hp);
                }

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
