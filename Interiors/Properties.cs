using RAGE;
using RAGE.Elements;
using RAGE.Game;
using System;
using System.Collections.Generic;

namespace Client.Interiors
{
    public class Property
    {
        public uint ID { get; set; }
        public byte PropertyType { get; set; }
        public string Name { get; set; }
        public uint OwnerType { get; set; }
        public uint OwnerID { get; set; }
        public string OwnerName { get; set; }
        public uint Postal { get; set; }
        public string StreetName { get; set; }
        public uint StreetNumber { get; set; }
        public Vector3 EntrancePos { get; set; }
        public float EntranceHeading { get; set; }
        public uint EntranceDimension { get; set; }
        public Vector3 ExitPos { get; set; }
        public float ExitHeading { get; set; }
        public uint ExitDimension { get; set; }
        public string IPL { get; set; }
        public bool Locked { get; set; }
        public int Price { get; set; }
        public Property(uint id, byte proptype, string name, uint ownertype, uint ownerid, Vector3 entrancepos, float entranceheading, uint entrancedim, Vector3 exitpos, float exitheading, uint exitdim, string ipl, bool locked, int price, uint postal, string streetname, uint streetnumber)
        {
                ID = id;
                PropertyType = proptype;
                Name = name;
                OwnerType = ownertype;
                OwnerID = ownerid;
                EntrancePos = entrancepos;
                EntranceHeading = entranceheading;
                EntranceDimension = entrancedim;
                ExitPos = exitpos;
                ExitHeading = exitheading;
                ExitDimension = exitdim;
                IPL = ipl;
                Locked = locked;
                Price = price;
                Postal = postal;
                StreetName = streetname;
                StreetNumber = streetnumber;
        }
    }


    class ClientProperty : Property
    {
        public DisplayMarker EntranceMarker { get; set; } = null;
        public DisplayMarker ExitMarker { get; set; } = null;
        public Colshape Entrance { get; set; } = null;
        public Colshape Exit { get; set; } = null;
        public ClientProperty(uint id, byte proptype, string name, uint ownertype, uint ownerid, Vector3 entrancepos, float entranceheading, uint entrancedim, Vector3 exitpos, float exitheading, uint exitdim, string ipl, bool locked, int price, uint postal, string streetname, uint streetnumber) : base(id, proptype, name, ownertype, ownerid, entrancepos, entranceheading, entrancedim, exitpos, exitheading, exitdim, ipl, locked, price, postal, streetname, streetnumber)
        {
            Entrance = new RAGE.Elements.TubeColshape(entrancepos, 2f, 3f, entrancedim);
            Exit = new RAGE.Elements.TubeColshape(exitpos, 2f, 3f, exitdim);
            Entrance.SetData("entrance:ID", id);
            Exit.SetData("exit:ID", id);
        }

        public void SetEntranceMarker(DisplayMarker marker)
        {
            EntranceMarker = marker;
        }

        public void SetExitMarker(DisplayMarker marker)
        {
            ExitMarker = marker;
        }
    }

    public class DisplayMarker
    {
        public int Type { get; set; }
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; }

