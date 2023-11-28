using Client.Inventory;
using Microsoft.VisualBasic;
using RAGE;
using RAGE.Elements;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Linq;

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

        static HtmlWindow NametagCEF;
        public static HtmlWindow ChatCEF;
        HtmlWindow VersionCEF;

        public NameTag() {

            RAGE.Nametags.Enabled = false;
            SetNameTagEnabled(true);
            
            Events.Add("client:Chat", ShowChat);
            Events.Add("client:BindKeys", BindKeys);
            Events.Add("client:NametagTest", NametagTest);

            RAGE.Chat.Show(false);
            ChatCEF = new HtmlWindow("package://frontend/chat/index.html");
            ChatCEF.Active = false;

            NametagCEF = new HtmlWindow("package://frontend/nametag/nametag.html");
            NametagCEF.Active = true;

            VersionCEF = new HtmlWindow("package://frontend/version/ver.html");
            VersionCEF.Active = true;
            ChatCEF.MarkAsChat();
            Events.OnEntityStreamIn += StreamIn;
            Events.OnEntityStreamOut += StreamOut;
            Events.Tick += Render;
        }

        private void StreamOut(Entity entity)
        {
            if (entity.Type == RAGE.Elements.Type.Player)//ha játékos
            {
                Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                if (streamedPlayers.Contains(p))
                {
                    streamedPlayers.Remove(p);
                }
            }
        }

        private void StreamIn(Entity entity)
        {
            if (entity.Type == RAGE.Elements.Type.Player)//ha játékos
            {
                Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                streamedPlayers.Add(p);
            }
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

        static RAGE.Elements.Entity GetEntityFromRaycast(Vector3 fromCoords, Vector3 toCoords, int ignoreEntity, int flags)
        {
            bool hit = false;
            Vector3 endCoords = new Vector3();
            Vector3 surfaceNormal = new Vector3();
            int materialHash = -1;
            int elementHitHandle = -1;
            RAGE.Elements.Entity entityHit = null;
            ignoreEntity = RAGE.Elements.Player.LocalPlayer.Handle;
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
                    entityHit = RAGE.Elements.Entities.Players.All.FirstOrDefault(x => x.Handle == elementHitHandle);
                }
                else if (entityType == 2)
                {
                    entityHit = RAGE.Elements.Entities.Vehicles.All.FirstOrDefault(x => x.Handle == elementHitHandle);
                }
                else if (entityType == 3)
                {
                    entityHit = RAGE.Elements.Entities.Objects.All.FirstOrDefault(x => x.Handle == elementHitHandle);
                }
            }

            return entityHit;
        }


        public static void SetNameTagEnabled(bool status)
        {
            if (status)
            {
                Events.Tick += Render;
                //RAGE.Game.Graphics.RequestStreamedTextureDict("3dtextures", true);
            }
            else
            {
                Events.Tick -= Render;
            }
        }


        static List<Player> streamedPlayers = new List<Player>();

        static List<Ped> pedtest = new List<Ped>();
        private static async void NametagTest(object[] args)
        {
            for (int i = -5; i < +5; i++)
            {
                for (int j = -5; j < +5; j++)
                {
                    Ped p = new Ped(RAGE.Game.Misc.GetHashKey("mp_m_freemode_01"), new Vector3(RAGE.Elements.Player.LocalPlayer.Position.X + i, RAGE.Elements.Player.LocalPlayer.Position.Y + j, RAGE.Elements.Player.LocalPlayer.Position.Z), 0f, 0);
                    pedtest.Add(p);
                }
            }
        }

        private static async void Render(List<Events.TickNametagData> nametags)
        {
            NametagCEF.ExecuteJs($"clearNametags()");
            Vector3 Cam = RAGE.Game.Cam.GetGameplayCamCoord();
            foreach (var item in streamedPlayers)
            {
                
                RAGE.Elements.Entity e = GetEntityFromRaycast(Cam, item.Position, item.Vehicle.Handle, -1);
                if (e != null)//sikeres raycast
                {
                    if (e.Type == RAGE.Elements.Type.Player)//ped
                    {
                        int screenX = 1920;
                        int screenY = 1080;
                        float x = 0;
                        float y = 0;
                        //RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                        RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                        RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(item.Position.X, item.Position.Y, item.Position.Z + 1f, ref x, ref y);

                        float distance = Vector3.Distance(Cam, item.Position);//megnézzük a távolságot

                        if (distance < 10f)
                        {
                            float scale = distance / 10f;
                            if (scale < 0.3f) scale = 0.5f;

                            if (x > -1f && x < 1f && y > -1f && y < 1f)
                            {
                                //Chat.Output("render X: " + Convert.ToInt32(screenX * x) + " Y: " + Convert.ToInt32(screenY * y) + " SCALE: " + scale);
                                NametagCEF.ExecuteJs($"renderNametag(\"{item.Id}\",\"{item.Name}\",\"{Convert.ToInt32(screenX * x)}\",\"{Convert.ToInt32(screenY * y)}\",\"{scale}\")");
                            }
                        }
                    }


                    //DrawNametags();
                    /*
                    foreach (var item in streamedPlayers)
                    {
                        int screenX = 1920;
                        int screenY = 1080;
                        float x = 1920;
                        float y = 1080;
                        //RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                        RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                        RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(item.Position.X, item.Position.Y, item.Position.Z+1f, ref x, ref y);

                        float distance = Vector3.Distance(RAGE.Game.Cam.GetGameplayCamCoord(), item.Position);//megnézzük a távolságot
                        float scale = 0.6f;
                        if (distance < 10f)
                        {
                            scale = 1f / distance;
                        }
                        Chat.Output(distance + " scale: " + scale);
                        if (x > -1f && x < 1f && y > -1f && y < 1f)
                        {
                            NametagCEF.ExecuteJs($"renderNametag(\"{item.RemoteId}\",\"{item.Name}\",\"{Convert.ToInt32(screenX * x)}\",\"{Convert.ToInt32(screenY * y)}\",\"{scale}\")");
                        }
                    }
                    */


                    /*
                    foreach (var item in pedtest)
                    {
                        RAGE.Elements.Entity e = GetEntityFromRaycast(Cam, item.Position, 0, -1);
                        if (e != null)//sikeres raycast
                        {
                            if (e.Type == RAGE.Elements.Type.Ped)//ped
                            {
                                int screenX = 1920;
                                int screenY = 1080;
                                float x = 1920;
                                float y = 1080;
                                //RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                                RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                                RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(item.Position.X, item.Position.Y, item.Position.Z + 1f, ref x, ref y);

                                float distance = Vector3.Distance(Cam, item.Position);//megnézzük a távolságot

                                if (distance < 10f)
                                {
                                    float scale = distance / 10f;
                                    if (scale < 0.5f) scale = 0.5f;

                                    if (x > -1f && x < 1f && y > -1f && y < 1f)
                                    {
                                        NametagCEF.ExecuteJs($"renderNametag(\"{item.Model}\",\"{item.Id}\",\"{Convert.ToInt32(screenX * x)}\",\"{Convert.ToInt32(screenY * y)}\",\"{scale}\")");
                                    }
                                }
                            }
                    */
                    //Chat.Output(distance + " scale: " + scale);

                }
            }
            

            // InventoryCEF.ExecuteJs($"addItemToSlot(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\")");

        }
    }
}
