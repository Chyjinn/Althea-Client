using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using RAGE;
using RAGE.Elements;
using RAGE.Game;
using RAGE.NUI;
using RAGE.Ui;

namespace Client
{
    public class Main : Events.Script
    {
        int flyCam = -1;
        DateTime nextUpdate = DateTime.Now;
        public Main() 
        {
            Interior.EnableInteriorProp(166657, "V_Michael_bed_tidy");
            Interior.EnableInteriorProp(166657, "V_Michael_M_items");
            Interior.EnableInteriorProp(166657, "V_Michael_D_items");
            Interior.EnableInteriorProp(166657, "V_Michael_S_items");
            Interior.EnableInteriorProp(166657, "V_Michael_L_Items");
            Interior.RefreshInterior(166657);

            Binds.bindKeys();
            Events.Add("client:Fly", ToggleFly);
            
        }

        private void ToggleFly(object[] args)
        {
            bool flag = (bool)args[0];
            if (flag)
            {
                RAGE.Game.Player.SetPlayerInvincible(true);
                RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
                RAGE.Elements.Player.LocalPlayer.SetAlpha(0, true);
                
                Events.Tick += Fly;
            }
        }

        private Vector3 GetNormalizedVector(Vector3 vIn)
        {
            double magnitude = Math.Sqrt(vIn.X * vIn.X + vIn.Y * vIn.Y + vIn.Z * vIn.Z);
            Vector3 v = new Vector3();
            v.X = vIn.X / (float)magnitude;
            v.Y = vIn.Y / (float)magnitude;
            v.Z = vIn.Z / (float)magnitude;
            return v;
        }

        private float degToRad(double degrees)
        {
            return (float)(degrees * Math.PI) / 180;
        }

        private Vector3 RotationToDirection(Vector3 rot)
        {
            Vector3 ret = new Vector3();
            float z = degToRad(rot.Z);
            float x = degToRad(rot.X);
            float num = (float)Math.Abs(Math.Cos(x));

            ret.X = (float)-Math.Sin(z) * num;
            ret.Y = (float)Math.Cos(z) * num;
            ret.Z = (float)Math.Sin(x);
            return ret;
        }

        private Vector3 GetCrossProduct(Vector3 v1, Vector3 v2)
        {
            Vector3 crossProduct = new Vector3();
            crossProduct.X = v1.Y * v2.Z - v1.Z * v2.Y;
            crossProduct.Y = v1.Z * v2.X - v1.X * v2.Z;
            crossProduct.Z = v1.X * v2.Y - v1.Y * v2.X;
            return crossProduct;
        }

        float maxSpeed = 0.5f;
        float speed = 0;
        float zSpeedUp = 0;
        float zSpeedDown = 0;
        float zSpeed = 0;

        int[] disabledControls = new int[31]{ 30, // A & D
        31, // W & S
        21, // Left Shift
        36, // Left Ctrl
        22, // Space
        44, // Q
        38, // E
        71, // W - Vehicle
        72, // S - Vehicle
        59, // A & D - Vehicle
        60, // L Shift & L CTRL - Vehicle
        42, // D PAD Up || ]
        43, // D PAD Down || [
        85,
        86,
        15, // Mouse Wheel Up
        14, // Mouse Wheel Down
        228,
        229,
        172,
        173,
        37,
        44,
        178,
        244,
        220,
        221,
        218,
        219,
        16,
    17 };

        private void Fly(List<Events.TickNametagData> nametags)
        {
            if (flyCam == -1)
            {
                
                Cam.DestroyAllCams(true);
                Vector3 coords = RAGE.Elements.Player.LocalPlayer.Position;
                flyCam = Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", coords.X,coords.Y,coords.Z,0f,0f,358f,70.0f,true,2);
                Cam.SetCamActive(flyCam, true);
                Cam.RenderScriptCams(true, false, 0, true, false, 0);
                Cam.SetCamAffectsAiming(flyCam, false);
                Chat.Output("KAMERA LÉTREHOZVA" + coords.X + " " + coords.Y + " " + coords.Z);
            }

            if (Pad.IsDisabledControlPressed(0, 21))
            {
                maxSpeed = 1.2f;
            }
            else
            {
                maxSpeed = 0.4f;
            }


            for (int i = 0; i < disabledControls.Length; i++)
            {
                Pad.DisableControlAction(0, disabledControls[i], true);
            }


            if (Pad.IsDisabledControlPressed(0, 38))
            {
                zSpeed = maxSpeed/2;
            }
            else if(Pad.IsDisabledControlPressed(0, 44))
            {
                zSpeed = -maxSpeed/2;
            
            }
            else
            {
                zSpeed = 0.0f;
            }


            float rightAxisX = Pad.GetDisabledControlNormal(0, 220);
            float rightAxisY = Pad.GetDisabledControlNormal(0, 221);

            float LeftAxisX = Pad.GetDisabledControlNormal(0, 218);
            float LeftAxisY = Pad.GetDisabledControlNormal(0, 219);

            if (LeftAxisX == 0 && LeftAxisY == 0)
            {
                speed = 0;
            } else
            {
                if (speed < maxSpeed)
                {
                    speed += 0.2f * (speed + 0.01f);
                }
            }

            if (speed > maxSpeed)
            {
                speed -= 0.2f * (speed + 0.01f);
            }

            Vector3 upVector = new Vector3(0, 0, 1);
            Vector3 pos = Cam.GetCamCoord(flyCam);
            Vector3 rot = Cam.GetCamRot(flyCam,2);
            Vector3 rr = RotationToDirection(rot);
            Vector3 preRightVector = GetCrossProduct(GetNormalizedVector(rr), GetNormalizedVector(upVector));

            Vector3 movementVector = new Vector3();
            movementVector.X = rr.X * LeftAxisY * speed;
            movementVector.Y = rr.Y * LeftAxisY * speed;
            movementVector.Z = rr.Z * LeftAxisY * zSpeed;

            Vector3 rightVector = new Vector3();
            rightVector.X = preRightVector.X * LeftAxisX * speed;
            rightVector.Y = preRightVector.Y * LeftAxisX * speed;
            rightVector.Z = preRightVector.Z * LeftAxisX * speed;

            Vector3 newPos = new Vector3();
            newPos.X = pos.X - movementVector.X + rightVector.X;
            newPos.Y = pos.Y - movementVector.Y + rightVector.Y;
            newPos.Z = pos.Z - movementVector.Z + rightVector.Z + zSpeed;
            Cam.SetCamCoord(flyCam,newPos.X,newPos.Y,newPos.Z);
            //Vector3 GCamRot = Cam.GetGameplayCamRot(2);
            Cam.SetCamRot(flyCam, rot.X + rightAxisY * -5.0f, 0.0f, rot.Z + rightAxisX * -5.0f, 2);
            //Cam.SetCamRot(flyCam, GCamRot.X, GCamRot.Y, GCamRot.Z, 2); ;
            if (DateTime.Now > nextUpdate)
            {
                TimeSpan span = new TimeSpan(2500000);
                nextUpdate = DateTime.Now + span;
                Events.CallRemote("server:Fly",newPos.X, newPos.Y, newPos.Z);
            }
        }

        public void LoadIPL(object[] args)
        {
            string name = Convert.ToString(args[0]);
            RAGE.Game.Streaming.RequestIpl(name);
            RAGE.Chat.Output(RAGE.Game.Streaming.IsIplActive(name).ToString());
        }
    }
}
