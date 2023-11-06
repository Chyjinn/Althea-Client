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
        public static void bindKeys()
        {
            mouseBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.M, true, ToggleCursor);
            inventoryBind = RAGE.Input.Bind(73, true, Inventory.Items.ToggleInventory);
            recordBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.F3, true, toggleRecording);
            Events.CallRemote("server:SendWind");//lekérjük a szélsebességet a szerverről
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
            }
        }

        public static void ToggleCursor(bool flag)
        {
            if (!RAGE.Elements.Player.LocalPlayer.IsTypingInTextChat)
            {
                RAGE.Ui.Cursor.ShowCursor(flag, flag);
            }
           
        }


    }
}
