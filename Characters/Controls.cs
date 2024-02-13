using RAGE.Game;
using RAGE;
using System;
using System.Collections.Generic;

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
            Events.AddDataHandler("Player:Frozen", PlayerFrozen);
            Events.AddDataHandler("Player:Invisible", PlayerInvisible);
            Events.AddDataHandler("Player:Ragdoll", PlayerRagdoll);
            Events.AddDataHandler("Player:WeaponTint", WeaponTint);
            Events.AddDataHandler("Player:Crouching", PlayerCrouch);


            Events.OnEntityStreamIn += OnEntityStreamIn;
            Events.OnPlayerWeaponShot += WeaponShot;
            Events.Tick += Tick;


            RAGE.Game.Streaming.RequestClipSet(crouchClipset);
            RAGE.Game.Streaming.RequestClipSet(strafeClipSet);
        }

        static DateTime dt = DateTime.Now;
        static TimeSpan span = TimeSpan.FromSeconds(1);
        public static async void ToggleCrouching()
        {
            if (RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat == false)
            {
                if (dt < DateTime.Now)
                {
                    Events.CallRemote("server:ToggleCrouching");
                    dt = DateTime.Now + span;
                }

            }
        }

        private static string CurrentAnim = null;
        private static bool Crawling = false;
        private static string dict = "move_crawl";

        public static void CrawlHandler()
        {
            if (RAGE.Elements.Player.LocalPlayer.Vehicle != null || RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat) return;

            if (!RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat)
            {
                if (dt < DateTime.Now)
                {
                    if (Crawling)
                    {
                        Crawling = false;
                        idleCrawl = true;
                        Events.Tick -= HandleControls;
                        RAGE.Elements.Player.LocalPlayer.ClearTasks();
                    }
                    else
                    {
                        Crawling = true;
                        idleCrawl = true;

                        if (RAGE.Elements.Player.LocalPlayer.IsRunning() ||RAGE.Elements.Player.LocalPlayer.IsSprinting())
                        {
                            RAGE.Game.Streaming.RequestAnimDict("move_jump");
                            RAGE.Elements.Player.LocalPlayer.TaskPlayAnim("move_jump", "dive_start_run", 8f, 1000f, -1, 2, 0, false, false, false);
                            float timer = RAGE.Game.Entity.GetEntityAnimTotalTime(RAGE.Elements.Player.LocalPlayer.Handle, "move_jump", "dive_start_run");
                            Chat.Output("futásból");
                            RAGE.Task.Run(() =>
                            {
                                Chat.Output("földre");
                                RAGE.Game.Streaming.RequestAnimDict("move_crawlprone2crawlfront");
                                RAGE.Elements.Player.LocalPlayer.TaskPlayAnim("move_crawlprone2crawlfront", "front", 512f, 1000f, -1, 2, 0, false, false, false);
                            }, Convert.ToInt32(timer));

                        }
                        else
                        {
                            RAGE.Game.Streaming.RequestAnimDict("move_crawlprone2crawlfront");
                            RAGE.Elements.Player.LocalPlayer.TaskPlayAnim("move_crawlprone2crawlfront", "front", 512f, 1000f, -1, 2, 0, false, false, false);
                        }
                       


                        Events.Tick += HandleControls;
                    }
                    dt = DateTime.Now + span;
                }

            }
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
                CurrentAnim = "onfront_fwd";
                RAGE.Game.Streaming.RequestAnimDict(dict);
                float timer = RAGE.Game.Entity.GetEntityAnimTotalTime(RAGE.Elements.Player.LocalPlayer.Handle, dict, CurrentAnim);
                if (LastAnim + TimeSpan.FromMilliseconds(timer) <= DateTime.Now)
                {
                    RAGE.Elements.Player.LocalPlayer.TaskPlayAnim(dict, CurrentAnim, 8f, 256f, -1, 1, 0f, false, false, false);
                    LastAnim = DateTime.Now;
                }
                
            }
            else if (RAGE.Game.Pad.IsDisabledControlPressed(0, 33))
            {
                idleCrawl = false;
                CurrentAnim = "onfront_bwd";
                RAGE.Game.Streaming.RequestAnimDict(dict);
                float timer = RAGE.Game.Entity.GetEntityAnimTotalTime(RAGE.Elements.Player.LocalPlayer.Handle, dict, CurrentAnim);

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
                rotation += 0.8f;
                RAGE.Elements.Player.LocalPlayer.SetRotation(0f, 0f, rotation, 2, false);
            }
            else if (RAGE.Game.Pad.IsDisabledControlPressed(0, 35))
            {
                rotation -= 0.8f;
                RAGE.Elements.Player.LocalPlayer.SetRotation(0f, 0f, rotation, 2, false);
            }
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

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (RAGE.Game.Ped.IsPedInCover(RAGE.Elements.Player.LocalPlayer.Handle,false) && !RAGE.Game.Ped.IsPedAimingFromCover(RAGE.Elements.Player.LocalPlayer.Handle))//fedezékben van és nem céloz
            {
                Pad.DisableControlAction(2, 24, true);
                Pad.DisableControlAction(2, 142, true);
                Pad.DisableControlAction(2, 257, true);
                //kikapcsoljuk hogy ne tudjon lőni
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
                if (p.HasData("Player:Frozen"))
                {
                    bool frozen = (bool)p.GetSharedData("Player:Frozen");
                    p.FreezePosition(frozen);
                }

                if (p.HasData("Player:Ragdoll"))
                {
                    bool state = (bool)p.GetSharedData("Player:Ragdoll");
                    if (state)
                    {
                        p.SetToRagdoll(1000000, 0, 0, true, true, false);
                    }
                    
                }

                if (p.HasData("Player:Crouching"))
                {
                    bool crouching = (bool)p.GetSharedData("Player:Crouching");
                    if (crouching)
                    {
                        p.SetMovementClipset(crouchClipset, clipSetSwitchTime);
                        p.SetStrafeClipset(strafeClipSet);
                    }
                }

                if (p.HasData("Player:Invisible"))
                {
                    bool invisible = (bool)p.GetSharedData("Player:Invisible");
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
