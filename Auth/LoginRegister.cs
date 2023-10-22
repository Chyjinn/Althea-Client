using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Net.Mail;
using RAGE;
using RAGE.Elements;
using System.Threading;
using RAGE.Game;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

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
            //Events.OnPlayerReady += GetTokenFromJS;//amint betöltött a játék megpróbáljuk betölteni a login tokent
            Events.Add("client:LoginScreen", LoginScreen);
            Events.Add("client:ShowLoginForm", ShowLoginForm);
            Events.Add("client:ShowRegisterForm", ShowRegisterForm);
            Events.Add("client:DestroyAuthForm", DestroyAuthForm);

            Events.Add("client:LoginAttempt", LoginAttempt);
            Events.Add("client:LoginAttemptWithToken", LoginAttemptWithToken);

            Events.Add("client:IncorrectToken", IncorrectToken);

            Events.Add("client:RegisterAttempt", RegisterAttempt);

            Events.Add("client:SaveToken", SaveToken);
            Events.Add("client:LoadToken", LoadToken);
            RAGE.Game.Graphics.TransitionToBlurred(500);
        }

        private void LoginScreen(object[] args)
        {
            //a szerver meghívja hogy jelenítsük meg a login screent. Betöltjük a token-t
            GetTokenFromJS();
            //ez meg fogja hívni a client:LoadToken-t
        }

        private void LoadToken(object[] args)//A "js:loadToken" ezt fogja meghívni, itt vizsgálni kell hogy valid token van-e ha a player belenyúlna (jó adatokat kapunk-e, lejárt-e a token)
        {
            if (args.Length == 3)
            {
                //TODO: kezelni ha a user belenyúl a tokenbe
                if (args[0].ToString() == "0" || args[1].ToString() == "tokenerror" || args[2].ToString() == "0")//ha nem tudja beolvasni a JS a tokent akkor is ezeket az értékeket kapjuk
                {
                    ProcessLoginScreen(false);
                }
                else
                {
                    //jó adatokat kaptunk
                    uint accid = 0;
                    string token = args[1].ToString();
                    DateTime exp = DateTime.MinValue;
                    UInt32.TryParse((string)args[0], out accid);
                    DateTime.TryParse((string)args[2], out exp);

                    t = new TokenData(Convert.ToUInt32(args[0]), args[1].ToString(), Convert.ToDateTime(args[2]));
                    if (t.expiration > DateTime.Now && t.accID != 0)//megvizsgáljuk hogy lejárt-e már a helyi token, alapból egyezik az adatbázissal de ha a user átírja akkor lehet megkapja ezt, szerver oldalon is kell ellenőrizni
                    {
                        ProcessLoginScreen(true);
                    }
                    else//lejárt a token
                    {
                        ProcessLoginScreen(false);
                    }
                }
            }
            else//nem 3 adatot tartalmaz
            {
                ProcessLoginScreen(false);
            }
        }

        public void ProcessLoginScreen(bool type)//type=true -> token-es login | type=false -> sima login
        {
            SetHudState(true);
            RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
            if (type)//tokenlogin
            {
                AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/tokenlogin.html");
                int x = 1920;
                int y = 1080;
                RAGE.Ui.Cursor.ShowCursor(true, true);
                RAGE.Game.Graphics.GetActiveScreenResolution(ref x, ref y);
                AuthCEF.ExecuteJs($"SetResolution(\"{x}\", \"{y}\")");
            }
            else//sima login
            {
                AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");
                int x = 1920;
                int y = 1080;
                RAGE.Ui.Cursor.ShowCursor(true, true);
                RAGE.Game.Graphics.GetActiveScreenResolution(ref x, ref y);
                AuthCEF.ExecuteJs($"SetResolution(\"{x}\", \"{y}\")");
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

        public void ShowLoginForm(object[] args)//CEF hívja
        {
            DestroyAuthForm();
            AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");
            AuthCEF.Active = true;
        }

        public void ShowRegisterForm(object[] args)//CEF hívja
        {
            DestroyAuthForm();
            AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/register.html");
            AuthCEF.Active = true;
        }



        private void GetTokenFromJS()//client:LoadToken-t hívja majd vissza
        {
            Events.CallLocal("js:LoadToken");
        }

        private void SaveToken(object[] args)//szerver meghívja, mentjük .storage-ba a tokent
        {
            uint accid = Convert.ToUInt32(args[0]);
            string token = args[1].ToString();
            string expiration = args[2].ToString();
            Events.CallLocal("js:SaveToken", accid, token, expiration);
        }

        public void SetHudState(bool flag)
        {
            RAGE.Game.Ui.DisplayRadar(flag);
        }



        private void IncorrectToken(object[] args)
        {
            AuthCEF.Active = false;
            AuthCEF.Destroy();
            AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");

            object[] arg = new object[3] { 0, "", 0 };
            SaveToken(arg);
        }



        public void LoginAttempt(object[] args)
        {
            Events.CallRemote("server:LoginAttempt", (string)args[0], (string)args[1], (bool)args[2]);
        }

        public void RegisterAttempt(object[] args)
        {
            Events.CallRemote("server:RegisterAttempt", (string)args[0], (string)args[1], (string)args[2], (string)args[3]);
        }
        private void LoginAttemptWithToken(object[] args)
        {
            Events.CallRemote("server:LoginAttemptWithToken", t.accID, t.tokenString);
        }




    }
}
