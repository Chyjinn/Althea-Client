using RAGE;
using RAGE.Ui;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using Client.Characters;
using System.Linq;
using static System.Collections.Specialized.BitVector32;
using RAGE.Game;
using System.Security.Principal;

namespace Client.Inventory
{
    public class Item
    {
        public uint DBID { get; set; }
        public uint OwnerID { get; set; }
        public int OwnerType { get; set; }
        public uint ItemID { get; set; }
        public string ItemValue { get; set; }//itemvalue, json
        public int ItemAmount { get; set; }
        public bool InUse { get; set; }
        public bool Duty { get; set; }
        public int Priority { get; set; }
        public Item(uint dbid, uint ownerid, int ownertype, uint itemid, string itemvalue, int itemamount, bool inuse, bool duty, int priority)
        {
            DBID = dbid;
            OwnerID = ownerid;
            OwnerType = ownertype;
            ItemID = itemid;
            ItemValue = itemvalue;
            ItemAmount = itemamount;
            Duty = duty;
            Priority = priority;
            InUse = inuse;
        }

        public bool IsContainer()
        {
            bool state = false;
            if (ItemID == 11)
            {
                state = true;
            }
            return state;
        }

    }


    public class Entry
    {
        public uint ItemID { get; set; }//itemid
        public string Name { get; set; }//item neve
        public string Description { get; set; }//leírás, ha van megjelenítjük
        public int ItemType { get; set; }//felhasználás kezeléséhez kell majd, pl Weapon akkor úgy kezeljük
        public string ItemImage { get; set; }//lehet local, pl. src/img.png, vagy url
        public uint ItemWeight { get; set; }
        public string Object { get; set; }
        public bool Stackable { get; set; }
        public Entry(uint id, string name, string desc, int type, uint weight, string itemimage, string obj, bool stack)
        {
            ItemID = id;
            Name = name;
            Description = desc;
            ItemType = type;
            ItemWeight = weight;
            ItemImage = itemimage;
            Object = obj;
            Stackable = stack;
        }

    }

    public class Items : Events.Script
    {
        static HtmlWindow InventoryCEF;
        static Entry[] itemList;
        public Items() { 
            InventoryCEF = new RAGE.Ui.HtmlWindow("package://frontend/inventory/inventory.html");
            InventoryCEF.Active = false;
            Events.Add("client:InventoryFromServer", ReloadInventory);
            Events.Add("client:ContainerFromServer", ReloadContainer);

            Events.Add("client:ItemListFromServer", ReloadItemList);

            Events.Add("client:MoveItemToClothing", MoveItemToClothing);
            
            Events.Add("client:AddItemToClothing", AddItemToClothing);

            Events.Add("client:RemoveItem", RemoveItem);//mindenhonnan töröl
            Events.Add("client:AddItemToInventory", AddItemToInventory);
            Events.Add("client:AddItemToContainer", AddItemToContainer);
            Events.Add("client:CloseContainer", CloseContainer);

            Events.Add("client:SwapItem", SwapItem);
            Events.Add("client:MoveItem", MoveItem);

            Events.Add("client:RefreshInventoryPreview", RefreshInventoryPreview);


            Events.Add("client:UseItem", UseItem);
            Events.Add("client:DropItem", DropItem);

            Events.Add("client:GetItemPriorities", GetItemPriorities);


            Events.Add("client:ChangeItemInUse", ChangeItemInUse);
            Events.Add("client:SetContainerName", SetContainerName);
            Events.Add("client:ItemUseToCEF", ItemUseToCEF);
            Events.Add("client:TakeItemPictures", TakeItemPictures);
            Events.Add("client:TakeIDPicture", TakeIDPicture);
            Events.Add("client:GiveItemMenu", OpenGiveMenu);
            Events.Add("client:GiveItem", GiveItemToServer);

            Events.Add("client:SetInventoryWeights", SetInventoryWeights);
            Events.Add("client:SetContainerWeights", SetContainerWeights);


            Events.OnPlayerQuit += Quit;

            Events.Add("client:IDbase64toServer", Base64ToServer);
            Events.OnClickWithRaycast += WorldClickToContainer;
            ToggleInventory(false);
            RAGE.Game.Streaming.RequestNamedPtfxAsset("scr_bike_adversary");
            ObjectGlows();
        }

