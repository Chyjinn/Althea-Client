using RAGE.Game;
using RAGE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace Client.Characters
{
    internal class Controls: Script
    {
        int[] disabledControls = new int[32]{ 30, // A & D
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
            Events.OnEntityStreamIn += OnEntityStreamIn;
        }
        private void PlayerInvisible(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);

                if ((bool)arg == true)
                {
                    p.SetAlpha(0, true);
                }
                else
                {
                    p.SetAlpha(255, true);
                }
            }
        }

        private void PlayerFrozen(RAGE.Elements.Entity entity, object arg, object oldArg)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                bool state = Convert.ToBoolean(arg);
                p.FreezePosition(state);
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

        public void OnEntityStreamIn(RAGE.Elements.Entity entity)
        {
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                RAGE.Elements.Player p = RAGE.Elements.Entities.Players.GetAtRemote(entity.RemoteId);
                bool state = (bool)p.GetSharedData("frozen");
                p.FreezePosition(state);

                bool invisible = (bool)p.GetSharedData("invisible");
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

        
        private void KeepControlsDisabled(List<Events.TickNametagData> nametags)
        {
            for (int i = 0; i < disabledControls.Length; i++)
            {
                Pad.DisableControlAction(0, disabledControls[i], true);
            }
        }
    }
}
