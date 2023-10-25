﻿using RAGE.Game;
using RAGE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

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
            Events.OnEntityStreamIn += OnEntityStreamIn;
            Events.OnPlayerWeaponShot += WeaponShot;
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

                if (p.HasData("player:Invisible"))
                {
                    bool invisible = (bool)p.GetSharedData("player:Invisible");
                    if (invisible)
                    {
                        p.SetAlpha(0, false);
                    }
                    else
                    {
                        p.SetAlpha(255, false);
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
