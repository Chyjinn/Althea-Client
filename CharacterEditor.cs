using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RAGE;

namespace Client
{
    class CharacterEditor : Events.Script
    {

        public static void UpdateAppearance(bool gender, int[] parents, int[] skins, float[] resemblance, float[] face)
        {
            CharacterData CD = new CharacterData();
            float[] arc = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            int[] szulok = { 12, 8, 4 };
            int[] borszin = { 29, 8, 12 };
            CD.Face = arc;
            CD.Gender = false;
            CD.Parents = szulok;
            CD.ParentSkins = borszin;
            CD.Shape = 0.5f;
            CD.Tone = 0.5f;
            CD.Modifier = 0.5f;

            RAGE.Chat.Output("Sikeresen frissítettük a karaktert!");
            string model = "mp_f_freemode_01";
            switch (CD.Gender)
            {
                case false:
                    model = "mp_f_freemode_01";
                    break;
                case true:
                    model = "mp_m_freemode_01";
                    break;
            }
            RAGE.Elements.Player.LocalPlayer.Model = RAGE.Game.Misc.GetHashKey(model);

            RAGE.Game.Ped.SetPedHeadBlendData(RAGE.Game.Player.GetPlayerPed(), CD.Parents[0], CD.Parents[1], CD.Parents[2], CD.ParentSkins[0], CD.ParentSkins[1], CD.ParentSkins[2], CD.Shape, CD.Tone, CD.Modifier, false);
        }



        
    }
}
