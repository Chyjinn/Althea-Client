using System.Collections.Generic;
using System.Drawing;
using RAGE;
using RAGE.Game;
using RAGE.Ui;

namespace RageBibopClient
{
    class PLD : Events.Script
    {
        HtmlWindow CEF = null;
        string Street = null;
        string Crossing = null;
        string Zone = null;
        string Direction = null;

        public PLD()
        {
            //CEF = new HtmlWindow("package://PLD//index.html");
            //Events.Tick += Update;
        }

        public void Update(List<Events.TickNametagData> tick)
        {
            /*
            var position = RAGE.Elements.Player.LocalPlayer.Position;
            var tempStreet = 0;
            var tempCrossing = 0;
            var tempZone = "";
            float tempDirection = 0;
            Street = "";
            Crossing = "";
            Zone = "";
            Direction = "-";

            Pathfind.GetStreetNameAtCoord(position.X, position.Y, position.Z, ref tempStreet, ref tempCrossing);
            tempZone = RAGE.Game.Zone.GetNameOfZone(position.X, position.Y, position.Z);
            tempDirection = RAGE.Elements.Player.LocalPlayer.GetHeading();

            if (tempCrossing != 0)
            {
                Crossing = "/ " + Ui.GetStreetNameFromHashKey((uint)tempCrossing);
            }
            if (tempStreet != 0)
            {
                Street = Ui.GetStreetNameFromHashKey((uint)tempStreet);
            }
            if (tempZone != "")
            {
                Zone = Ui.GetLabelText(tempZone);
            }
            if (tempDirection != 0)
            {
                if(tempDirection < 45 || tempDirection > 315)
                {
                    Direction = "N";
                }
                if (tempDirection > 45 && tempDirection < 135)
                {
                    Direction = "W";
                }
                if (tempDirection > 135 && tempDirection < 225)
                {
                    Direction = "S";
                }
                if (tempDirection > 225 && tempDirection < 315)
                {
                    Direction = "E";
                }
            }

            CEF.ExecuteJs($"Update(\"{Street}\", \"{Crossing}\", \"{Zone}\", \"{Direction}\")");
            */
        }
    }
}
