using RAGE.Game;
using RAGE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace Client.Characters
{
    internal class Controls : Events.Script
    {
        int[] disabledControls = new int[32]{ 
            30, // A & D
            31, // W & S
            21, // Left Shift
            36, // Left Ctrl
            22, // Space
            24, // Attack
            44, // Q
            38, // E
            71, // W - Vehicle
            72, // S - Vehicle
            59, // A & D - Vehicle
            60, // L Shift & L CTRL - Vehicle
            42, // D PAD Up || ]
            43, // D PAD Down || [
            85,
            86,
            15, // Mouse Wheel Up
            14, // Mouse Wheel Down
            228,
            229,
            172,
            173,
            37,
            44,
            178,
            244,
            220,
            221,
            218,
            219,
            16,
            17 };

        public Controls()
        {
            Events.AddDataHandler("player:Frozen", PlayerFrozen);
            Events.AddDataHandler("player:Invisible", PlayerInvisible);
            Events.AddDataHandler("player:Ragdoll", PlayerRagdoll);
            Events.AddDataHandler("player:WeaponTint", WeaponTint);
            Events.AddDataHandler("player:Crouching", PlayerCrouch);


            Events.OnEntityStreamIn += OnEntityStreamIn;
            Events.OnPlayerWeaponShot += WeaponShot;
            Events.OnIncomingDamage += Damage;
            Events.OnOutgoingDamage += OutDamage;
            Events.Tick += Tick;

            RAGE.Game.Streaming.RequestClipSet(crouchClipset);
            RAGE.Game.Streaming.RequestClipSet(strafeClipSet);
            RAGE.Game.Streaming.RequestAnimDict("move_crawlprone2crawlfront");
            RAGE.Game.Streaming.RequestAnimDict("move_crawl");
        }


        string crouchClipset = "move_ped_crouched";
        string strafeClipSet = "move_ped_crouched_strafing";
        float clipSetSwitchTime = 0.25f;
        public static bool crawling = false;

        private void PlayerCrouch(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                if (Convert.ToBoolean(arg))//guggolás
                {
                    p.SetMovementClipset(crouchClipset, clipSetSwitchTime);
                    p.SetStrafeClipset(strafeClipSet);
                }
                else//kiszállni
                {
                    p.ResetMovementClipset(clipSetSwitchTime);
                    p.ResetStrafeClipset();
                }
            }
        }
        string crawlAnim = "";
        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (RAGE.Game.Ped.IsPedInCover(RAGE.Elements.Player.LocalPlayer.Handle,false) && !RAGE.Game.Ped.IsPedAimingFromCover(RAGE.Elements.Player.LocalPlayer.Handle))//fedezékben van és nem céloz
            {
                Pad.DisableControlAction(2, 24, true);
                Pad.DisableControlAction(2, 142, true);
                Pad.DisableControlAction(2, 257, true);
                //kikapcsoljuk hogy ne tudjon lőni
            }
            /*
            if (crawling)//ha be van kapcsolva a kúszás
            {
                Vector3 rot = RAGE.Elements.Player.LocalPlayer.GetRotation(2);
                Pad.DisableControlAction(0, 32, true);
                Pad.DisableControlAction(0, 33, true);
                Pad.DisableControlAction(0, 34, true);
                Pad.DisableControlAction(0, 35, true);
                if (Pad.IsDisabledControlPressed(0,34))
                {
                    RAGE.Elements.Player.LocalPlayer.SetRotation(rot.X, rot.Y, rot.Z + 0.2f, 2, true);
                }
                if (Pad.IsDisabledControlPressed(0, 354))
                {
                    RAGE.Elements.Player.LocalPlayer.SetRotation(rot.X, rot.Y, rot.Z - 0.2f, 2, true);
                }

                if (Pad.IsDisabledControlPressed(0, 32))
                {
                    if (crawlAnim == "onfront_fwd" || crawlAnim == "onfront_bwd")
                    {
                        return;
                    }
                    crawlAnim = "onfront_fwd";


                    RAGE.Elements.Player.LocalPlayer.SetRotation(rot.X, rot.Y, rot.Z + 0.2f, 2, true);
                }

            }
            */
        }



        private void OutDamage(RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, RAGE.Elements.Player sourcePlayer, ulong weaponHash, ulong boneIdx, int damage, Events.CancelEventArgs cancel)
        {
            if (weaponHash == RAGE.Util.Joaat.Hash("weapon_beanbag"))//ha beanbag talált el
            {
                RAGE.Game.Graphics.StartParticleFxNonLoopedOnEntity("water_boat_wash", targetEntity.Id, 0f, 0f, 0f, 0f, 0f, 0f, 2f, true, true, true);
            }
        }

        private void WeaponTint(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                p.SetWeaponTintIndex(RAGE.Util.Joaat.Hash("weapon_beanbag"), Convert.ToInt32(arg));
            }
        }

        private void Damage(RAGE.Elements.Player sourcePlayer, RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, ulong weaponHash, ulong boneIdx, int damage, Events.CancelEventArgs cancel)
        {
            Chat.Output("Damage: " + damage);
            
            if (sourcePlayer != null)
            {
                if (weaponHash == RAGE.Util.Joaat.Hash("weapon_beanbag"))//ha beanbag talált el
                {
                    RAGE.Elements.Player.LocalPlayer.SetToRagdoll(5000, 10000, 0, false, false, false);
                    Events.CallRemote("server:BeanBagHit");
                }
                
                //Chat.Output(sourcePlayer.Name + " megsebzett " + damage + " (" + boneIdx + ")");
                /*
                if (boneIdx == 12844 || boneIdx == 31086)//fejbe lőttek
                {
                    RAGE.Elements.Player.LocalPlayer.SetToRagdoll(500, 1000, 2, false, false, false);
                }
                */

            }
        }

        private void WeaponShot(Vector3 targetPos, RAGE.Elements.Player target, Events.CancelEventArgs cancel)
        {
            Random r = new Random();

            int value = r.Next(-100, 101);
            float percent = Convert.ToSingle(value) / 70f;
            float heading = Cam.GetGameplayCamRelativeHeading() + percent;
            float pitch = Cam.GetGameplayCamRelativePitch() + percent;
            //RAGE.Game.Cam.SetGameplayCamShakeAmplitude(100f);
            RAGE.Game.Cam.ShakeGameplayCam("SMALL_EXPLOSION_SHAKE", 0.07f);
            RAGE.Game.Cam.SetGameplayCamRelativeHeading(heading);
            RAGE.Game.Cam.SetGameplayCamRelativePitch(pitch, 1f);
            //Cam.SetGameplayCamRawYaw
            //RAGE.Game.Pad.SetControlNormal(0, 270, 1f);
            //RAGE.Game.Pad.SetControlNormal(0, 272, -1f);
        }

        private void PlayerRagdoll(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                bool state = Convert.ToBoolean(arg);

                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                if (state)
                {
                    p.SetToRagdoll(10000000, 0, 0, true, true, false);
                }
                else
                {
                    p.SetToRagdoll(1, 0, 0, true, true, false);
                }
            }
        }

        private void PlayerInvisible(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                bool state = Convert.ToBoolean(arg);

                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                if (state)
                {
                    p.SetAlpha(0, false);
                }
                else
                {
                    p.SetAlpha(255, false);
                }
            }
        }

        private void PlayerFrozen(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                bool state = Convert.ToBoolean(arg);

                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                p.FreezePosition(state);
                if (p.RemoteId == RAGE.Elements.Player.LocalPlayer.RemoteId)
                {
                    if (state)
                    {
                        Events.Tick += KeepControlsDisabled;
                    }
                    else
                    {
                        Events.Tick -= KeepControlsDisabled;
                    }
                }
            }
        }

        public void OnEntityStreamIn(RAGE.Elements.Entity entity)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                if (p.HasData("player:Frozen"))
                {
                    bool frozen = (bool)p.GetSharedData("player:Frozen");
                    p.FreezePosition(frozen);
                }

                if (p.HasData("player:Ragdoll"))
                {
                    bool state = (bool)p.GetSharedData("player:Ragdoll");
                    if (state)
                    {
                        p.SetToRagdoll(1000000, 0, 0, true, true, false);
                    }
                    
                }

                if (p.HasData("player:Crouching"))
                {
                    bool crouching = (bool)p.GetSharedData("player:Crouching");
                    if (crouching)
                    {
                        p.SetMovementClipset(crouchClipset, clipSetSwitchTime);
                        p.SetStrafeClipset(strafeClipSet);
                    }
                }

                if (p.HasData("player:Invisible"))
                {
                    bool invisible = (bool)p.GetSharedData("player:Invisible");
                    if (invisible)
                    {
                        p.SetAlpha(0, true);
                    }
                    else
                    {
                        p.SetAlpha(255, true);
                    }
                }
            }
        }

        
        private void KeepControlsDisabled(List<Events.TickNametagData> nametags)
        {
            for (int i = 0; i < disabledControls.Length; i++)
            {
                Pad.DisableControlAction(0, disabledControls[i], true);
            }
        }
    }
}
