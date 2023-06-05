using RAGE;
using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class EditorTest : Events.Script
    {
        public EditorTest() {
            Key.Bind(Keys.VK_F3, true, () =>
            {
                Binds.toggleRecording();
                return 1;
            });
            Key.Bind(Keys.VK_F4, true, () =>
            {
                Binds.startRockstarEditor();
                return 1;
            });

        }

    }
}