        private void SetInventoryWeights(object[] args)
        {
            uint weight = Convert.ToUInt32(args[0]);
            uint capacity = Convert.ToUInt32(args[1]);
            InventoryCEF.ExecuteJs($"setInventoryWeights(\"{weight}\",\"{capacity}\")");
        }

        private void SetContainerWeights(object[] args)
        {
            uint weight = Convert.ToUInt32(args[0]);
            uint capacity = Convert.ToUInt32(args[1]);
            InventoryCEF.ExecuteJs($"setContainerWeights(\"{weight}\",\"{capacity}\")");
        }

        Dictionary<int, int> HotKeys = new Dictionary<int, int>
        {
            //HOTKEY, ITEM_DBID az item használat meghívásához
            //átadásnál kell valami check hogy hotkey-en van-e csak akkor maradjon hotkeyen ha a saját inventorydban marad
            { 0, 0 },
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 4 },
            { 5, 5 },
            { 6, 6 },
            { 7, 7 },
            { 8, 8 },
            { 9, 9 },
        };

        private void ObjectGlows()
        {
            List<MapObject> all = RAGE.Elements.Entities.Objects.All;
            foreach (var item in all)
            {
                if (item.GetSharedData("object:ID") != null)//van object ID, tehát lerakott item
                {
                    item.SetCollision(false, false);
                    /*
                    RAGE.Game.Graphics.UseParticleFxAssetNextCall("scr_bike_adversary");
                    Chat.Output("ENTITY ID: " + item.GetData<uint>("object:ID").ToString());
                    int particle = RAGE.Game.Graphics.StartParticleFxLoopedOnEntity("scr_adversary_ped_light_good", item.Handle, 0f, 0f, 0f, 0f, 0f, 0f, 1f, false, false, false);
                    RAGE.Game.Graphics.SetParticleFxLoopedColour(particle, 1f, 0f, 0f, false);
                    */
                }
            }
        }

        private void GiveItemToServer(object[] args)
        {
            uint item_dbid = Convert.ToUInt32(args[0]); 
            int playerid = Convert.ToInt32(args[1]);
            uint amount = Convert.ToUInt32(args[2]);
            Events.CallRemote("server:GiveItemToPlayer", item_dbid, playerid, amount);
        }

        private void OpenGiveMenu(object[] args)
        {
            var players = RAGE.Elements.Entities.Players.Streamed.ToList();
            var player = RAGE.Elements.Player.LocalPlayer;
            List<RAGE.Elements.Player> closePlayers = new List<RAGE.Elements.Player>();
            foreach (var item in players)
            {
                float dist = Vector3.Distance(item.Position, player.Position);
                if (dist < 5f && item != player)
                {
                    closePlayers.Add(item);
                }
            }

            InventoryCEF.ExecuteJs($"closeGiveMenu()");
            
            foreach (var item in closePlayers)
            {
                InventoryCEF.ExecuteJs($"AddNameToGivemenu(\"{item.Name}\",\"{item.RemoteId}\")");
            }
            InventoryCEF.ExecuteJs($"openGiveMenu()");
        }

        private void Base64ToServer(object[] args)
        {
            Chat.Output(args[0].ToString());
        }

        private void TakeIDPicture(object[] args)
        {
            RAGE.Game.Ui.DisplayRadar(false);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(6, 1, 0, 0);//cipő beállítása a magassarkúk miatt
            RAGE.Task.Run(() =>
            {

                InfrontCamera(0.65f, 13f);
                Vector3 toLook = RAGE.Game.Cam.GetCamCoord(camera);
                
                RAGE.Elements.Player.LocalPlayer.TaskLookAtCoord(toLook.X,toLook.Y,toLook.Z,-1,0,0);
                RAGE.Task.Run(() =>
                {
                    RAGE.Input.TakeScreenshot("idpicture.png", 1, 100, 0);
                }, 200);
            }, 200);
        }

        int greenscreen;
        private void Quit(RAGE.Elements.Player player)
        {
            if (player == RAGE.Elements.Player.LocalPlayer)
            {
                RAGE.Game.Object.DeleteObject(ref greenscreen);
                RAGE.Game.Graphics.StopParticleFxLooped(particle, false);
            }
        }

