using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Text;

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
        public static void bindKeys()
        {
            mouseBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.M, true, ToggleCursor);
            inventoryBind = RAGE.Input.Bind(73, true, Inventory.Items.ToggleInventory);
            recordBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.F3, true, toggleRecording);
            crouchBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.X, true, ToggleCrouching);
            crawlBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Z, true, ToggleCrawl);
            RAGE.Game.Misc.SetFadeOutAfterDeath(false);
            Events.CallRemote("server:SendWind");//lekérjük a szélsebességet a szerverről
        }

        public static void bindEnterPropety()
        {
            propertyBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.E, true, Interiors.Properties.EnterProperty);
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
        
        public static async void ToggleCrawl()
        {
            if (dt < DateTime.Now)
            {
                if (RAGE.Elements.Player.LocalPlayer.Vehicle == null &&  RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat == false)//nem ír és nincs kocsiban
                {
                    if (Characters.Controls.crawling)//ha kúszik
                    {
                        Characters.Controls.crawling = false;
                        RAGE.Elements.Player.LocalPlayer.ClearTasks();
                        RAGE.Elements.Player.LocalPlayer.ClearSecondaryTask();
                    }
                    else//nem kúszik
                    {
                        Characters.Controls.crawling = true;
                        RAGE.Elements.Player.LocalPlayer.TaskPlayAnim("move_crawlprone2crawlfront", "front", 8f, 1000f, -1, 2, 0, false, false, false);
                    }
                    dt = DateTime.Now + span;
                }
                
            }

        }


    }
}
