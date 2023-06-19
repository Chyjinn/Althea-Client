using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Net.Mail;
using RAGE;
using RAGE.Elements;
using System.Threading;
using RAGE.Game;
using Newtonsoft.Json.Linq;

namespace Client.Login
{
    class TokenData
    {
        public uint accID {  get; set; }
        public string tokenString { get; set; }
        public DateTime expiration { get; set; }
        public TokenData(uint id, string tkstr, DateTime exp)
        {
            accID = id;
            tokenString = tkstr;
            expiration = exp;
        }
    }
    class LoginRegister : Events.Script
    {
        RAGE.Ui.HtmlWindow AuthCEF;
        TokenData t;
        public LoginRegister()
        {
            Events.OnPlayerReady += GetTokenFromJS;

            Events.Add("client:ShowLoginForm", ShowLoginForm);
            Events.Add("client:ShowRegisterForm", ShowRegisterForm);
            Events.Add("client:DestroyAuthForm", DestroyAuthForm);

            Events.Add("client:LoginAttempt", LoginAttempt);
            Events.Add("client:LoginAttemptWithToken", LoginAttemptWithToken);
            Events.Add("client:IncorrectToken", IncorrectToken);

            Events.Add("client:RegisterAttempt", RegisterAttempt);

            Events.Add("client:SaveToken", SaveToken);
            Events.Add("client:LoadToken", LoadToken);
        }

        private void IncorrectToken(object[] args)
        {
            AuthCEF.Active = false;
            AuthCEF.Destroy();
            AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");

            object[] arg = new object[3] { 0, "", 0 };
            SaveToken(arg);
        }

        private void GetTokenFromJS()
        {
            Events.CallLocal("js:loadToken");
        }

        private void LoginAttemptWithToken(object[] args)
        {
            Events.CallRemote("server:LoginAttemptWithToken", t.accID, t.tokenString);
        }

        private void LoadToken(object[] args)
        {
            if (args[0].ToString() == "0" || args[1].ToString() == "tokenerror" || args[2].ToString() == "0")
            {
                ProcessLoginScreen(false);
            }
            else
            {
                t = new TokenData(Convert.ToUInt32(args[0]), args[1].ToString(), Convert.ToDateTime(args[2]));
                if (t.expiration > DateTime.Now)
                {
                    ProcessLoginScreen(true);
                }
            }

        }

        private void SaveToken(object[] args)
        {
            uint accid = Convert.ToUInt32(args[0]);
            string token = args[1].ToString();
            string expiration = args[2].ToString();
            Chat.Output("Új token" + expiration);
            Events.CallLocal("js:saveToken", accid,token,expiration);
        }

        public void ProcessLoginScreen(bool type)
        {
            for (int i = 0; i < 362; i++)
            {
                Pad.DisableControlAction(0, i, true);
            }

            SetHudState(true);
            
            RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
            if (type)
            {
                AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/tokenlogin.html");
            }
            else
            {
                AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");
            }
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
            Events.CallRemote("server:LoginAttempt", (string)args[0], (string)args[1], (bool)args[2]);
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
