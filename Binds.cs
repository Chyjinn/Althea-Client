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

        public static void ToggleCursor()
        {
            RAGE.Ui.Cursor.Visible = !RAGE.Ui.Cursor.Visible;
        }
    }
}