        public DisplayMarker(int type, Vector3 position, float rotation, float scale, byte red, byte green, byte blue, byte alpha)
        {
            Type = type;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }
    }

        internal class Properties : Events.Script
    {

        List<ClientProperty> Props = new List<ClientProperty>();
        List<DisplayMarker> DisplayMarkers = new List<DisplayMarker>();
        static uint PropID = 0;

        public Properties()
        {
            Events.Add("client:ReloadProperties", ReloadProperties);
            Events.Add("client:ModifyProperty", ModifyPropertyByID);
            Events.Add("client:SetMarkerType", SetMarkerType);
            Events.Add("client:CreateInterior", CreateInterior);
            Events.Add("client:RequestIPL", RequestIPL);
            Events.Add("client:RemoveIPL", RemoveIPL);
            Events.Add("client:GetGroundZ", GetGroundZ);

            Events.OnPlayerEnterColshape += EnterColShape;
            Events.OnPlayerExitColshape += ExitColShape;

            Events.Tick += Tick;
        }

        private void GetGroundZ(object[] args)
        {
            float groundZ = 0f;
            Vector3 Coords = RAGE.Elements.Player.LocalPlayer.Position;
            RAGE.Game.Misc.GetGroundZFor3dCoord(Coords.X, Coords.Y, Coords.Z, ref groundZ, false);
            Chat.Output("A föld pozíciója: " + groundZ);
        }

        private void RemoveIPL(object[] args)
        {
            string name = Convert.ToString(args[0]);
            RAGE.Game.Streaming.RemoveIpl(name);
            RAGE.Chat.Output(name + " IPL státusz:" + RAGE.Game.Streaming.IsIplActive(name).ToString());
        }

        public void RequestIPL(object[] args)
        {
            string name = Convert.ToString(args[0]);
            RAGE.Game.Streaming.RequestIpl(name);
            RAGE.Chat.Output(name + " IPL státusz:" + RAGE.Game.Streaming.IsIplActive(name).ToString());
        }


        public ClientProperty GetPropertyByID(uint id)
        {
            foreach (var item in Props)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }
            return null;
        }

        public void ModifyPropertyByID(object[] args)
        {
            Property pr = RAGE.Util.Json.Deserialize<Property>(args[0].ToString());//megkapjuk a json alakját a frissített property-nek
            ClientProperty p = GetPropertyByID(pr.ID);//megszerezzük a jelenlegi property-t, ez lehet NULL is egyébként ha nem létezik
            if(p == null)//nem létezik még a property kliens oldalon tehát hozzá kell adjuk
            {
                p = new ClientProperty(pr.ID, pr.PropertyType, pr.Name, pr.OwnerType, pr.OwnerID, pr.EntrancePos, pr.EntranceHeading, pr.EntranceDimension, pr.ExitPos, pr.ExitHeading, pr.ExitDimension, pr.IPL, pr.Locked, pr.Price, pr.Postal, pr.StreetName, pr.StreetNumber);
                DisplayMarker enterMarker = new DisplayMarker(20, p.EntrancePos, 180f, 0.4f, 254, 117, 27, 75);
                DisplayMarker exitMarker = new DisplayMarker(20, p.ExitPos, 180f, 0.4f, 254, 117, 27, 75);
                p.SetEntranceMarker(enterMarker);
                p.SetExitMarker(exitMarker);
                Props.Add(p);
                Chat.Output("új interior hozzáadva!");
            }
            else//létezik már a property tehát csak törölnünk kell a colshape-ket és újra létrehozni
            {
                p.Entrance.Destroy();
                p.Exit.Destroy();
                p = new ClientProperty(pr.ID, pr.PropertyType, pr.Name, pr.OwnerType, pr.OwnerID, pr.EntrancePos, pr.EntranceHeading, pr.EntranceDimension, pr.ExitPos, pr.ExitHeading, pr.ExitDimension, pr.IPL, pr.Locked, pr.Price, pr.Postal, pr.StreetName, pr.StreetNumber);
                
                //ezt majd átírni egy külön függvénybe hogy könnyen lehessen majd dinamikusan kezelni, hiszen állítható lesz
                DisplayMarker enterMarker = new DisplayMarker(20, p.EntrancePos, 180f, 0.4f, 254, 117, 27, 75);
                DisplayMarker exitMarker = new DisplayMarker(20, p.ExitPos, 180f, 0.4f, 254, 117, 27, 75);
                p.SetEntranceMarker(enterMarker);
                p.SetExitMarker(exitMarker);
                Chat.Output("Meglévő interior szerkesztve!");
            }


        }


        static DateTime PropertyEntryTime = DateTime.Now;
        static TimeSpan PropertyCooldown = TimeSpan.FromSeconds(3);
        public static void EnterProperty()
        {
            if (PropID != 0)//egy valid épületbe szeretne belépni
            {
                if (PropertyEntryTime+PropertyCooldown < DateTime.Now)
                {
                    Chat.Output("Belépés az interiorba" + PropID);
                    Events.CallRemote("server:EnterProperty", PropID);
                    PropertyEntryTime = DateTime.Now;
                }
                else
                {
                    Chat.Output("Várj egy kicsit");
                }
            }
        }
        public static void ExitProperty()
        {
            if (PropID != 0)//egy valid épületbe szeretne belépni
            {
                if (PropertyEntryTime + PropertyCooldown < DateTime.Now)
                {
                    Chat.Output("Kilépés az interiorból" + PropID);
                    Events.CallRemote("server:ExitProperty", PropID);
                    PropertyEntryTime = DateTime.Now;
                }
                else
                {
                    Chat.Output("Várj egy kicsit");
                }
            }
        }
        private void EnterColShape(Colshape c, Events.CancelEventArgs cancel)
        {
            if (c.HasData("entrance:ID"))
            {
                uint id = c.GetData<uint>("entrance:ID");
                Property p = GetPropertyByID(id);
                PropID = id;
                Binds.Binds.bindEnterPropety();
                //információ megjelenítése
                Chat.Output("Belépéshez használd az [E] gombot. Tulajdonos: " + p.OwnerName + " Utca: " + p.StreetName + " Házszám: " + p.StreetNumber);
            }
            else if (c.HasData("exit:ID"))
            {
                uint id = c.GetData<uint>("exit:ID");
                Property p = GetPropertyByID(id);
                PropID = id;
                Binds.Binds.bindExitProperty();
                Chat.Output("Kilépéshez használd az [E] gombot. Tulajdonos: " + p.OwnerName + " Utca: " + p.StreetName + " Házszám: " + p.StreetNumber);
            }
        }

        private void ExitColShape(Colshape c, Events.CancelEventArgs cancel)
        {
            PropID = 0;
            Binds.Binds.unbindProperty();
        }

        private void ReloadProperties(object[] args)
        {
            foreach (var item in Props)
            {
                item.Entrance.Destroy();
                item.Exit.Destroy();
            }

            List<Property> pr = RAGE.Util.Json.Deserialize<List<Property>>(args[0].ToString());
            Props.Clear();
            foreach (var item in pr)
            {
                ClientProperty clientProp = new ClientProperty(item.ID, item.PropertyType, item.Name, item.OwnerType, item.OwnerID, item.EntrancePos, item.EntranceHeading, item.EntranceDimension, item.ExitPos, item.ExitHeading, item.ExitDimension, item.IPL, item.Locked, item.Price, item.Postal, item.StreetName, item.StreetNumber);

                DisplayMarker enterMarker = new DisplayMarker(20, clientProp.EntrancePos, 180f, 0.4f, 254, 117, 27, 75);
                DisplayMarker exitMarker = new DisplayMarker(20, clientProp.ExitPos, 180f, 0.4f, 254, 117, 27, 75);

                clientProp.SetEntranceMarker(enterMarker);
                clientProp.SetExitMarker(exitMarker);

                Props.Add(clientProp);
            }
        }

        DateTime LastUpdate = DateTime.Now;
        TimeSpan UpdateTime = TimeSpan.FromSeconds(1);

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (LastUpdate + UpdateTime < DateTime.Now)
            {
                DisplayMarkers.Clear();
                foreach (var item in Props)
                {
                    if (RAGE.Elements.Player.LocalPlayer.Dimension == item.EntranceDimension)
                    {
                        if (Vector3.Distance(RAGE.Elements.Player.LocalPlayer.Position, item.EntrancePos) < 7f)
                        {
                            DisplayMarkers.Add(item.EntranceMarker);
                        }
                    }
                    else if(RAGE.Elements.Player.LocalPlayer.Dimension == item.ExitDimension)
                    {
                        if (Vector3.Distance(RAGE.Elements.Player.LocalPlayer.Position, item.ExitPos) < 7f)
                        {
                            DisplayMarkers.Add(item.ExitMarker);
                        }
                    }
                }
                LastUpdate = DateTime.Now;
            }

            foreach (var item in DisplayMarkers)
            {
                if (item.Type != -1)
                {
                    RAGE.Game.Graphics.DrawMarker(item.Type, item.Position.X, item.Position.Y, item.Position.Z, 0f, 0f, 0f, item.Rotation, 0f, 0f, item.Scale, item.Scale, item.Scale, item.Red, item.Green, item.Blue, item.Alpha, false, true, 2, false, null, null, false);
                }
            }
        }

        private void SetMarkerType(object[] args)
        {
            int markerType = Convert.ToInt32(args[0]);
            float rotation = 0f;
            float scale = 1f;
            byte opacity = 75;
            float offsetZ = 0f;
            switch (markerType)
            {
                case 0:
                    rotation = 0f;
                    scale = 0.8f;
                    offsetZ = 1f;
                    opacity = 50;
                    break;
                case 1:
                    rotation = 0f;
                    scale = 1f;
                    offsetZ = 0f;
                    opacity = 75;
                    break;
                case 20:
                    rotation = 180f;
                    scale = 0.4f;
                    offsetZ = 1f;
                    opacity = 75;
                    break;
                case 25:
                    rotation = 0f;
                    scale = 1f;
                    opacity = 125;
                    offsetZ = 0.05f;
                    break;
                default:
                    rotation = 0f;
                    scale = 1f;
                    opacity = 75;
                    offsetZ = 0f;
                    break;
            }
            foreach (var item in Props)
            {
                Vector3 enterPos = new Vector3(item.EntrancePos.X, item.EntrancePos.Y, item.EntrancePos.Z + offsetZ);
                Vector3 exitPos = new Vector3(item.ExitPos.X, item.ExitPos.Y, item.ExitPos.Z + offsetZ);

                DisplayMarker enterMarker = new DisplayMarker(markerType, enterPos, rotation, scale, 254, 117, 27, opacity);
                DisplayMarker exitMarker = new DisplayMarker(markerType, exitPos, rotation, scale, 254, 117, 27, opacity);

                item.SetEntranceMarker(enterMarker);
                item.SetExitMarker(exitMarker);
            }

            //átállítjuk a lastupdate-t hogy egyből meg is jelenjen a változtatás
            LastUpdate = DateTime.Now - TimeSpan.FromSeconds(30);
        }



        public void CreateInterior(object[] args)//lekérjük a GROUNDZ magasságát és visszaadjuk feldolgozásra a szervernek
        {
            float groundZ = 0f;
            string name = Convert.ToString(args[0]);
            Vector3 Coords = new Vector3(Convert.ToSingle(args[1]), Convert.ToSingle(args[2]), Convert.ToSingle(args[3]));
            uint inti_id = Convert.ToUInt32(args[4]);
            byte prop_type = Convert.ToByte(args[5]);
            int price = Convert.ToInt32(args[6]);

            RAGE.Game.Misc.GetGroundZFor3dCoord(Coords.X, Coords.Y, Coords.Z, ref groundZ, false);

            var tempStreet = 0;
            var tempCrossing = 0;
            string Street = "";

            Pathfind.GetStreetNameAtCoord(Coords.X, Coords.Y, Coords.Z, ref tempStreet, ref tempCrossing);

            if (tempStreet != 0)
            {
                Street = Ui.GetStreetNameFromHashKey((uint)tempStreet);
            }


            Events.CallRemote("server:CreateInterior", inti_id, prop_type, name, groundZ, Street, price);
        }

    }
}
