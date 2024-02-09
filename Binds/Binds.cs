using Client.Characters;
using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Client.Binds
{
    internal static class Binds
    {
        static int mouseBind;
        static int inventoryBind;
        static int recordBind;
        static int crouchBind;
        static int crawlBind;

        static int propertyBind;
        static int propertyLockBind;
        public static void bindKeys()
        {
            mouseBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.M, true, ToggleCursor);
            inventoryBind = RAGE.Input.Bind(73, true, Inventory.Items.ToggleInventory);
            recordBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.F3, true, toggleRecording);
            crouchBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.X, true, ToggleCrouching);
            crawlBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Z, true, Controls.CrawlHandler);
            int injuries = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.U, true, Characters.Injuries.EnableInjuredCrawl);

            RAGE.Game.Misc.SetFadeOutAfterDeath(false);
            //Events.CallRemote("server:SendWind");//lekérjük a szélsebességet a szerverről
        }

        public static void bindEnterPropety()
        {
            propertyBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.E, true, Interiors.Properties.EnterProperty);
        }

        public static void bindToggleLockProperty()
        {
            propertyLockBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.K, true, Interiors.Properties.TogglePropertyLock);
        }


        public static void bindExitProperty()
        {
            propertyBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.E, true, Interiors.Properties.ExitProperty);
        }


        public static void unbindProperty()
        {
            RAGE.Input.Unbind(propertyBind);
        }


        public static void unbindKeys()
        {
            RAGE.Input.Unbind(mouseBind);
            RAGE.Input.Unbind(inventoryBind);
            RAGE.Input.Unbind(recordBind);
            RAGE.Input.Unbind(crouchBind);
            RAGE.Input.Unbind(crawlBind);
        }

        public static void toggleRecording()
        {
            if (!Recording.IsRecording())
            {
                Recording.Start(1);
            }
            else
            {
                Recording.StopAndSaveClip();
            }
        }

        public static void startRockstarEditor()
        {
            Rendering.ResetEditorValues();
            Rendering.ActivateRockstarEditor();
        }

        public static void ToggleCursor()
        {
            if (!RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat)
            {
                RAGE.Ui.Cursor.ShowCursor(!RAGE.Ui.Cursor.Visible, !RAGE.Ui.Cursor.Visible);
                if (!RAGE.Ui.Cursor.Visible)
                {
                    Inventory.Items.HideTooltip();
                }
            }
        }

        public static void ToggleCursor(bool flag)
        {
            if (!RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat)
            {
                RAGE.Ui.Cursor.ShowCursor(flag, flag);
                if (!RAGE.Ui.Cursor.Visible)
                {
                    Inventory.Items.HideTooltip();
                }
            }
           
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



    }
}
