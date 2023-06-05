using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Net.Mail;
using RAGE;
using RAGE.Elements;
using System.Threading;

namespace Client.Login
{
    class Login : Events.Script
    {
        RAGE.Ui.HtmlWindow LoginCEF = null;
        RAGE.Ui.HtmlWindow RegisterCEF = null;
        public Login() {
            Events.Add("ShowLoginForm", ShowLoginForm);
            Events.Add("ShowRegisterForm", ShowRegisterForm);
            Events.Add("LoadIPL", LoadIPL);
            LoginCEF = new RAGE.Ui.HtmlWindow("package://frontend/login.html");
            RegisterCEF = new RAGE.Ui.HtmlWindow("package://frontend/register.html");
            LoginCEF.Active = false;
            RegisterCEF.Active = false;
            RAGE.Game.Streaming.RequestIpl("V_Michael_Scuba");
            Key.Bind(Keys.VK_M, true, () =>
            {
                Binds.ToggleCursor();
                return 1;
            });
            
        }
        public void LoadIPL(object[] args)
        {
            string name = Convert.ToString(args[0]);
            RAGE.Game.Streaming.RequestIpl(name);
            RAGE.Chat.Output(RAGE.Game.Streaming.IsIplActive(name).ToString());
        }

        public void SendLoginInfoToServer(object[] args)
        {
            Events.CallRemote("LoginInfoFromClient", (string)args[0], (string)args[1]);
        }
        int camera = 1;

        public void SetLoginCamera(float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
        {
            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", posX, posY, posZ, rotX, rotY, rotZ, 90.0f, true, 2);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
        }

        public void ShowLoginForm(object[] args)
        {
            bool flag = (bool)args[0];
            Binds.ToggleCursor(true);

            LoginCEF.Active= flag;
            
            
        }

        public void ShowRegisterForm(object[] args)
        {
            bool flag = (bool)args[0];
            Binds.ToggleCursor(true);
            RegisterCEF.Active = flag;
        }

        public void SetHudState(bool flag)
        {
            RAGE.Nametags.Enabled = flag;
            RAGE.Game.Ui.DisplayRadar(flag);
            RAGE.Chat.Show(flag);
        }



    }
}
