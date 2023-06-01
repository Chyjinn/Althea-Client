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
                Binds.ToggleCursor();
                return 1;
            });

            Key.Bind(Keys.VK_F3, true, () =>
            {
                Binds.toggleRecording();
                return 1;
            });

            Key.Bind(Keys.VK_F5, true, () =>
            {
                int cam = Cam.GetRenderingCam();
                Cam.SetCamFov(cam, 130f);
                return 1;
            });
        }

        public static void ToggleCursor()
        {
            RAGE.Ui.Cursor.Visible = !RAGE.Ui.Cursor.Visible;
        }

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


    }
}
