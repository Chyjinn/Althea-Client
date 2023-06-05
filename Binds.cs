using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    internal static class Binds
    {
        public static void ToggleCursor(bool flag)
        {
            RAGE.Ui.Cursor.Visible = flag;
        }

        public static void toggleRecording()
        {
            if (!RAGE.Game.Recording.IsRecording())
            {
                RAGE.Game.Recording.Start(1);
            }
            else
            {
                RAGE.Game.Recording.StopAndSaveClip();
            }
        }

        public static void startRockstarEditor()
        {
            RAGE.Game.Rendering.ResetEditorValues();
            RAGE.Game.Rendering.ActivateRockstarEditor();
        }

        public static void ToggleCursor()
        {
            RAGE.Ui.Cursor.Visible = !RAGE.Ui.Cursor.Visible;
        }
    }
}
