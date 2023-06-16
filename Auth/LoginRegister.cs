using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Net.Mail;
using RAGE;
using RAGE.Elements;
using System.Threading;
using RAGE.Game;

namespace Client.Login
{
    class LoginRegister : Events.Script
    {
        RAGE.Ui.HtmlWindow LoginCEF;
        RAGE.Ui.HtmlWindow RegisterCEF;
        public LoginRegister() {
            Events.OnPlayerReady += ProcessLoginScreen;

            Events.Add("ShowLoginForm", ShowLoginForm);
            Events.Add("ShowRegisterForm", ShowRegisterForm);

            
            Events.Add("client:LoginAttempt", LoginAttempt);
            Events.Add("client:RegisterAttempt", RegisterAttempt);
            Events.Add("client:LogError", ErrorLog);
        }

        public void ErrorLog(object[] args)
        {
            Chat.Output(args.ToString());
        }

        public void ProcessLoginScreen()
        {
            CreateAuthForms();
        }

        public void CreateAuthForms()
        {
            for (int i = 0; i < 362; i++)
            {
                Pad.DisableControlAction(0, i, true);
            }

            LoginCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");
            LoginCEF.Active = true;
            RegisterCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/register.html");
            RegisterCEF.Active = true;
            RegisterCEF.Active = false;
            SetHudState(true);
        }

        public void DestroyAuthForms()
        {
            LoginCEF.Active = false;
            RegisterCEF.Active = false;
            LoginCEF.Destroy();
            RegisterCEF.Destroy();
        }

        public void ShowLoginForm(object[] args)
        {
            RegisterCEF.Active = false;
            LoginCEF.Active = true;
        }

        public void ShowRegisterForm(object[] args)
        {
            RegisterCEF.Active = true;
            LoginCEF.Active = false;
        }

        int camera = 1;
        public void SetLoginCamera(object[] args)
        {
            float posX = Convert.ToSingle(args[0]);
            float posY = Convert.ToSingle(args[1]);
            float posZ = Convert.ToSingle(args[2]);
            float rotX = Convert.ToSingle(args[3]);
            float rotY = Convert.ToSingle(args[4]);
            float rotZ = Convert.ToSingle(args[5]);
            float fov = Convert.ToSingle(args[6]);
            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 501.61f, 5604.5f, 797.9f, 0f, 0f, 0f, fov, true, 2);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
        }



        public void LoginAttempt(object[] args)
        {
            Events.CallRemote("server:LoginAttempt", (string)args[0], (string)args[1]);
        }

        public void RegisterAttempt(object[] args)
        {
            string username = (string)args[0];
            string email = (string)args[1];
            string password1 = (string)args[1];
            string password2 = (string)args[1];
            Events.CallRemote("server:RegisterAttempt", username, email, password1, password2);
        }

        public void LoadIPL(object[] args)
        {
            string name = Convert.ToString(args[0]);
            RAGE.Game.Streaming.RequestIpl(name);
            RAGE.Chat.Output(RAGE.Game.Streaming.IsIplActive(name).ToString());
        }



        public void SetHudState(bool flag)
        {
            RAGE.Nametags.Enabled = flag;
            RAGE.Game.Ui.DisplayRadar(flag);
            RAGE.Chat.Show(flag);
        }



    }
}
