using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Characters
{
    public class Character
    {
        public uint Id { get; set; }//-1
        public string Name { get; set; }//-2
        public DateTime DOB { get; set; }//-3
        public string POB { get; set; }//-4
        public float posX { get; set; }//-5
        public float posY { get; set; }//-6
        public float posZ { get; set; }//-7
        public float Rot { get; set; }//-8
        public int AppearanceID { get; set; }//-9
        public Appearance Appearance { get; set; }//-10
    }

    public class Appearance
    {
        public int Id { get; set; }//-11
        public bool Gender { get; set; }//0
        public byte EyeColor { get; set; }//1
        public byte HairColor { get; set; }//2
        public byte HairHighlight { get; set; }//3
        //PARENTS
        public byte Parent1Face { get; set; }//4
        public byte Parent2Face { get; set; }//5
        public byte Parent3Face { get; set; }//6
        public byte Parent1Skin { get; set; }//7
        public byte Parent2Skin { get; set; }//8
        public byte Parent3Skin { get; set; }//9
        //MIX
        public byte FaceMix { get; set; }//10
        public byte SkinMix { get; set; }//11
        public byte OverrideMix { get; set; }//12
        public int HairStyle { get; set; } //-12
        //FACE
        public sbyte NoseWidth { get; set; }//13
        public sbyte NoseHeight { get; set; }//14
        public sbyte NoseLength { get; set; }//15
        public sbyte NoseBridge { get; set; }//16
        public sbyte NoseTip { get; set; }//17
        public sbyte NoseBroken { get; set; }//18
        public sbyte BrowHeight { get; set; }//19
        public sbyte BrowWidth { get; set; }//20
        public sbyte CheekboneHeight { get; set; }//21
        public sbyte CheekboneWidth { get; set; }//22
        public sbyte CheekWidth { get; set; }//23
        public sbyte Eyes { get; set; }//24
        public sbyte Lips { get; set; }//25
        public sbyte JawWidth { get; set; }//26
        public sbyte JawHeight { get; set; }//27
        public sbyte ChinLength { get; set; }//28
        public sbyte ChinPosition { get; set; }//29
        public sbyte ChinWidth { get; set; }//30
        public sbyte ChinShape { get; set; }//31
        public sbyte NeckWidth { get; set; }//32
        //OVERLAYS - 13 db, összesen 30 v 31
        public byte BlemishId { get; set; }//33
        public byte BlemishOpacity { get; set; }//34
        public byte FacialHairId { get; set; }//35
        public byte FacialHairColor { get; set; }//36
        public byte FacialHairOpacity { get; set; }//37
        public byte EyeBrowId { get; set; }//38
        public byte EyeBrowColor { get; set; }//39
        public byte EyeBrowOpacity { get; set; }//40
        public byte AgeId { get; set; }//41
        public byte AgeOpacity { get; set; }//42
        public byte MakeupId { get; set; }//43
        public byte MakeupOpacity { get; set; }//44
        public byte BlushId { get; set; }//45
        public byte BlushColor { get; set; }//46
        public byte BlushOpacity { get; set; }//47
        public byte ComplexionId { get; set; }//48
        public byte ComplexionOpacity { get; set; }//49
        public byte SundamageId { get; set; }//50
        public byte SundamageOpacity { get; set; }//51
        public byte LipstickId { get; set; }//52
        public byte LipstickColor { get; set; }//53
        public byte LipstickOpacity { get; set; }//54
        public byte FrecklesId { get; set; }//55
        public byte FrecklesOpacity { get; set; }//56
        public byte ChestHairId { get; set; }//57
        public byte ChestHairColor { get; set; }//58
        public byte ChestHairOpacity { get; set; }//59
        public byte BodyBlemish1Id { get; set; }//60
        public byte BodyBlemish1Opacity { get; set; }//61
        public byte BodyBlemish2Id { get; set; }//62
        public byte BodyBlemish2Opacity { get; set; }//63
    }
}
