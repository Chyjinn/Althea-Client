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
            Events.OnPlayerReady += GetTokenFromJS;//amint betöltött a játék megpróbáljuk betölteni a login tokent
            
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

        private void GetTokenFromJS()//indításnál fut le (onPlayerReady) -> client:LoadToken-t hívja majd vissza
        {
            Events.CallLocal("js:loadToken");
        }

        private void SaveToken(object[] args)//szerver meghívja, mentjük .storage-ba a tokent
        {
            uint accid = Convert.ToUInt32(args[0]);
            string token = args[1].ToString();
            string expiration = args[2].ToString();
            Events.CallLocal("js:saveToken", accid, token, expiration);
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


        public void SetHudState(bool flag)
        {
            RAGE.Nametags.Enabled = flag;
            RAGE.Game.Ui.DisplayRadar(flag);
            RAGE.Chat.Show(flag);
        }



        private void IncorrectToken(object[] args)
        {
            AuthCEF.Active = false;
            AuthCEF.Destroy();
            AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");

            object[] arg = new object[3] { 0, "", 0 };
            SaveToken(arg);
        }



        private void LoadToken(object[] args)//A "js:loadToken" ezt fogja meghívni, itt vizsgálni kell hogy valid token van-e ha a player belenyúlna (jó adatokat kapunk-e, lejárt-e a token)
        {
            //TODO: kezelni ha a user belenyúl a tokenbe
            if (args[0].ToString() == "0" || args[1].ToString() == "tokenerror" || args[2].ToString() == "0")//ha nem tudja beolvasni a JS a tokent akkor is ezeket az értékeket kapjuk
            {
                ProcessLoginScreen(false);
            }
            else
            {
                //jó adatokat kaptunk
                t = new TokenData(Convert.ToUInt32(args[0]), args[1].ToString(), Convert.ToDateTime(args[2]));
                if (t.expiration > DateTime.Now)//megvizsgáljuk hogy lejárt-e már a helyi token, alapból egyezik az adatbázissal de ha a user átírja akkor lehet megkapja ezt, szerver oldalon is kell ellenőrizni
                {
                    ProcessLoginScreen(true);
                }
            }

        }

        public void ProcessLoginScreen(bool type)//type=true -> token-es login | type=false -> sima login
        {
            SetHudState(true);
            RAGE.Elements.Player.LocalPlayer.FreezePosition(false);

            if (type)//tokenlogin
            {
                //AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/tokenlogin.html");
            }
            else//sima login
            {
                //AuthCEF = new RAGE.Ui.HtmlWindow("package://frontend/auth/login.html");
            }
            //AuthCEF.Active = true;
            
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





    }
}
