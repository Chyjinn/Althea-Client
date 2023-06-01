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
    class Login : Events.Script
    {
        RAGE.Ui.HtmlWindow LoginCEF;
        RAGE.Ui.HtmlWindow RegisterCEF;
        public Login() {
            Events.Add("CreateAuthForms", CreateAuthForms);
            Events.Add("ShowLoginForm", ShowLoginForm);
            Events.Add("ShowRegisterForm", ShowRegisterForm);

            Events.Add("SetLoginCamera", SetLoginCamera);


            Events.Add("client:LoginAttempt", LoginAttempt);
            Events.Add("client:RegisterAttempt", RegisterAttempt);
        }
        public void LoginAttempt(object[] args)
        {
            string username = (string)args[0];
            string password = (string)args[1];
            Events.CallRemote("server:LoginAttempt", username, password);
        }

        public void RegisterAttempt(object[] args)
        {
            string username = (string)args[0];
            string email = (string)args[1];
            string password1 = (string)args[1];
            string password2 = (string)args[1];
            Events.CallRemote("server:RegisterAttempt", username, email, password1, password2);
        }

        public void SendLoginInfoToServer(object[] args)
        {
            Events.CallRemote("LoginInfoFromClient", (string)args[0], (string)args[1]);
        }

        public void CreateAuthForms(object[] args)
        {
            LoginCEF = new RAGE.Ui.HtmlWindow("package://auth/login.html");
            LoginCEF.Active = true;
            RegisterCEF = new RAGE.Ui.HtmlWindow("package://auth/register.html");
            RegisterCEF.Active = false;
            SetHudState(true);
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
            camera = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", posX, posY, posZ, rotX, rotY, rotZ, fov, true, 2);
            RAGE.Game.Cam.SetCamActive(camera, true);
            RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
        }

        public void SetHudState(bool flag)
        {
            RAGE.Nametags.Enabled = flag;
            RAGE.Game.Ui.DisplayRadar(flag);
            RAGE.Chat.Show(flag);
        }



    }
}
