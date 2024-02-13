﻿using RAGE;
using RAGE.Elements;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Characters
{
    internal class Injuries : Events.Script
    {
        static Dictionary<int, string> BoneNames = new Dictionary<int, string>
        {
            //COMPONENET ID - OFFSET
            { 0, "ROOT" },
        };
        /*
         SKEL_ROOT	0
FB_R_Brow_Out_000	1356
SKEL_L_Toe0	2108
MH_R_Elbow	2992
SKEL_L_Finger01	4089
SKEL_L_Finger02	4090
SKEL_L_Finger31	4137
SKEL_L_Finger32	4138
SKEL_L_Finger41	4153
SKEL_L_Finger42	4154
SKEL_L_Finger11	4169
SKEL_L_Finger12	4170
SKEL_L_Finger21	4185
SKEL_L_Finger22	4186
RB_L_ArmRoll	5232
IK_R_Hand	6286
RB_R_ThighRoll	6442
SKEL_R_Clavicle	10706
FB_R_Lip_Corner_000	11174
SKEL_Pelvis	11816
IK_Head	12844
SKEL_L_Foot	14201
MH_R_Knee	16335
FB_LowerLipRoot_000	17188
FB_R_Lip_Top_000	17719
SKEL_L_Hand	18905
FB_R_CheekBone_000	19336
FB_UpperLipRoot_000	20178
FB_L_Lip_Top_000	20279
FB_LowerLip_000	20623
SKEL_R_Toe0	20781
FB_L_CheekBone_000	21550
MH_L_Elbow	22711
SKEL_Spine0	23553
RB_L_ThighRoll	23639
PH_R_Foot	24806
SKEL_Spine1	24816
SKEL_Spine2	24817
SKEL_Spine3	24818
FB_L_Eye_000	25260
SKEL_L_Finger00	26610
SKEL_L_Finger10	26611
SKEL_L_Finger20	26612
SKEL_L_Finger30	26613
SKEL_L_Finger40	26614
FB_R_Eye_000	27474
SKEL_R_Forearm	28252
PH_R_Hand	28422
FB_L_Lip_Corner_000	29868
SKEL_Head	31086
IK_R_Foot	35502
RB_Neck_1	35731
IK_L_Hand	36029
SKEL_R_Calf	36864
RB_R_ArmRoll	37119
FB_Brow_Centre_000	37193
SKEL_Neck_1	39317
SKEL_R_UpperArm	40269
FB_R_Lid_Upper_000	43536
RB_R_ForeArmRoll	43810
SKEL_L_UpperArm	45509
FB_L_Lid_Upper_000	45750
MH_L_Knee	46078
FB_Jaw_000	46240
FB_L_Lip_Bot_000	47419
FB_Tongue_000	47495
FB_R_Lip_Bot_000	49979
SKEL_R_Thigh	51826
SKEL_R_Foot	52301
IK_Root	56604
SKEL_R_Hand	57005
SKEL_Spine_Root	57597
PH_L_Foot	57717
SKEL_L_Thigh	58271
FB_L_Brow_Out_000	58331
SKEL_R_Finger00	58866
SKEL_R_Finger10	58867
SKEL_R_Finger20	58868
SKEL_R_Finger30	58869
SKEL_R_Finger40	58870
PH_L_Hand	60309
RB_L_ForeArmRoll	61007
SKEL_L_Forearm	61163
FB_UpperLip_000	61839
SKEL_L_Calf	63931
SKEL_R_Finger01	64016
SKEL_R_Finger02	64017
SKEL_R_Finger31	64064
SKEL_R_Finger32	64065
SKEL_R_Finger41	64080
SKEL_R_Finger42	64081
SKEL_R_Finger11	64096
SKEL_R_Finger12	64097
SKEL_R_Finger21	64112
SKEL_R_Finger22	64113
SKEL_L_Clavicle	64729
FACIAL_facialRoot	65068
IK_L_Foot	65245 
          */

        public string GetBoneName(int boneid)
        {
            if (BoneNames.ContainsKey(boneid))
            {
                return BoneNames[boneid];
            }
            else
            {
                return "Nem található.";
            }
        }

        public Injuries()
        {
            Events.OnIncomingDamage += IncomingDamage;
            Events.OnOutgoingDamage += OutgoingDamage;
            Events.OnPlayerDeath += PlayerDeath;
            
        }

        private void PlayerDeath(RAGE.Elements.Player player, uint reason, RAGE.Elements.Player killer, Events.CancelEventArgs cancel)
        {
            if (player == RAGE.Elements.Player.LocalPlayer)
            {
                Chat.Output("Meghaltál.");
                RAGE.Game.Graphics.StartScreenEffect("DeathFailMPIn", 10000, false);
                if (killer != null)
                {
                    Chat.Output(killer.Name + " ölt meg téged. Indok:" + reason);
                }
            }
            else
            {
                Chat.Output(player.Name + " meghalt.");
            }
        }

        static DateTime dt = DateTime.Now;
        static TimeSpan span = TimeSpan.FromSeconds(1);
        private void OutgoingDamage(RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, RAGE.Elements.Player sourcePlayer, ulong weaponHash, ulong boneIdx, int damage, Events.CancelEventArgs cancel)
        {
            //itt kell majd menteni a játékosok közti sebzést mert pontosabb
            //ha egy játékos megsérül pl. esésben azzal még nem tudom mi legyen
            if (targetEntity.Type == RAGE.Elements.Type.Player)
            {
                RAGE.Elements.Player p = targetEntity as RAGE.Elements.Player;
                int bone = -1;
                p.GetLastDamageBone(ref bone);
                
                p.ClearLastDamageBone();

                Events.CallRemote("server:AddDamageToPlayer", p.RemoteId, damage, bone);
            }

           // Events.CallRemote("server:Log", "Kimenő sebzés. Célpont: " + p.Name + " DMG: " + damage + " BONE: " + bone + " ALTERNATÍV BONE: " + boneIdx);
        }

        private void IncomingDamage(RAGE.Elements.Player sourcePlayer, RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, ulong weaponHash, ulong boneIdx, int damage, Events.CancelEventArgs cancel)
        {
            RAGE.Elements.Player p = targetEntity as RAGE.Elements.Player;
            if (sourcePlayer != null)
            {
                if (weaponHash == RAGE.Util.Joaat.Hash("weapon_beanbag"))//ha beanbag talált el
                {
                    RAGE.Elements.Player.LocalPlayer.SetToRagdoll(5000, 10000, 0, false, false, false);
                    Events.CallRemote("server:BeanBagHit");
                }
            }
            int bone = -1;
            RAGE.Elements.Player.LocalPlayer.GetLastDamageBone(ref bone);
            RAGE.Elements.Player.LocalPlayer.GetLastDamageBone(ref bone);

            
            Events.CallRemote("server:Log", "Bejövő sebzés. Forrás: " + p.Name + " DMG: " + damage + " BONE: " + bone + " ALTERNATÍV BONE: " + boneIdx);
        }

        private static string CurrentAnim = null;
        private static bool Crawling = false;
        private static string dict = "anim@scripted@data_leak@fix_bil_ig2_chopper_crawl@prototype@";

        public static void EnableInjuredCrawl()
        {
            if (RAGE.Elements.Player.LocalPlayer.Vehicle != null) return;
            if (RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat == false)
            {
                if (dt < DateTime.Now)
                {
                    if (Crawling)
                    {
                        DisableInjuredCrawl();
                    }
                    else
                    {
                        Crawling = true;
                        idleCrawl = true;
                        RAGE.Game.Streaming.RequestAnimDict("anim@scripted@data_leak@fix_bil_ig2_chopper_crawl@prototype@");
                        RAGE.Elements.Player.LocalPlayer.TaskPlayAnim("anim@scripted@data_leak@fix_bil_ig2_chopper_crawl@prototype@", "crawl_idle_ped", 8f, 1000f, -1, 2, 0, false, false, false);

                        Events.Tick += HandleControls;
                    }
                    dt = DateTime.Now + span;
                }
            }
        }

        public static void DisableInjuredCrawl()
        {
            Crawling = false;
            idleCrawl = true;
            Events.Tick -= HandleControls;
            RAGE.Elements.Player.LocalPlayer.ClearTasks();
        }



        private static DateTime LastAnim = DateTime.Now;
        private static bool idleCrawl = false;
        private static void HandleControls(List<Events.TickNametagData> nametags)
        {
            if (!Crawling)
            {
                RAGE.Elements.Player.LocalPlayer.ClearTasks();
                return;
            }

            RAGE.Game.Pad.DisableControlAction(0, 32, true);
            RAGE.Game.Pad.DisableControlAction(0, 33, true);
            RAGE.Game.Pad.DisableControlAction(0, 34, true);
            RAGE.Game.Pad.DisableControlAction(0, 35, true);

            float rotation = RAGE.Elements.Player.LocalPlayer.GetHeading();

            if (RAGE.Game.Pad.IsDisabledControlPressed(0, 32))
            {
                idleCrawl = false;
                CurrentAnim = "crawl_to_passout_ped";
                RAGE.Game.Streaming.RequestAnimDict(dict);
                float timer = RAGE.Game.Entity.GetEntityAnimTotalTime(RAGE.Elements.Player.LocalPlayer.Handle, dict, CurrentAnim)-700f;
                if (LastAnim + TimeSpan.FromMilliseconds(timer) <= DateTime.Now)
                {
                    RAGE.Elements.Player.LocalPlayer.TaskPlayAnim(dict, CurrentAnim, 8f, 256f, -1, 1, 0f, false, false, false);
                    LastAnim = DateTime.Now;
                }
            }
            else
            {
                if (idleCrawl == false)
                {
                    RAGE.Elements.Player.LocalPlayer.TaskPlayAnim(dict, CurrentAnim, 8f, 256f, -1, 2, 0, false, false, false);
                    idleCrawl = true;
                }
            }

            if (RAGE.Game.Pad.IsDisabledControlPressed(0, 34))
            {
                rotation += 0.015f;
                RAGE.Elements.Player.LocalPlayer.SetRotation(0f, 0f, rotation, 2, false);
            }
            else if (RAGE.Game.Pad.IsDisabledControlPressed(0, 35))
            {
                rotation -= 0.015f;
                RAGE.Elements.Player.LocalPlayer.SetRotation(0f, 0f, rotation, 2, false);
            }
        }
    }
}
