using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Client.Characters
{
    class Editor : Events.Script
    {

        //RAGE.Input.Unbind(leftbindid);
        //nem jó mert csak 1x hívja meg a függvényt
        RAGE.Ui.HtmlWindow EditorCEF;
        public Editor()
        {
            Events.Add("client:CharEdit",CharacterEditor);

            Events.Add("client:AttributeToServer", AttributeToServer);
            Events.Add("client:FinishEditing", FinishEditing);
        }

        private void FinishEditing(object[] args)
        {
            Events.CallRemote("server:FinishEditing");
        }

        private void AttributeToServer(object[] args)
        {
            Events.CallRemote("server:EditAttribute", Convert.ToInt32(args[0]), Convert.ToString(args[1]));//attribute id, value
        }
        public void CharacterEditor(object[] args)
        {
            bool state = (bool)args[0];

            if (state)
            {
                EditorCEF = new RAGE.Ui.HtmlWindow("package://frontend/editor/charedit.html");
                EditorCEF.Active = true;
            }
            else
            {
                EditorCEF.Active = false;
                EditorCEF.Destroy();
            }
        }


        


    }
}
