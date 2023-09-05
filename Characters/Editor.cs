using Newtonsoft.Json.Linq;
using RAGE;
using RAGE.Game;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Numerics;

namespace Client.Characters
{
    class Editor : Events.Script
    {
        private Character character;
        Appearance appearance;
        RAGE.Ui.HtmlWindow EditorCEF;
        public Editor()
        {
            Events.Add("client:CharEdit",CharacterEditor);

            Events.Add("client:AttributeToServer", AttributeToServer);
            Events.Add("client:FinishEditing", FinishEditing);

            Events.Add("client:LoadCharacterAppearance", LoadCharacterAppearance);
        }

        private void FinishEditing(object[] args)
        {
            Events.CallRemote("server:FinishEditing", RAGE.Util.Json.Serialize<Character>(character));//json string-ként átküldjük amit állítottunk eddig kliens oldalon
        }


        private void LoadCharacterAppearance(object[] args)
        {
            character = RAGE.Util.Json.Deserialize<Character>(args[0].ToString());//megkapjuk szervertől a karaktert, lementjük
            Chat.Output("Karakter betöltve szerver oldalról..." + args[0].ToString());
            HandleCharacterAppearance();
        }

        private void AttributeToServer(object[] args)
        {
            //itt átállítani a megfelelő értéket
            switch (Convert.ToInt32(args[0]))
            {
                default:
                    break;
                case -2://Name
                    character.Name = args[1].ToString();
                    break;
                case -3://POB
                    character.POB = args[1].ToString();
                    break;
                case -4://DOB
                    character.DOB = Convert.ToDateTime(args[1]);
                    break;
                case -12:
                    character.Appearance.HairStyle = Convert.ToInt32(args[1]);
                    break;
                case 0:
                    character.Appearance.Gender = Convert.ToBoolean(args[1]);
                    break;
                case 1:
                    character.Appearance.EyeColor = Convert.ToByte(args[1]);
                    break;
                case 2:
                    character.Appearance.HairColor = Convert.ToByte(args[1]);
                    break;
                case 3:
                    character.Appearance.HairHighlight = Convert.ToByte(args[1]);
                    break;
                case 4:
                    character.Appearance.Parent1Face = Convert.ToByte(args[1]);
                    break;
                case 5:
                    character.Appearance.Parent2Face = Convert.ToByte(args[1]);
                    break;
                case 6:
                    character.Appearance.Parent3Face = Convert.ToByte(args[1]);
                    break;
                case 7:
                    character.Appearance.Parent1Skin = Convert.ToByte(args[1]);
                    break;
                case 8:
                    character.Appearance.Parent2Skin = Convert.ToByte(args[1]);
                    break;
                case 9:
                    character.Appearance.Parent3Skin = Convert.ToByte(args[1]);
                    break;
                case 10:
                    character.Appearance.FaceMix = Convert.ToByte(args[1]);
                    break;
                case 11:
                    character.Appearance.SkinMix = Convert.ToByte(args[1]);
                    break;
                case 12:
                    character.Appearance.OverrideMix = Convert.ToByte(args[1]);
                    break;
                case 13:
                    character.Appearance.NoseWidth = Convert.ToSByte(args[1]);
                    break;
                case 14:
                    character.Appearance.NoseHeight = Convert.ToSByte(args[1]);
                    break;
                case 15:
                    character.Appearance.NoseLength = Convert.ToSByte(args[1]);
                    break;
                case 16:
                    character.Appearance.NoseBridge = Convert.ToSByte(args[1]);
                    break;
                case 17:
                    character.Appearance.NoseTip = Convert.ToSByte(args[1]);
                    break;
                case 18:
                    character.Appearance.NoseBroken = Convert.ToSByte(args[1]);
                    break;
                case 19:
                    character.Appearance.BrowHeight = Convert.ToSByte(args[1]);
                    break;
                case 20:
                    character.Appearance.BrowWidth = Convert.ToSByte(args[1]);
                    break;
                case 21:
                    character.Appearance.CheekboneHeight = Convert.ToSByte(args[1]);
                    break;
                case 22:
                    character.Appearance.CheekboneWidth = Convert.ToSByte(args[1]);
                    break;
                case 23:
                    character.Appearance.CheekWidth = Convert.ToSByte(args[1]);
                    break;
                case 24:
                    character.Appearance.Eyes = Convert.ToSByte(args[1]);
                    break;
                case 25:
                    character.Appearance.Lips = Convert.ToSByte(args[1]);
                    break;
                case 26:
                    character.Appearance.JawWidth = Convert.ToSByte(args[1]);
                    break;
                case 27:
                    character.Appearance.JawHeight = Convert.ToSByte(args[1]);
                    break;
                case 28:
                    character.Appearance.ChinLength = Convert.ToSByte(args[1]);
                    break;
                case 29:
                    character.Appearance.ChinPosition = Convert.ToSByte(args[1]);
                    break;
                case 30:
                    character.Appearance.ChinWidth = Convert.ToSByte(args[1]);
                    break;
                case 31:
                    character.Appearance.ChinShape = Convert.ToSByte(args[1]);
                    break;
                case 32:
                    character.Appearance.NeckWidth = Convert.ToSByte(args[1]);
                    break;
                case 33:
                    character.Appearance.BlemishId = Convert.ToByte(args[1]);
                    break;
                case 34:
                    character.Appearance.BlemishOpacity = Convert.ToByte(args[1]);
                    break;
                case 35:
                    character.Appearance.FacialHairId = Convert.ToByte(args[1]);
                    break;
                case 36:
                    character.Appearance.FacialHairColor = Convert.ToByte(args[1]);
                    break;
                case 37:
                    character.Appearance.FacialHairOpacity = Convert.ToByte(args[1]);
                    break;
                case 38:
                    character.Appearance.EyeBrowId = Convert.ToByte(args[1]);
                    break;
                case 39:
                    character.Appearance.EyeBrowColor = Convert.ToByte(args[1]);
                    break;
                case 40:
                    character.Appearance.EyeBrowOpacity = Convert.ToByte(args[1]);
                    break;
                case 41:
                    character.Appearance.AgeId = Convert.ToByte(args[1]);
                    break;
                case 42:
                    character.Appearance.AgeOpacity = Convert.ToByte(args[1]);
                    break;
                case 43:
                    character.Appearance.MakeupId = Convert.ToByte(args[1]);
                    break;
                case 44:
                    character.Appearance.MakeupOpacity = Convert.ToByte(args[1]);
                    break;
                case 45:
                    character.Appearance.BlushId = Convert.ToByte(args[1]);
                    break;
                case 46:
                    character.Appearance.BlushColor = Convert.ToByte(args[1]);
                    break;
                case 47:
                    character.Appearance.BlushOpacity = Convert.ToByte(args[1]);
                    break;
                case 48:
                    character.Appearance.ComplexionId = Convert.ToByte(args[1]);
                    break;
                case 49:
                    character.Appearance.ComplexionOpacity = Convert.ToByte(args[1]);
                    break;
                case 50:
                    character.Appearance.SundamageId = Convert.ToByte(args[1]);
                    break;
                case 51:
                    character.Appearance.SundamageOpacity = Convert.ToByte(args[1]);
                    break;
                case 52:
                    character.Appearance.LipstickId = Convert.ToByte(args[1]);
                    break;
                case 53:
                    character.Appearance.LipstickColor = Convert.ToByte(args[1]);
                    break;
                case 54:
                    character.Appearance.LipstickOpacity = Convert.ToByte(args[1]);
                    break;
                case 55:
                    character.Appearance.FrecklesId = Convert.ToByte(args[1]);
                    break;
                case 56:
                    character.Appearance.FrecklesOpacity = Convert.ToByte(args[1]);
                    break;
                case 57:
                    character.Appearance.ChestHairId = Convert.ToByte(args[1]);
                    break;
                case 58:
                    character.Appearance.ChestHairColor = Convert.ToByte(args[1]);
                    break;
                case 59:
                    character.Appearance.ChestHairOpacity = Convert.ToByte(args[1]);
                    break;
                case 60:
                    character.Appearance.BodyBlemish1Id = Convert.ToByte(args[1]);
                    break;
                case 61:
                    character.Appearance.BodyBlemish1Opacity = Convert.ToByte(args[1]);
                    break;
                case 62:
                    character.Appearance.BodyBlemish2Id = Convert.ToByte(args[1]);
                    break;
                case 63:
                    character.Appearance.BodyBlemish2Opacity = Convert.ToByte(args[1]);
                    break;

            }
            //aztán beállítani a karakter kinézetét
            HandleCharacterAppearance();
        }


