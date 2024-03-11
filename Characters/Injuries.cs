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
        RAGE.Elements.Player LocalPlayer = RAGE.Elements.Player.LocalPlayer;
        int hp = 0;
        int armor = 0;

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
            if (player == RAGE.Elements.Player.LocalPlayer)
            {
                RAGE.Game.Graphics.StopAllScreenEffects();
                RAGE.Game.Graphics.ClearTimecycleModifier();
            }
            player.ResetStrafeClipset();
            if (hp <= 10 && hp > 0)
            {
                RAGE.Game.Streaming.RequestClipSet("move_m@drunk@verydrunk");
                player.SetMovementClipset("move_m@drunk@verydrunk", clipSetSwitchTime);

                if (player == RAGE.Elements.Player.LocalPlayer)
                {
                    RAGE.Game.Graphics.StartScreenEffect("PPFilter", 1, true);
                    RAGE.Game.Graphics.SetTimecycleModifier("drug_wobbly");
                    RAGE.Game.Graphics.SetTimecycleModifierStrength(1f);
                }
            }
            else if (hp <= 20)//nagyon sérült
            {
                RAGE.Game.Streaming.RequestClipSet("move_injured_generic");
                player.SetMovementClipset("move_injured_generic", clipSetSwitchTime);


                if (player == RAGE.Elements.Player.LocalPlayer)
                {
                    RAGE.Game.Graphics.SetTimecycleModifier("Drunk");
                    RAGE.Game.Graphics.SetTimecycleModifierStrength(1f);
                }
            }
            else if (hp <= 30)//jobban megsérült, elkezdődik az émelygés
            {
                RAGE.Game.Streaming.RequestClipSet("move_injured_generic");
                player.SetMovementClipset("move_injured_generic", clipSetSwitchTime);


                if (player == RAGE.Elements.Player.LocalPlayer)
                {
                    RAGE.Game.Graphics.SetTimecycleModifier("Drunk");
                    RAGE.Game.Graphics.SetTimecycleModifierStrength(0.5f);
                }
            }
            else if (hp <= 40)//kicsit sérült
            {
                if ((bool)player.GetSharedData("Player:Gender"))//férfi
                {
                    RAGE.Game.Streaming.RequestClipSet("move_m@injured");
                    player.SetMovementClipset("move_m@injured", clipSetSwitchTime);

                }
                else
                {
                    RAGE.Game.Streaming.RequestClipSet("move_f@injured");
                    player.SetMovementClipset("move_f@injured", clipSetSwitchTime);

                }
            }
            else//sok hp
            {
                player.ResetMovementClipset(clipSetSwitchTime);
                player.ResetStrafeClipset();
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
            Events.Tick += DisableControls;
            if (LocalPlayer.Vehicle == null)
            {
                RAGE.Elements.Player.LocalPlayer.ClearTasksImmediately();
                LocalPlayer.SetToRagdoll(1000, 2000, 0, true, true, true);
                Events.Tick += HandleRagdoll;
            }

        }

        private void DisableAnim(object[] args)
        {
            Events.Tick -= DisableControls;
            Events.Tick -= HandleRagdoll;
        }

        private void DisableControls(List<Events.TickNametagData> nametags)
        {
            for (int i = 0; i < Characters.Controls.disabledControls.Length; i++)
            {
                Pad.DisableControlAction(0, Characters.Controls.disabledControls[i], true);
            }
        }

        private void HandleRagdoll(List<Events.TickNametagData> nametags)
        {
            LocalPlayer.ResetRagdollTimer();
        }


        float enginehp = 0f;
        float bodyhp = 0f;
        float bodyhp2 = 0f;

        private void Tick(List<Events.TickNametagData> nametags)
        {
            int hploss = 0;
            int armorloss = 0;

            float enginehploss = 0f;
            float bodyhploss = 0f;
            float bodyhploss2 = 0f;

            if (LocalPlayer.Vehicle != null)//ha járműben ül
            {
                if (enginehp != LocalPlayer.Vehicle.GetEngineHealth())
                {
                    enginehploss = enginehp - LocalPlayer.Vehicle.GetEngineHealth();
                    enginehp = LocalPlayer.Vehicle.GetEngineHealth();
                }

            }









            if (hp != LocalPlayer.GetHealth())
            {
                hploss = hp - LocalPlayer.GetHealth();
                hp = LocalPlayer.GetHealth();
            }

            if (armor != LocalPlayer.GetArmour())
            {
                armorloss = armor - LocalPlayer.GetArmour();
                armor = LocalPlayer.GetArmour();
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
                                    Events.CallRemote("server:PlayerCrashedPlayer", item.RemoteId, hploss, bone, enginehploss);
                                    foundAccident = true;
                                    continue;
                                }
                            }

                            if (foundAccident == false)
                            {
                                Events.CallRemote("server:PlayerCrashed", hploss, bone, enginehploss);
                            }
                        }
                        else
                        {
                            foreach (var item in streamedPlayers)
                            {
                                if (RAGE.Game.Entity.HasEntityBeenDamagedByEntity(LocalPlayer.Handle, item.Handle, true))
                                {
                                    Events.CallRemote("server:PlayerCrashedPlayer", item.RemoteId, hploss, 0, enginehploss);
                                    foundAccident = true;
                                    continue;
                                }
                            }

                            if (foundAccident == false)
                            {
                                Events.CallRemote("server:PlayerCrashed", hploss, bone, enginehploss);
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
