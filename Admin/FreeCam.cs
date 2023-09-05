using RAGE.Game;
using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Admin
{
    internal class FreeCam : Events.Script
    {
        public FreeCam()
        {
            Events.Add("client:Fly", ToggleFly);
            
        }


        int flyCam = -1;
        DateTime nextUpdate = DateTime.Now;

        float speed = 0.4f;
        float slowSpeed = 0.2f;
        float normalSpeed = 0.4f;
        float fastSpeed = 1.2f;
        float zSpeed = 0;

        private void ToggleFly(object[] args)
        {
            bool flag = Convert.ToBoolean(RAGE.Elements.Player.LocalPlayer.GetSharedData("player:Flying"));
            //RAGE.Discord.Update("abbbb", "bcaaca");
            if (flag)
            {
                RAGE.Game.Player.SetPlayerInvincible(true);
                //RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
                //RAGE.Elements.Player.LocalPlayer.SetAlpha(0, true);

                Events.Tick += Fly;

                Cam.DestroyAllCams(true);
                Vector3 coords = RAGE.Elements.Player.LocalPlayer.Position;
                flyCam = Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", coords.X, coords.Y, coords.Z, 0f, 0f, 358f, 70.0f, true, 2);
                Cam.SetCamActive(flyCam, true);
                Cam.RenderScriptCams(true, false, 0, true, false, 0);
                Cam.SetCamAffectsAiming(flyCam, false);
            }
            else
            {
                RAGE.Game.Player.SetPlayerInvincible(false);
                //RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
                //RAGE.Elements.Player.LocalPlayer.SetAlpha(255, true);
                Cam.RenderScriptCams(false, false, 0, true, false, 0);
                Cam.DestroyAllCams(true);
                Events.Tick -= Fly;
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



        private void Fly(List<Events.TickNametagData> nametags)
        {
            if (Pad.IsDisabledControlPressed(0, 21))//SHIFT
            {
                speed = fastSpeed;
            }
            else if (Pad.IsDisabledControlPressed(0, 36))//BAL CTRL
            {
                speed = slowSpeed;
            }
            else
            {
                speed = normalSpeed;
            }

            if (Pad.IsDisabledControlPressed(0, 38))
            {
                zSpeed = speed / 2;
            }
            else if (Pad.IsDisabledControlPressed(0, 44))
            {
                zSpeed = -speed / 2;

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
            }

            Vector3 upVector = new Vector3(0, 0, 1);
            Vector3 pos = Cam.GetCamCoord(flyCam);
            Vector3 rot = Cam.GetCamRot(flyCam, 2);
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
            Cam.SetCamCoord(flyCam, newPos.X, newPos.Y, newPos.Z);
            Cam.SetCamRot(flyCam, rot.X + rightAxisY * -5.0f, 0.0f, rot.Z + rightAxisX * -5.0f, 2);

            //Ui.UnlockMinimapAngle();
            

            //Chat.Output(rot.Z.ToString() + " - " + Convert.ToInt32(rot.Z + rightAxisX * -5.0f).ToString());
            RAGE.Elements.Player.LocalPlayer.SetRotation(0f, 0f, rot.Z, 2, true);
            RAGE.Game.Ui.LockMinimapAngle(Convert.ToInt32(rot.Z));
            RAGE.Elements.Player.LocalPlayer.Position = newPos;
            
            if (DateTime.Now > nextUpdate)
            {
                TimeSpan span = new TimeSpan(5000000);
                nextUpdate = DateTime.Now + span;
                
                Events.CallRemote("server:Fly", newPos.X, newPos.Y, newPos.Z);
            }
        }
    }
}
