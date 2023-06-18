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
        RAGE.Ui.HtmlWindow AuthCEF;
        public LoginRegister() {
            Events.OnPlayerReady += ProcessLoginScreen;

            Events.Add("client:ShowLoginForm", ShowLoginForm);
            Events.Add("client:ShowRegisterForm", ShowRegisterForm);
            Events.Add("client:DestroyAuthForm", DestroyAuthForm);
            Events.Add("client:LoginAttempt", LoginAttempt);
            Events.Add("client:RegisterAttempt", RegisterAttempt);
        }

        public void ProcessLoginScreen()
        {
            SetHudState(true);
            CreateAuthForm();
            RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
        }

        public void CreateAuthForm()
        {
            for (int i = 0; i < 362; i++)
            {
                Pad.DisableControlAction(0, i, true);
            }
            
            AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");
            AuthCEF.Active = true;
        }

        public void DestroyAuthForm(object[] args)//Szerver oldali hívásnál
        {
            AuthCEF.Active = false;
            AuthCEF.Destroy();
        }

        public void DestroyAuthForm()//Local hívásnál
        {
            AuthCEF.Active = false;
            AuthCEF.Destroy();
        }

        public void ShowLoginForm(object[] args)
        {
            DestroyAuthForm();
            AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");
            AuthCEF.Active = true;
        }

        public void ShowRegisterForm(object[] args)
        {
            DestroyAuthForm();
            AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/register.html");
            AuthCEF.Active = true;
        }

        public void LoginAttempt(object[] args)
        {
            Events.CallRemote("server:LoginAttempt", (string)args[0], (string)args[1]);
        }

        public void RegisterAttempt(object[] args)
        {
            Events.CallRemote("server:RegisterAttempt", (string)args[0], (string)args[1], (string)args[2], (string)args[3]);
        }

        public void SetHudState(bool flag)
        {
            RAGE.Nametags.Enabled = flag;
            RAGE.Game.Ui.DisplayRadar(flag);
            RAGE.Chat.Show(flag);
        }



    }
}
