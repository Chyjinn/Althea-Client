using RAGE;
using RAGE.Elements;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        RAGE.Elements.Player LocalPlayer = RAGE.Elements.Player.LocalPlayer;
        int hp = 0;

        public Injuries()
        {
            Events.OnIncomingDamage += IncomingDamage;
            hp = RAGE.Elements.Player.LocalPlayer.GetHealth();
            Events.Tick += Tick;
            Events.AddDataHandler("Player:Injured", PlayerInjured);
            Events.Add("client:EnableAnim", EnableAnim);
            Events.Add("client:DisableAnim", DisableAnim);
            Events.Add("client:EnableInjuredCrawl", EnableInjuredCrawl);
            Events.Add("client:DisableInjuredCrawl", DisableInjuredCrawl);
            RAGE.Game.Graphics.ClearTimecycleModifier();
            RAGE.Game.Graphics.StopAllScreenEffects();
            RAGE.Game.Misc.SetFadeOutAfterDeath(true);
            RAGE.Game.Misc.SetFadeInAfterDeathArrest(true);
            RAGE.Elements.Player.LocalPlayer.SetSuffersCriticalHits(false);
        }


        public static void HandlePlayerInjury(RAGE.Elements.Player player, int hp)
        {
            if (hp <= 10)
            {
                RAGE.Game.Streaming.RequestClipSet("move_m@drunk@verydrunk");
                player.SetMovementClipset("move_m@drunk@verydrunk", clipSetSwitchTime);
                player.SetStrafeClipset("move_strafe@injured");

                if (player == RAGE.Elements.Player.LocalPlayer)
                {
                    RAGE.Game.Graphics.StartScreenEffect("PPFilter", 1, true);
                    RAGE.Game.Graphics.ClearTimecycleModifier();
                    RAGE.Game.Graphics.SetTimecycleModifier("drug_wobbly");
                    RAGE.Game.Graphics.SetTimecycleModifierStrength(1f);
                }
            }
            else if (hp <= 20)//nagyon sérült
            {
                RAGE.Game.Streaming.RequestClipSet("move_injured_generic");
                player.SetMovementClipset("move_injured_generic", clipSetSwitchTime);
                player.SetStrafeClipset("move_strafe@injured");

                if (player == RAGE.Elements.Player.LocalPlayer)
                {
                    RAGE.Game.Graphics.ClearTimecycleModifier();
                    RAGE.Game.Graphics.SetTimecycleModifier("drug_wobbly");
                    RAGE.Game.Graphics.SetTimecycleModifierStrength(0.5f);
                    RAGE.Game.Graphics.StopAllScreenEffects();
                }
            }
            else if (hp <= 30)//jobban megsérült, elkezdődik az émelygés
            {
                RAGE.Game.Streaming.RequestClipSet("move_injured_generic");
                player.SetMovementClipset("move_injured_generic", clipSetSwitchTime);
                player.SetStrafeClipset("move_strafe@injured");

                if (player == RAGE.Elements.Player.LocalPlayer)
                {
                    RAGE.Game.Graphics.SetTimecycleModifier("Drunk");
                    RAGE.Game.Graphics.SetTimecycleModifierStrength(0.5f);
                    RAGE.Game.Graphics.StopAllScreenEffects();
                }

            }
            else if (hp <= 40)//kicsit sérült
            {
                if ((bool)player.GetSharedData("Player:Gender"))//férfi
                {
                    RAGE.Game.Streaming.RequestClipSet("move_m@injured");
                    player.SetMovementClipset("move_m@injured", clipSetSwitchTime);
                    player.SetStrafeClipset("move_strafe@injured");
                }
                else
                {
                    RAGE.Game.Streaming.RequestClipSet("move_f@injured");
                    player.SetMovementClipset("move_f@injured", clipSetSwitchTime);
                    player.SetStrafeClipset("move_strafe@injured");
                }

                if (player == RAGE.Elements.Player.LocalPlayer)
                {
                    RAGE.Game.Graphics.ClearTimecycleModifier();
                    RAGE.Game.Graphics.StopAllScreenEffects();
                }
            }
            else//sok hp
            {
                player.ResetMovementClipset(clipSetSwitchTime);
                player.ResetStrafeClipset();
                if (player == RAGE.Elements.Player.LocalPlayer)
                {
                    RAGE.Game.Graphics.ClearTimecycleModifier();
                    RAGE.Game.Graphics.StopAllScreenEffects();
                }
                
            }
        }

        public static void PlayerInjured(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                int hp = Convert.ToInt32(arg);
                HandlePlayerInjury(p, hp);
            }

        }

        private void EnableAnim(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.ClearTasksImmediately();
            LocalPlayer.SetToRagdoll(2000, 4000, 0, true, true, true);
            Events.Tick += HandleAnim;
        }

        private void DisableAnim(object[] args)
        {
            Events.Tick -= HandleAnim;
        }

        private void HandleAnim(List<Events.TickNametagData> nametags)
        {
            for (int i = 0; i < Characters.Controls.disabledControls.Length; i++)
            {
                Pad.DisableControlAction(0, Characters.Controls.disabledControls[i], true);
            }
            LocalPlayer.ResetRagdollTimer();
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            int hploss = 0;
            if (hp != LocalPlayer.GetHealth())
            {
                hploss = hp - LocalPlayer.GetHealth();
                hp = LocalPlayer.GetHealth();
            }
            if (hploss > 0)
            {
                int bone = -1;
                var streamedPlayers = RAGE.Elements.Entities.Players.All.Where((RAGE.Elements.Player player) => player.Exists);
                var streamedVehicles = RAGE.Elements.Entities.Vehicles.All.Where((RAGE.Elements.Vehicle vehicle) => vehicle.Exists);
                if (LocalPlayer.HasBeenDamagedByAnyPed() && LocalPlayer.HasBeenDamagedByAnyVehicle())//elütötte egy autó
                {
                    bool foundAccident = false;
                    if (LocalPlayer.Vehicle != null)//autóbaleset
                    {
                        if (LocalPlayer.GetLastDamageBone(ref bone))
                        {
                            foreach (var item in streamedPlayers)
                            {
                                if (RAGE.Game.Entity.HasEntityBeenDamagedByEntity(LocalPlayer.Handle, item.Handle, true))
                                {
                                    Events.CallRemote("server:PlayerCrashedPlayer", item.RemoteId, hploss, bone);
                                    foundAccident = true;
                                    continue;
                                }
                            }

                            if (foundAccident == false)
                            {
                                Events.CallRemote("server:PlayerCrashed", hploss, bone);
                            }
                        }
                        else
                        {
                            foreach (var item in streamedPlayers)
                            {
                                if (RAGE.Game.Entity.HasEntityBeenDamagedByEntity(LocalPlayer.Handle, item.Handle, true))
                                {
                                    Events.CallRemote("server:PlayerCrashedPlayer", item.RemoteId, hploss, 0);
                                    foundAccident = true;
                                    continue;
                                }
                            }

                            if (foundAccident == false)
                            {
                                Events.CallRemote("server:PlayerCrashed", hploss, bone);
                            }
                        }
                    }
                    else//elütés
                    {
                        if (LocalPlayer.GetLastDamageBone(ref bone))
                        {
                            Events.CallRemote("server:PlayerRammed", hploss, bone);
                        }
                        else
                        {
                            Events.CallRemote("server:PlayerRammed", hploss, 0);
                        }
                    }
                }
                else if (LocalPlayer.HasBeenDamagedByAnyPed())//játékos sértette meg - ez működik
                {
                    if (LocalPlayer.GetLastDamageBone(ref bone))
                    {
                        foreach (var item in streamedPlayers)
                        {
                            if (RAGE.Game.Entity.HasEntityBeenDamagedByEntity(LocalPlayer.Handle,item.Handle,true))
                            {
                                Events.CallRemote("server:PlayerHitPlayer", item.RemoteId, hploss, bone);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in streamedPlayers)
                        {
                            if (RAGE.Game.Entity.HasEntityBeenDamagedByEntity(LocalPlayer.Handle, item.Handle, true))
                            {
                                Events.CallRemote("server:PlayerHitPlayer", item.RemoteId, hploss, 0);
                                continue;
                            }
                        }
                    }
                }
                else if (LocalPlayer.HasBeenDamagedByAnyVehicle())//jármű sértette meg magában - ez működik
                {
                    if (LocalPlayer.GetLastDamageBone(ref bone))
                    {
                        Events.CallRemote("server:PlayerDamagedByVehicle", hploss, bone);
                    }
                    else
                    {
                        Events.CallRemote("server:PlayerDamagedByVehicle", hploss, 0);
                    }
                }
                else if (LocalPlayer.HasBeenDamagedByAnyObject())//object sértette meg
                {
                    if (LocalPlayer.GetLastDamageBone(ref bone))
                    {
                        Events.CallRemote("server:PlayerDamagedByObject", hploss, bone);
                    }
                    else
                    {
                        Events.CallRemote("server:PlayerDamagedByObject", hploss, 0);
                    }
                }
                else//más miatt sérült meg, pl elesett
                {
                    if (LocalPlayer.GetLastDamageBone(ref bone))
                    {
                        Events.CallRemote("server:PlayerDamaged", hploss, bone);
                    }
                    else
                    {
                        Events.CallRemote("server:PlayerDamaged", hploss, 0);
                    }
                }




                LocalPlayer.ClearLastDamageEntity();
            }


        }


        static float clipSetSwitchTime = 0.75f;

        private void IncomingDamage(RAGE.Elements.Player sourcePlayer, RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, ulong weaponHash, ulong boneIdx, int damage, Events.CancelEventArgs cancel)
        {
            if (sourcePlayer != null)
            {
                if (weaponHash == RAGE.Util.Joaat.Hash("weapon_beanbag"))//ha beanbag talált el
                {
                    RAGE.Elements.Player.LocalPlayer.SetToRagdoll(5000, 10000, 0, false, false, false);
                    Events.CallRemote("server:BeanBagHit");
                }
            }
        }



        private static string CurrentAnim = null;
        private static bool Crawling = false;
        private static string dict = "anim@scripted@data_leak@fix_bil_ig2_chopper_crawl@prototype@";

        public static void EnableInjuredCrawl(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.ClearTasksImmediately();
            Crawling = true;
            idleCrawl = true;
            RAGE.Game.Streaming.RequestAnimDict("anim@scripted@data_leak@fix_bil_ig2_chopper_crawl@prototype@");
            RAGE.Elements.Player.LocalPlayer.TaskPlayAnim("anim@scripted@data_leak@fix_bil_ig2_chopper_crawl@prototype@", "crawl_idle_ped", 32f, 1f, -1, 2, 0, false, false, false);

            Events.Tick += HandleControls;
        }

        public static void DisableInjuredCrawl(object[] args)
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
                    RAGE.Elements.Player.LocalPlayer.TaskPlayAnim(dict, CurrentAnim, 4f, 8f, -1, 1, 0f, false, false, false);
                    LastAnim = DateTime.Now;
                }
            }
            else
            {
                if (idleCrawl == false)
                {
                    RAGE.Elements.Player.LocalPlayer.TaskPlayAnim(dict, CurrentAnim, 4f, 8f, -1, 2, 0, false, false, false);
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