        public async void InfrontCamera(float heightoffset, float fov)
        {
            DeleteCamera();
            await RAGE.Task.WaitAsync(500);
            Vector3 pos = RAGE.Elements.Player.LocalPlayer.Position;

            float radians = -RAGE.Elements.Player.LocalPlayer.GetHeading() * (float)Math.PI / 180f;
            float nx = pos.X + (2f * (float)Math.Sin(radians));
            float ny = pos.Y + (2f * (float)Math.Cos(radians));

            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", nx, ny, pos.Z + heightoffset, 0f, 0f, 0f, fov, true, 2);
            RAGE.Game.Cam.PointCamAtCoord(camera, pos.X, pos.Y, pos.Z + heightoffset);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, true, 200, true, false, 0);
        }

        int component = -1;
        private async void TakeItemPictures(object[] args)
        {
            RAGE.Game.Ui.DisplayRadar(false);
            greenscreen = RAGE.Game.Object.CreateObject(RAGE.Util.Joaat.Hash("prop_ld_greenscreen_01"), 228.55f, -992f, -100.3f, false, false, false);
            RAGE.Task.Run(() =>
            {
                
                    
                    RAGE.Task.Run(async () =>
                    {
                        //InfrontCamera(Convert.ToSingle(args[0]), Convert.ToSingle(args[1]));
                        
                        for (int i = 1; i <= 11; i++)//összes komponensen végigmegyünk
                        {
                            RAGE.Task.WaitAsync(1000);
                            string gender = "semmi";
                            if (RAGE.Elements.Player.LocalPlayer.Model == RAGE.Game.Misc.GetHashKey("mp_f_freemode_01"))//nő
                            {
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(1, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(2, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(3, 10, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(4, 13, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(5, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(6, 12, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(7, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(8, 2, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(9, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(10, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(11, 82, 0, 0);
                                gender = "female";
                            }
                            else if (RAGE.Elements.Player.LocalPlayer.Model == RAGE.Game.Misc.GetHashKey("mp_m_freemode_01"))//férfi
                            {
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(1, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(2, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(3, 3, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(4, 11, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(5, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(6, 13, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(7, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(8, 15, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(9, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(10, 0, 0, 0);
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(11, 15, 0, 0);
                                gender = "male";
                            }
                            int numofdrawables = RAGE.Elements.Player.LocalPlayer.GetNumberOfDrawableVariations(i);
                            switch (i)
                            {
                                case 1://maszk
                                    component = -1;
                                    InfrontCamera(0.675f, 13f);
                                    break;
                                case 2:
                                    continue;
                                case 3:
                                    continue;
                                case 4://nadrág
                                    component = -1;
                                    InfrontCamera(-0.45f, 35f);
                                    break;
                                case 5://táska
                                    InfrontCamera(0.2f, 20f);
                                    component = 5;
                                    break;
                                case 6://cipő
                                    InfrontCamera(-0.85f, 15f);
                                    component = -1;
                                    break;
                                case 7://kiegészítő (mint felső)
                                    component = -1;
                                    InfrontCamera(0.2f, 25f);
                                    break;
                                case 8:
                                    continue;
                                case 9:
                                    continue;
                                case 10:
                                    continue;
                                case 11://felső
                                    component = -1;
                                    InfrontCamera(0.2f, 35f);//female
                                    break;
                            }
                            await RAGE.Task.WaitAsync(2000);
                            Events.Tick += Tick;
                            for (int j = 0; j < numofdrawables; j++)//összes drawable
                            {
                                
                                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(i, j, 0, 0);
                                await RAGE.Task.WaitAsync(200);
                                RAGE.Input.TakeScreenshot(gender+"_"+i+"_"+j+".png", 1, 100, 0);
                                //Chat.Output("Screenshot létrehozva! " + gender + "_" + i + "_" + j + ".png");
                                await RAGE.Task.WaitAsync(400);
                            }
                            Events.Tick -= Tick;
                            RAGE.Elements.Player.LocalPlayer.SetHeading(0f);
                        }
                        

                    }, 1000);
                

            }, 2000);
            //férfi felsők: 0.225 20
            //férfi nadrág: -0.45 30
            //férfi cipő: -1 15
            //férfi kalap, maszk 0.675 11
            //férfi szemüveg 0.675 6
            //férfi fülbevaló 0.675 6



            //FÉRFI TORSO: 3
            //NŐI TORSO: 8
            //FÉRFI LEG: 11
            //NŐI LEG: 13
            //FÉRFI CIPŐ: 33
            //NŐI CIPŐ: 34

            //SetCamera(228.35f, -986.3f, -99f, 0f, 0f, 180f, 50f);
            /*
            for (int i = 1; i <= 1; i++)//végigmegyünk az összes ruha itemen
            {
                int numofdrawables = RAGE.Elements.Player.LocalPlayer.GetNumberOfDrawableVariations(i);
                //itt beállítjuk a megfelelő kamerát és a többi ruhadarabot a képekhez

                
                for (int j = 0; j < numofdrawables; j++)//végigmegyünk az összes drawable-n
                {
                    int numoftextures = RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations(i, j);
                    for (int y = 0; y < numoftextures; y++)//végigmegyünk az adott drawable összes textúráján
                    {
                        RAGE.Elements.Player.LocalPlayer.SetComponentVariation(i, j, y, 0);
                        //lefényképezzük, várakoztatunk picit
                        RAGE.Game.Utils.Wait(250);
                    }
                    
                }
            }
            */
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            RAGE.Elements.Player.LocalPlayer.ClearTasksImmediately();
            if (component == 5)
            {
                RAGE.Elements.Player.LocalPlayer.SetHeading(0f);
            }
            else if(component == 14)
            {
                RAGE.Elements.Player.LocalPlayer.SetHeading(90f);
            }
            else
            {
                RAGE.Elements.Player.LocalPlayer.SetHeading(180f);
            }
            
        }

        int camera = 2;

        public void SetCamera(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float fov)
        {
            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", posX, posY, posZ, rotX, rotY, rotZ, fov, true, 2);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
        }

        public void DeleteCamera()
        {
            RAGE.Game.Cam.SetCamActive(camera, false);
            RAGE.Game.Cam.DestroyCam(camera, true);
            RAGE.Game.Cam.RenderScriptCams(false, true, 200, true, false, 0);
        }


        private void CloseContainer(object[] args)
        {
            InventoryCEF.ExecuteJs($"HideContainer()");
        }

        private void AddItemToContainer(object[] args)
        {
            Item item = RAGE.Util.Json.Deserialize<Item>(args[0].ToString());
            InventoryCEF.ExecuteJs($"addItemToContainer(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{Convert.ToString(item.InUse)}\",\"{GetItemTypeById(item.ItemID)}\")");
            RAGE.Task.Run(() =>
            {
                Events.CallRemote("server:RequestInventoryWeight");
                Events.CallRemote("server:RequestContainerWeight");
            }, 250);
        }

        private void ReloadContainer(object[] args)
        {
            List<Item> container = RAGE.Util.Json.Deserialize<List<Item>>(args[0].ToString());
            ContainerToCEF(container);
        }

        private void ContainerToCEF(List<Item> inv)
        {
            InventoryCEF.ExecuteJs($"ClearContainer()");
            foreach (var item in inv)
            {
                InventoryCEF.ExecuteJs($"addItemToContainer(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{Convert.ToString(item.InUse)}\",\"{GetItemTypeById(item.ItemID)}\")");
            }
            RAGE.Task.Run(() =>
            {
                Events.CallRemote("server:RequestContainerWeight");
            }, 250);
            InventoryCEF.ExecuteJs($"ShowContainer()");

            ToggleInventory(true);
            
        }

        private void SetContainerName(object[] args)
        {
            string containername = Convert.ToString(args[0]);
            InventoryCEF.ExecuteJs($"setContainerName(\"{containername}\")");//tároló neve
        }


        private void GetItemPriorities(object[] args)
        {
            string json = Convert.ToString(args[0]);
            
            Dictionary<uint, int> priorities = RAGE.Util.Json.Deserialize<Dictionary<uint, int>>(json);

            foreach (var item in priorities)
            {
                InventoryCEF.ExecuteJs($"setItemPrio(\"{item.Key}\",\"{item.Value}\")");//DB_ID és priority-t átküldjük minden létező itemre
            }
        }

        private void ChangeItemInUse(object[] args)
        {
            uint dbid = Convert.ToUInt32(args[0]);
            bool state = Convert.ToBoolean(args[1]);
            InventoryCEF.ExecuteJs($"setItemUse(\"{dbid}\",\"{state}\")");
        }
        RAGE.Elements.Vehicle lastVehicle = null;
        int particle;
        private void WorldClickToContainer(int x, int y, bool up, bool right, float relativeX, float relativeY, Vector3 worldPos, int entityHandle)
        {
            if (!up && !InventoryCEF.Active)//jobb klikket lenyomta és zárva van az inventory
            {
                RAGE.Elements.Entity e = GetEntityFromRaycast(RAGE.Game.Cam.GetGameplayCamCoord(), worldPos, 0, -1);

                if (e != null)//entity-re klikkeltünk
                {
                    if (right)//jobb klikk -> inventory megnyitás
                    {
                        if (e.Type == RAGE.Elements.Type.Vehicle && RAGE.Elements.Player.LocalPlayer.Vehicle == null)//járműre klikkeltünk és nem ülünk járműben
                        {
                            //megnyitjuk a jármű inventory-ját -> szerver oldali kérés
                            Vector3 playerpos = RAGE.Elements.Player.LocalPlayer.Position;
                            Vector3 vehiclepos = e.Position;
                            float distance = RAGE.Game.Misc.GetDistanceBetweenCoords(playerpos.X, playerpos.Y, playerpos.Z, vehiclepos.X, vehiclepos.Y, vehiclepos.Z, true);
                            if (distance < 3f)
                            {
                                Events.CallRemote("server:OpenVehicleTrunk", e.RemoteId);
                                lastVehicle = RAGE.Elements.Entities.Vehicles.GetAt(e.Id);
                            }
                        }
                        else if (e.Type == RAGE.Elements.Type.Vehicle && RAGE.Elements.Player.LocalPlayer.Vehicle != null)
                        {
                            if (e == RAGE.Elements.Player.LocalPlayer.Vehicle)
                            {
                                Events.CallRemote("server:OpenVehicleGloveBox");
                                lastVehicle = RAGE.Elements.Entities.Vehicles.GetAt(e.Id);
                            }
                        }
                        else if (e.Type == RAGE.Elements.Type.Object)
                        {
                            if (e.GetSharedData("object:ID") != null)//van object ID, tehát lerakott item
                            {
                                if (Vector3.Distance(RAGE.Elements.Player.LocalPlayer.Position, e.Position) < 2.5f)
                                {
                                    uint item_id = Convert.ToUInt32(e.GetSharedData("object:ID"));
                                    //eldobott object megnyitását meghívni
                                    Events.CallRemote("server:OpenGroundItem", item_id);
                                }


                                /*
                                //RAGE.Game.Graphics.SetParticleFxLoopedScale(particle, 0f);
                                RAGE.Game.Graphics.StopParticleFxLooped(particle, false);
                                MapObject obj = RAGE.Elements.Entities.Objects.GetAt(e.Id);
                                RAGE.Game.Graphics.UseParticleFxAssetNextCall("scr_bike_adversary");

                                particle = RAGE.Game.Graphics.StartParticleFxLoopedOnEntity("scr_adversary_ped_light_good", obj.Handle, 0f, 0f, 0.5f, 0f, 0f, 0f, 0.1f, false, false, false);
                                RAGE.Game.Graphics.SetParticleFxLoopedColour(particle, 0.3f, 0f, 0f, false);
                                //RAGE.Game.Graphics.SetParticleFxLoopedRange(particle, 5f);
                                */
                            }
                        }
                    }
                    else//bal klikk -> eldobott item felvétel
                    {
                        if (e.Type == RAGE.Elements.Type.Object)
                        {
                            if (e.GetSharedData("object:ID") != null)//van object ID, tehát lerakott item
                            {
                                if (Vector3.Distance(RAGE.Elements.Player.LocalPlayer.Position, e.Position) < 3f)
                                {
                                    uint item_id = Convert.ToUInt32(e.GetSharedData("object:ID"));
                                    Events.CallRemote("server:PickUpItem", item_id);
                                }


                                /*
                                //RAGE.Game.Graphics.SetParticleFxLoopedScale(particle, 0f);
                                RAGE.Game.Graphics.StopParticleFxLooped(particle, false);
                                MapObject obj = RAGE.Elements.Entities.Objects.GetAt(e.Id);
                                RAGE.Game.Graphics.UseParticleFxAssetNextCall("scr_bike_adversary");

                                particle = RAGE.Game.Graphics.StartParticleFxLoopedOnEntity("scr_adversary_ped_light_good", obj.Handle, 0f, 0f, 0.5f, 0f, 0f, 0f, 0.1f, false, false, false);
                                RAGE.Game.Graphics.SetParticleFxLoopedColour(particle, 0.3f, 0f, 0f, false);
                                //RAGE.Game.Graphics.SetParticleFxLoopedRange(particle, 5f);
                                */
                            }
                        }
                    }
                }
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

        private void MoveItem(object[] args)
        {
            uint item_dbid = Convert.ToUInt32(args[0]);
            uint target_inv = Convert.ToUInt32(args[1]);
            Events.CallRemote("server:MoveItem", item_dbid, target_inv);
        }

        private void SwapItem(object[] args)
        {
            uint item1_dbid = Convert.ToUInt32(args[0]);
            uint item2_dbid = Convert.ToUInt32(args[1]);
            Events.CallRemote("server:SwapItem", item1_dbid, item2_dbid);
        }

        private void AddItemToClothing(object[] args)
        {
            Item item = RAGE.Util.Json.Deserialize<Item>(Convert.ToString(args[0]));

            InventoryCEF.ExecuteJs($"addItemToSlot(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{GetItemTypeById(item.ItemID)}\")");
            //InventoryCEF.ExecuteJs($"addItemToSlot(1,1,\"Kesztyű\",\"\",500,50,\"https://pngimg.com/d/mma_gloves_PNG25.png\",5,0)");
            //addItemToSlot(1,1,"Kesztyű","",500,50,"https://pngimg.com/d/mma_gloves_PNG25.png",5,0)
            RAGE.Task.Run(() =>
            {
                Events.CallRemote("server:RequestInventoryWeight");
                Events.CallRemote("server:RequestContainerWeight");
            }, 500);
        }

        private void RemoveItem(object[] args)
        {
            InventoryCEF.ExecuteJs($"RemoveItem(\"{Convert.ToUInt32(args[0])}\")");//DB_ID alapján törölje
            RAGE.Task.Run(() =>
            {
                Events.CallRemote("server:RequestInventoryWeight");
                Events.CallRemote("server:RequestContainerWeight");
            }, 250);
        }

        private void RemoveItem(uint db_id)
        {
            InventoryCEF.ExecuteJs($"RemoveItem(\"{db_id}\")");//DB_ID alapján törölje
        }

        private void MoveItemToClothing(object[] args)
        {
            uint db_id = Convert.ToUInt32(args[0]);
            int target_id = Convert.ToInt32(args[1]);
            Events.CallRemote("server:MoveItemToClothing", db_id,target_id);
        }

        private void ItemUseToCEF(object[] args)
        {
            int section = Convert.ToInt32(args[0]);
            int slot = Convert.ToInt32(args[1]);
            int state = Convert.ToInt32(args[2]);
            InventoryCEF.ExecuteJs($"itemActive(\"{section}\",\"{slot}\",\"{state}\")");
        }

        private void UseItem(object[] args)
        {
            uint target_item_dbid = Convert.ToUInt32(args[0]);
            Events.CallRemote("server:UseItem", target_item_dbid);
        }

        private void DropItem(object[] args)
        {
            uint target_item_dbid = Convert.ToUInt32(args[0]);
            uint target_item_itemid = Convert.ToUInt32(args[1]);
            //float groundZ = -1000f;
            //RAGE.Game.Misc.GetGroundZFor3dCoord(RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y, RAGE.Elements.Player.LocalPlayer.Position.Z, ref groundZ, false);

            int obj = RAGE.Game.Object.CreateObject(RAGE.Util.Joaat.Hash(GetItemObjectById(target_item_itemid)), RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y, RAGE.Elements.Player.LocalPlayer.Position.Z, false, false, false);
            


            RAGE.Game.Object.PlaceObjectOnGroundProperly(obj);
            Vector3 objCoords = RAGE.Game.Entity.GetEntityCoords(obj, true);

            if (target_item_itemid >= 31 && target_item_itemid <= 111)//ha fegyver akkor elforgatjuk oldalra
            {
                float groundZ = 0f;
                Vector3 currentRot = RAGE.Game.Entity.GetEntityRotation(obj, 2);
                RAGE.Game.Entity.SetEntityRotation(obj, currentRot.X + 90f, currentRot.Y + 90f, 0f, 2, false);
                RAGE.Game.Misc.GetGroundZFor3dCoord(objCoords.X, objCoords.Y, objCoords.Z, ref groundZ, false);
                objCoords.Z = groundZ;
            }

            
            Vector3 objRot = RAGE.Game.Entity.GetEntityRotation(obj, 2);

            Events.CallRemote("server:DropItem", target_item_dbid, objCoords.Z, objRot.X,objRot.Y,objRot.Z);
            RAGE.Game.Object.DeleteObject(ref obj);
        }

        static int hashClone = -1;
        static DateTime timeout = DateTime.Now;
        static TimeSpan span = TimeSpan.FromMilliseconds(500);
        public static void ToggleInventory()
        {
            if(DateTime.Now > timeout+span)
            {
                if (!InventoryCEF.Active && !RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat)
                {
                    float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();

                    hashClone = RAGE.Elements.Player.LocalPlayer.Clone(heading, true, true);
                    RAGE.Game.Graphics.TransitionToBlurred(300);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 240f, true, true, false, false);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);
                    RAGE.Task.Run(() =>
                    {
                        RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);                        
                        RAGE.Game.Ui.SetFrontendActive(true);
                        RAGE.Game.Ui.ActivateFrontendMenu(RAGE.Game.Misc.GetHashKey("FE_MENU_VERSION_RAGEBEAST"), true, -1);
                        
                        RAGE.Task.Run(() =>
                        {
                            RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
                            RAGE.Game.Invoker.Invoke(0x3CA6050692BC61B0, true);
                            RAGE.Game.Invoker.Invoke(0xECF128344E9FF9F1, true);
                            RAGE.Game.Invoker.Invoke(0x98215325A695E78A, false);
                            InventoryCEF.Active = true;
                            RAGE.Ui.Cursor.ShowCursor(true, true);
                            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 265f, true, true, false, false);

                            //RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                            //RAGE.Game.Entity.DeleteEntity(ref hashClone);
                            
                            Events.Tick += DisablePauseMenu;

                        }, 100);

                    }, 50);

                }
                else
                {
                    InventoryCEF.ExecuteJs($"HideContainer()");
                    RAGE.Game.Graphics.TransitionFromBlurred(300);
                   
                    RAGE.Game.Ui.SetFrontendActive(false);
                    
                    RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                    RAGE.Game.Entity.DeleteEntity(ref hashClone);
                    //RAGE.Game.Entity.DeleteEntity(ref hashClone);

                    
                    Events.Tick -= DisablePauseMenu;
                    InventoryCEF.Active = false;
                    RAGE.Ui.Cursor.ShowCursor(false, false);
                    Events.CallRemote("server:ClosedContainer");
                }
                timeout = DateTime.Now;
            }
        }

        public static void ToggleInventory(bool state)
        {
                if (state)
                {
                    if (!InventoryCEF.Active)
                    {
                    float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();

                    hashClone = RAGE.Elements.Player.LocalPlayer.Clone(heading, true, true);
                    RAGE.Game.Graphics.TransitionToBlurred(300);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 240f, true, true, false, false);
                    RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);
                    RAGE.Task.Run(() =>
                    {
                        RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);
                        RAGE.Game.Ui.SetFrontendActive(true);
                        RAGE.Game.Ui.ActivateFrontendMenu(RAGE.Game.Misc.GetHashKey("FE_MENU_VERSION_RAGEBEAST"), true, -1);

                        RAGE.Task.Run(() =>
                        {
                            RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
                            RAGE.Game.Invoker.Invoke(0x3CA6050692BC61B0, true);
                            RAGE.Game.Invoker.Invoke(0xECF128344E9FF9F1, true);
                            RAGE.Game.Invoker.Invoke(0x98215325A695E78A, false);
                            InventoryCEF.Active = true;
                            RAGE.Ui.Cursor.ShowCursor(true, true);
                            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 265f, true, true, false, false);
                            
                            //RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                            //RAGE.Game.Entity.DeleteEntity(ref hashClone);

                            Events.Tick += DisablePauseMenu;

                        }, 100);

                    }, 50);
                }
                }
                else
                {
                    if (InventoryCEF.Active)
                    {
                        InventoryCEF.ExecuteJs($"HideContainer()");

                        RAGE.Game.Graphics.TransitionFromBlurred(300);
                   
                        RAGE.Game.Ui.SetFrontendActive(false);
                    
                        RAGE.Game.Entity.SetPedAsNoLongerNeeded(ref hashClone);
                        RAGE.Game.Entity.DeleteEntity(ref hashClone);
                        //RAGE.Game.Entity.DeleteEntity(ref hashClone);
                        
                        Events.Tick -= DisablePauseMenu;
                        InventoryCEF.Active = false;
                        RAGE.Ui.Cursor.ShowCursor(false, false);
                        Events.CallRemote("server:ClosedContainer");
                }
                }
            
        }


        private static void DisablePauseMenu(List<Events.TickNametagData> nametags)
        {
            RAGE.Game.Pad.DisableControlAction(32, 200, true);
        }

        private void RefreshInventoryPreview()
        {
            float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();

            hashClone = RAGE.Elements.Player.LocalPlayer.Clone(heading, true, true);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 240f, true, true, false, false);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);
            RAGE.Task.Run(() =>
            {
                RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
            }, 250);
            
        }

        private void RefreshInventoryPreview(object[] args)
        {
            float heading = RAGE.Elements.Player.LocalPlayer.GetHeading();

            hashClone = RAGE.Elements.Player.LocalPlayer.Clone(heading, true, true);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityCoords, hashClone, 2170f, 715f, 240f, true, true, false, false);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetEntityVisible, hashClone, false, false);
            RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.FreezeEntityPosition, hashClone, true);
            RAGE.Task.Run(() =>
            {
                RAGE.Game.Ui.GivePedToPauseMenu(hashClone, 1);
            }, 250);
        }

        public string GetItemPicture(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemImage;
                }
            }
            return "";
        }

        private void ReloadItemList(object[] args)
        {
            itemList = RAGE.Util.Json.Deserialize<Entry[]>(args[0].ToString());
        }

        private void ClearInventory(object[] args)
        {
            InventoryCEF.ExecuteJs($"ClearInventory()");
        }

        public static void ClearInventory()
        {
            InventoryCEF.ExecuteJs($"ClearInventory()");
        }

        public void ReloadInventory(object[] args)
        {
            InventoryCEF.ExecuteJs($"ClearInventory()");
            List<Item> inventory = RAGE.Util.Json.Deserialize<List<Item>>(args[0].ToString());

            //Chat.Output("OWNER: " + inventory[0].OwnerID.ToString());
            InventoryToCEF(inventory);
            //megkaptuk szervertől az inventory-t, át kell küldeni CEF-re.
        }

        private void AddItemToInventory(object[] args)
        {
            Item item = RAGE.Util.Json.Deserialize<Item>(args[0].ToString());
            InventoryCEF.ExecuteJs($"addItemToInventory(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{Convert.ToString(item.InUse)}\",\"{GetItemTypeById(item.ItemID)}\")");
            RAGE.Task.Run(() =>
            {
                Events.CallRemote("server:RequestInventoryWeight");
                Events.CallRemote("server:RequestContainerWeight");
            }, 250);
        }

        public static void HideTooltip()
        {
            InventoryCEF.ExecuteJs($"hideToolTip()");//eltüntetni a tooltipet
        }

        private void InventoryToCEF(List<Item> inv)
        {
            foreach (var item in inv)
            {
                InventoryCEF.ExecuteJs($"addItemToInventory(\"{item.DBID}\",\"{item.ItemID}\",\"{GetItemNameById(item.ItemID)}\",\"{GetItemDescriptionById(item.ItemID)}\",\"{GetItemWeightById(item.ItemID)}\",\"{item.ItemAmount}\",\"{GetItemPicture(item.ItemID)}\",\"{item.Priority}\",\"{Convert.ToString(item.InUse)}\",\"{GetItemTypeById(item.ItemID)}\")");
            }
            InventoryCEF.ExecuteJs($"setInventoryName(\"{RAGE.Elements.Player.LocalPlayer.Name}\")");//karakternév
            Events.CallRemote("server:SetWornClothing");
            RAGE.Task.Run(() =>
            {
                Events.CallRemote("server:RequestInventoryWeight");
            }, 250);
        }


        public static int GetItemType(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemType;
                }
            }
            return 0;
        }





        public static string GetItemNameById(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.Name;
                }
            }
            return "Nem létező item.";
        }

        public static string GetItemObjectById(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.Object;
                }
            }
            return "-1";
        }



        public static int GetItemTypeById(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemType;
                }
            }
            return -1;
        }

        public static string GetItemDescriptionById(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.Description;
                }
            }
            return "";
        }

        public static uint GetItemWeightById(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemWeight;
                }
            }
            return 0;
        }


        //CEF.Active = true;
    }
}
