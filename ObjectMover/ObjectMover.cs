using RAGE;
using RAGE.Elements;
using RAGE.Game;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Client.ObjectMover
{
    internal class ObjectMover : Events.Script
    {
        static HtmlWindow MoverCEF;
        int obj;
        string model;
        bool ShowX = true;
        bool ShowY = true;
        bool ShowZ = true;
        
        public ObjectMover()
        {

            Events.Add("client:PlaceObject", PlaceObject);
            Events.Add("client:MoveObject", MoveObject);
            Events.Add("client:SetObject", SetObject);
            Events.Add("client:RotateObject", RotateObject);
            Events.Add("client:CloseObjectEditor", CloseEditor);
            Events.Add("client:SaveObject", SaveObject);

            Events.Add("client:ObjectToGround", ObjectToGround);
            Events.Add("client:StreamServerObjects", StreamServerObjects);
        }

        private void SaveObject(object[] args)
        {
            Vector3 objCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);
            Vector3 objRots = RAGE.Game.Entity.GetEntityRotation(obj, 2);

            Events.CallRemote("server:PlaceObject", model, objCoords.X, objCoords.Y, objCoords.Z, objRots.X, objRots.Y, objRots.Z);
            RAGE.Game.Entity.DeleteEntity(ref obj);
            MoverCEF.Active = false;
            MoverCEF.Destroy();
            Events.Tick -= Tick;
        }

        private void CloseEditor(object[] args)
        {
            RAGE.Game.Entity.DeleteEntity(ref obj);
            MoverCEF.Active = false;
            MoverCEF.Destroy();
            Events.Tick -= Tick;
        }

        private void StreamServerObjects(object[] args)
        {
            foreach (var item in RAGE.Elements.Entities.Objects.All)
            {
                item.NotifyStreaming = true;
                item.StreamRange = 50f;
            }
        }

        private void ObjectToGround(object[] args)
        {
            if (LastUpdate + UpdateTime < DateTime.Now)
            {
                RAGE.Game.Object.PlaceObjectOnGroundProperly(obj);
                Vector3 newCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);

                Vector3 newRot = RAGE.Game.Entity.GetEntityRotation(obj, 2);
                MoverCEF.ExecuteJs($"UpdateObjectPos(\"{newCoords.X}\",\"{newCoords.Y}\",\"{newCoords.Z}\",\"{newRot.X}\",\"{newRot.Y}\",\"{newRot.Z}\")");

                LastUpdate = DateTime.Now;
            }

        }

        DateTime LastUpdate = DateTime.Now;
        TimeSpan UpdateTime = TimeSpan.FromMilliseconds(500);

        private void SetObject(object[] args)
        {
            string axis = Convert.ToString(args[0]);
            float value = Convert.ToSingle(args[1]);
            Vector3 objCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);
            Vector3 objRots = RAGE.Game.Entity.GetEntityRotation(obj, 2);

            switch (axis)
            {
                case "px":
                    RAGE.Game.Entity.SetEntityCoords(obj, value, objCoords.Y, objCoords.Z, true, true, true, false);
                    break;
                case "py":
                    RAGE.Game.Entity.SetEntityCoords(obj, objCoords.X, value, objCoords.Z, true, true, true, false);
                    break;
                case "pz":
                    RAGE.Game.Entity.SetEntityCoords(obj, objCoords.X, objCoords.Y, value, true, true, true, false);
                    break;
                case "rx":
                    RAGE.Game.Entity.SetEntityRotation(obj, value, objRots.Y, objRots.Z, 2, false);
                    break;
                case "ry":
                    RAGE.Game.Entity.SetEntityRotation(obj, objRots.X, value, objRots.Z, 2, false);
                    break;
                case "rz":
                    RAGE.Game.Entity.SetEntityRotation(obj, objRots.X, objRots.Y, value, 2, false);
                    break;
                default:
                    break;
            }
        }


        private void MoveObject(object[] args)
        {
            int rel = Convert.ToInt32(args[0]);
            float relative = Convert.ToSingle(rel);
            string axis = Convert.ToString(args[1]);
            Vector3 objCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);

            switch (axis)
            {
                case "X":
                    RAGE.Game.Entity.SetEntityCoords(obj, objCoords.X + (relative / 500f), objCoords.Y, objCoords.Z, true, true, true, false);
                    break;
                case "Y":
                    RAGE.Game.Entity.SetEntityCoords(obj, objCoords.X, objCoords.Y + (relative / 500f), objCoords.Z, true, true, true, false);
                    break;
                case "Z":
                    RAGE.Game.Entity.SetEntityCoords(obj, objCoords.X, objCoords.Y, objCoords.Z + (relative / 500f), true, true, true, false);
                    break;
                default:
                    break;
            }





            if (LastUpdate + UpdateTime < DateTime.Now)
            {
                Vector3 newCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);
                
                Vector3 newRot = RAGE.Game.Entity.GetEntityRotation(obj, 2);
                MoverCEF.ExecuteJs($"UpdateObjectPos(\"{newCoords.X}\",\"{newCoords.Y}\",\"{newCoords.Z}\",\"{newRot.X}\",\"{newRot.Y}\",\"{newRot.Z}\")");


                LastUpdate = DateTime.Now;
            }



            //RAGE.Elements.Entity moveObj = RAGE.Elements.Entities.Objects.GetAtHandle(obj);
        }


        private void RotateObject(object[] args)
        {
            int rel = Convert.ToInt32(args[0]);
            float relative = Convert.ToSingle(rel);
            string axis = Convert.ToString(args[1]);
            Vector3 objRots = RAGE.Game.Entity.GetEntityRotation(obj, 2);

            switch (axis)
            {
                case "X":
                    RAGE.Game.Entity.SetEntityRotation(obj, objRots.X + (relative / 25f), objRots.Y, objRots.Z, 2, false);
                    break;
                case "Y":
                    RAGE.Game.Entity.SetEntityRotation(obj, objRots.X, objRots.Y + (relative / 25f), objRots.Z, 2, false);
                    break;
                case "Z":
                    RAGE.Game.Entity.SetEntityRotation(obj, objRots.X, objRots.Y, objRots.Z + (relative / 25f), 2, false);
                    break;
                default:
                    break;
            }





            if (LastUpdate + UpdateTime < DateTime.Now)
            {
                Vector3 newCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);

                Vector3 newRot = RAGE.Game.Entity.GetEntityRotation(obj, 2);
                MoverCEF.ExecuteJs($"UpdateObjectPos(\"{newCoords.X}\",\"{newCoords.Y}\",\"{newCoords.Z}\",\"{newRot.X}\",\"{newRot.Y}\",\"{newRot.Z}\")");


                LastUpdate = DateTime.Now;
            }
        }

        private void PlaceObject(object[] args)
        {
            string objName = Convert.ToString(args[0]);
            RAGE.Game.Streaming.RequestModel(RAGE.Game.Misc.GetHashKey(objName));
            model = objName;

            obj = RAGE.Game.Object.CreateObject(RAGE.Game.Misc.GetHashKey(objName), RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y+1f, RAGE.Elements.Player.LocalPlayer.Position.Z, false, false, false);
            
            RAGE.Game.Object.PlaceObjectOnGroundProperly(obj);
            RAGE.Game.Entity.FreezeEntityPosition(obj, true);
            RAGE.Game.Entity.SetEntityCollision(obj, false, false);
            Vector3 objCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);
            
            int screenX = 1920;
            int screenY = 1080;
            RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
            float objCenterX = 0;
            float objCenterY = 0;



            RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(objCoords.X, objCoords.Y, objCoords.Z, ref objCenterX, ref objCenterY);


            MoverCEF = new RAGE.Ui.HtmlWindow("package://frontend/object/object.html");
            MoverCEF.Active = true;
            MoverCEF.ExecuteJs($"ShowMover()");

            Events.Tick += Tick;
            //RAGE.Game.Object.DeleteObject(ref obj);
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            Vector3 objCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);

            /*
            if (Vector3.Distance(RAGE.Elements.Player.LocalPlayer.Position, objCoords) > 25f)
            {
                //túl messze került, kidobjuk belőle
                Chat.Output("Túl messze kerültél az objecttől!");
                
                RAGE.Game.Entity.DeleteEntity(ref obj);
                MoverCEF.Active = false;
                MoverCEF.Destroy();
                Events.Tick -= Tick;
            }*/

            float objCenterX = 0;
            float objCenterY = 0;



            RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(objCoords.X, objCoords.Y, objCoords.Z, ref objCenterX, ref objCenterY);

            if (objCenterX > -1f && objCenterX < 1f && objCenterY > -1f && objCenterY < 1f)//Az object középpontja a képernyőn van, rajzolhatunk
            {
                int screenX = 1920;
                int screenY = 1080;
                RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                float objXposX = 0;
                float objXposY = 0;

                float objYposX = 0;
                float objYposY = 0;

                float objZposX = 0;
                float objZposY = 0;

                RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(objCoords.X + 1f, objCoords.Y, objCoords.Z, ref objXposX, ref objXposY);
                RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(objCoords.X, objCoords.Y + 1f, objCoords.Z, ref objYposX, ref objYposY);
                RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(objCoords.X, objCoords.Y, objCoords.Z + 1f, ref objZposX, ref objZposY);

                if (objXposX > -1f && objXposX < 1f && objXposY > -1f && objXposY < 1f)
                {
                    if (ShowX == false)
                    {
                        MoverCEF.ExecuteJs($"Show('X')");
                        ShowX = true;
                    }
                    MoverCEF.ExecuteJs($"UpdateX(\"{Convert.ToInt32(screenX * objCenterX)}\",\"{Convert.ToInt32(screenY * objCenterY)}\",\"{Convert.ToInt32(screenX * objXposX)}\",\"{Convert.ToInt32(screenY * objXposY)}\")");
                }
                else
                {
                    if (ShowX)
                    {
                        MoverCEF.ExecuteJs($"Hide('X')");
                        ShowX = false;
                    }
                }

                if (objYposX > -1f && objYposX < 1f && objYposY > -1f && objYposY < 1f)
                {
                    if (ShowY == false)
                    {
                        MoverCEF.ExecuteJs($"Show('Y')");
                        ShowY = true;
                    }
                    MoverCEF.ExecuteJs($"UpdateY(\"{Convert.ToInt32(screenX * objCenterX)}\",\"{Convert.ToInt32(screenY * objCenterY)}\",\"{Convert.ToInt32(screenX * objYposX)}\",\"{Convert.ToInt32(screenY * objYposY)}\")");
                }
                else
                {
                    if (ShowY)
                    {
                        MoverCEF.ExecuteJs($"Hide('Y')");
                        ShowY = false;
                    }
                }

                if (objZposX > -1f && objZposX < 1f && objZposY > -1f && objZposY < 1f)
                {
                    if (ShowZ == false)
                    {
                        MoverCEF.ExecuteJs($"Show('Z')");
                        ShowZ = true;
                    }
                    MoverCEF.ExecuteJs($"UpdateZ(\"{Convert.ToInt32(screenX * objCenterX)}\",\"{Convert.ToInt32(screenY * objCenterY)}\",\"{Convert.ToInt32(screenX * objZposX)}\",\"{Convert.ToInt32(screenY * objZposY)}\")");
                }
                else
                {
                    if (ShowZ)
                    {
                        MoverCEF.ExecuteJs($"Hide('Z')");
                        ShowZ = false;
                    }
                }
            }
            else
            {
                MoverCEF.ExecuteJs($"Hide('X')");
                ShowX = false;
                MoverCEF.ExecuteJs($"Hide('Y')");
                ShowY = false;
                MoverCEF.ExecuteJs($"Hide('Z')");
                ShowZ = false;
            }
        }
    }
}
