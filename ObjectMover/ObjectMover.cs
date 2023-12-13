using RAGE;
using RAGE.Game;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Client.ObjectMover
{
    internal class ObjectMover : Events.Script
    {
        static HtmlWindow MoverCEF;
        int obj;
        bool ShowX = true;
        bool ShowY = true;
        bool ShowZ = true;
        
        public ObjectMover()
        {

            Events.Add("client:PlaceObject", PlaceObject);
            Events.Add("client:MoveObject", MoveObject);

            Events.Add("client:ObjectToGround", ObjectToGround);
        }

        private void ObjectToGround(object[] args)
        {
            if (LastUpdate + UpdateTime < DateTime.Now)
            {
                RAGE.Game.Object.PlaceObjectOnGroundProperly(obj);
                Vector3 newCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);

                Vector3 newRot = RAGE.Game.Entity.GetEntityRotation(obj, 2);
                MoverCEF.ExecuteJs($"UpdateObjectPos(\"{newCoords.X}\",\"{newCoords.Y}\",\"{newCoords.Z}\",\"{newRot.X}\",\"{newRot.Y}\",\"{newRot.Z}\")");
                if (Vector3.Distance(RAGE.Elements.Player.LocalPlayer.Position, newCoords) > 15f)
                {
                    //túl messze került, kidobjuk belőle
                    Chat.Output("Túl messze kerültél az objecttől!");
                    RAGE.Game.Entity.DeleteEntity(ref obj);
                    MoverCEF.Active = false;
                    MoverCEF.Destroy();
                }
                LastUpdate = DateTime.Now;
            }

        }

        DateTime LastUpdate = DateTime.Now;
        TimeSpan UpdateTime = TimeSpan.FromMilliseconds(500);

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
                if (Vector3.Distance(RAGE.Elements.Player.LocalPlayer.Position, newCoords) > 15f)
                {
                    //túl messze került, kidobjuk belőle
                    Chat.Output("Túl messze kerültél az objecttől!");
                    RAGE.Game.Entity.DeleteEntity(ref obj);
                    MoverCEF.Active = false;
                    MoverCEF.Destroy();
                }

                LastUpdate = DateTime.Now;
            }



            //RAGE.Elements.Entity moveObj = RAGE.Elements.Entities.Objects.GetAtHandle(obj);
          

        }

        private void PlaceObject(object[] args)
        {

            string objName = Convert.ToString(args[0]);
  
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
