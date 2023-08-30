using RAGE;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Client.Characters
{
    class Editor : Events.Script
    {
        //int leftbindid = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Left, true, RotateCharLeft);
        //int rightbindid = RAGE.Input.Bind(RAGE.Ui.VirtualKeys.Right, true, RotateCharRight);
        //RAGE.Input.Unbind(leftbindid);
        //nem jó mert csak 1x hívja meg a függvényt
        RAGE.Ui.HtmlWindow EditorCEF;
        RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
        DateTime nextUpdate = DateTime.Now;
        public Editor()
        {
            Events.Add("client:CharEdit",CharacterEditor);
        }

        public void CharacterEditor(object[] args)
        {
            EditorCEF = new RAGE.Ui.HtmlWindow("package://frontend/editor/charedit.html");
            EditorCEF.Active = true;

            Events.Tick += Tick;

        }
        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.Right))
            {
                RotateCharRight();
            }
            else if (RAGE.Input.IsDown(RAGE.Ui.VirtualKeys.Left))
            {
                RotateCharLeft();
            }
            
        }

        

    private void RotateCharRight()
        {
            Events.CallRemote("server:RotateCharRight");
        }

        private void RotateCharLeft()
        {

            Events.CallRemote("server:RotateCharLeft");
        }
    }
}
