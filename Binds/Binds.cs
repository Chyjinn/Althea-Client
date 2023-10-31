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
        public static void bindKeys()
        {
            mouseBind = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.M, true, ToggleCursor);
            inventoryBind = RAGE.Input.Bind(73, true, Inventory.Items.ToggleInventory);
        }

        public static void unbindKeys()
        {
            RAGE.Input.Unbind(mouseBind);
            RAGE.Input.Unbind(inventoryBind);
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
            RAGE.Ui.Cursor.ShowCursor(!RAGE.Ui.Cursor.Visible, !RAGE.Ui.Cursor.Visible);
        }

        public static void ToggleCursor(bool flag)
        {
            RAGE.Ui.Cursor.ShowCursor(flag, flag);
        }


    }
}