        public void HandleCharacterAppearance()
        {
            
            if (character.Appearance.Gender)//férfi
            {
                RAGE.Elements.Player.LocalPlayer.Model = 0x705E61F2;
                for (int i = 0; i < 19; i++)
                {
                    if (i != 2)
                    {
                        RAGE.Elements.Player.LocalPlayer.SetComponentVariation(i,0,0,0);
                    }
                }
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(3, 15, 0, 0);
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(4, 61, 0, 0);
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(6, 34, 0, 0);
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(8, 15, 0, 0);
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(11, 15, 0, 0);
                //beállítjuk az alap ruhákat neki
            }
            else
            {
                RAGE.Elements.Player.LocalPlayer.Model = 0x9C9EFFD8;
                for (int i = 0; i < 19; i++)
                {
                    if (i != 2)
                    {
                        RAGE.Elements.Player.LocalPlayer.SetComponentVariation(i, 0, 0, 0);
                    }
                }
                //beállítjuk az alap ruhákat neki
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(3, 15, 0, 0);
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(4, 15, 0, 0);
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(6, 35, 0, 0);
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(8, 2, 0, 0);
                RAGE.Elements.Player.LocalPlayer.SetComponentVariation(11, 15, 0, 0);
            }
            
            //parents
            RAGE.Elements.Player.LocalPlayer.SetHeadBlendData(character.Appearance.Parent1Face, character.Appearance.Parent2Face, character.Appearance.Parent3Face, character.Appearance.Parent1Skin, character.Appearance.Parent2Skin, character.Appearance.Parent3Skin, (float)character.Appearance.FaceMix/100f, (float)character.Appearance.SkinMix/100f, (float)character.Appearance.OverrideMix/100f, true);
            //face features
            float[] FaceFeatures = character.Appearance.GetFaceFeatures();

            for (int i = 0; i < 19; i++)
            {
                RAGE.Elements.Player.LocalPlayer.SetFaceFeature(i,FaceFeatures[i]);
            }
            
            //overlays
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(0, character.Appearance.BlemishId, (float)character.Appearance.BlemishOpacity/100f);
            
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(1, character.Appearance.FacialHairId, (float)character.Appearance.FacialHairOpacity/100f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(1, 1, character.Appearance.FacialHairColor, character.Appearance.FacialHairColor);
            //1 - szőr, 2 - smink

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(2, character.Appearance.EyeBrowId, (float)character.Appearance.EyeBrowOpacity/100f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(2, 1, character.Appearance.EyeBrowColor, character.Appearance.EyeBrowColor);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(3, character.Appearance.AgeId, (float)character.Appearance.AgeOpacity / 100f);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(4, character.Appearance.MakeupId, (float)character.Appearance.MakeupOpacity / 100f);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(5, character.Appearance.BlushId, (float)character.Appearance.BlushOpacity / 100f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(5, 2, character.Appearance.BlushColor, character.Appearance.BlushColor);


            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(6, character.Appearance.ComplexionId, (float)character.Appearance.ComplexionOpacity / 100f);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(7, character.Appearance.SundamageId, (float)character.Appearance.SundamageOpacity / 100f);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(8, character.Appearance.LipstickId, (float)character.Appearance.LipstickOpacity / 100f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(8, 2, character.Appearance.LipstickColor, character.Appearance.LipstickColor);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(9, character.Appearance.FrecklesId, (float)character.Appearance.FrecklesOpacity / 100f);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(10, character.Appearance.ChestHairId, (float)character.Appearance.ChestHairOpacity / 100f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(10, 1, character.Appearance.ChestHairColor, character.Appearance.ChestHairColor);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(11, character.Appearance.BodyBlemish1Id, (float)character.Appearance.BodyBlemish1Opacity / 100f);

            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(12, character.Appearance.BodyBlemish2Id, (float)character.Appearance.BodyBlemish2Opacity / 100f);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(2, character.Appearance.HairStyle, 0, 0);
            RAGE.Elements.Player.LocalPlayer.SetEyeColor(character.Appearance.EyeColor);
            RAGE.Elements.Player.LocalPlayer.SetHairColor(character.Appearance.HairColor, character.Appearance.HairHighlight);
        }



        public void CharacterEditor(object[] args)
        {
            bool state = (bool)args[0];

            if (state)
            {
                EditorCEF = new RAGE.Ui.HtmlWindow("package://frontend/editor/charedit.html");
                EditorCEF.Active = true;
                //itt le kell kezelni még a karakter változását majd
                //szerver beállította a karakter kinézetét, nekünk azt le kell kérni és menteni kliens oldalra
                
            }
            else
            {
                EditorCEF.Active = false;
                EditorCEF.Destroy();
            }
        }


        


    }
}
