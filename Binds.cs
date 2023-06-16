using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    internal static class Binds
    {

        public static void bindKeys()
        {
            Key.Bind(Keys.VK_M, true, () =>
            {
                ToggleCursor();
                return 1;
            });

            Key.Bind(Keys.VK_F3, true, () =>
            {
                toggleRecording();
                return 1;
            });

            Key.Bind(Keys.VK_F5, true, () =>
            {
                int cam = Cam.GetRenderingCam();
                Cam.SetCamFov(cam, 130f);
                return 1;
            });
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
            RAGE.Ui.Cursor.ShowCursor(!RAGE.Ui.Cursor.Visible, !RAGE.Ui.Cursor.Visible);
        }

        public static void ToggleCursor(bool flag)
        {
            RAGE.Ui.Cursor.ShowCursor(flag, flag);
        }


    }
}
