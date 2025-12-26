using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MFPS.ULogin.Discord
{
    public class bl_DiscordAuth : bl_LoginProBase
    {
        public string clientID;
        public string clientSecret;
        [LovattoToogle] public bool alwaysPromptConsent = false;
        public UserData accountProfile;

        private TokenResult tokenResult;
        private string accessCode;
        private bool isWaiting = false;
        private string oauthPass;
        private bool checkingCode = false;

        private const string API_ENDPOINT = "https://discord.com/api/v8";

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            bl_LoginPro.onRequestAuth += OnAuthRequested;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            bl_LoginPro.onRequestAuth -= OnAuthRequested;
        }

        /// <summary>
        /// Start authentication process
        /// </summary>
        void OnAuthRequested(AuthenticationType authentication)
        {
            if (authentication != AuthenticationType.Discord) return;

            RequestOauth();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestOauth()
        {
            if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret))
            {
                Debug.LogWarning("Discord Client ID or Client Secret values hasn't been assigned yet.");
                return;
            }
            oauthPass = bl_DataBaseUtils.GenerateKey();
            string redirect = $"{bl_LoginProDataBase.Instance.GetPhpFolder}discord-oauth.php";
            redirect = UnityWebRequest.EscapeURL(redirect);
            string prompt = alwaysPromptConsent ? "consent" : "none";
            string url = $"https://discord.com/api/oauth2/authorize?response_type=code&client_id={clientID}&scope=identify%20email&state={oauthPass}&redirect_uri={redirect}&prompt={prompt}";
            isWaiting = true;
            Application.OpenURL(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focus"></param>
        private void OnApplicationFocus(bool focus)
        {
            if (focus && isWaiting && !checkingCode)
            {
                CheckAuthCode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void CheckAuthCode()
        {
            bl_ULoginLoadingWindow.Instance?.SetText("Checking authentication...", true);
            checkingCode = true;
            WWWForm wf = new WWWForm();
            wf.AddField("type", 3);
            wf.AddField("state", oauthPass);
            wf.AddField("dbname", "discordSessions");

            string url = GetURL(bl_LoginProDataBase.URLType.OAuth);
            WebRequest.POST(url, wf, (result) =>
            {
                string text = result.RawText;
                if (!result.isError)
                {
                    if (bl_LoginProDataBase.Instance.FullLogs)
                        result.Print();

                    if (!text.Contains("not found"))
                    {
                        string[] data = text.Split('|');
                        if (data[0] == "success")
                        {
                            accessCode = data[2];
                            GetAccessToken(accessCode);
                            isWaiting = false;
                        }
                        else
                        {
                            bl_LoginPro.Instance.SetLogText(result.RawText);
                            result.Print(true);
                            bl_ULoginLoadingWindow.Instance?.SetActive(false);
                        }
                    }
                    else
                    {
                        result.Print();
                        //user has not been authenticated yet.
                        bl_ULoginLoadingWindow.Instance?.SetText("Authenticating with Discord...", true);
                    }
                }
                else
                {
                    result.PrintError();
                    bl_ULoginLoadingWindow.Instance?.SetActive(false);
                }
                checkingCode = false;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        void GetAccessToken(string code)
        {
            string redirect = $"{bl_LoginProDataBase.Instance.GetPhpFolder}discord-oauth.php";
            string url = $"{API_ENDPOINT}/oauth2/token";

            var wf = new WWWForm();
            wf.AddField("code", code);
            wf.AddField("client_id", clientID);
            wf.AddField("client_secret", clientSecret);
            wf.AddField("redirect_uri", redirect);
            wf.AddField("grant_type", "authorization_code");

            var headers = new Dictionary<string, string>() { { "Content-Type", "application/x-www-form-urlencoded" } };

            bl_ULoginLoadingWindow.Instance?.SetText("Obtaining permission...", true);

            WebRequest.POST(url, wf, (result) =>
            {
                if (!result.isError)
                {
                    if (bl_LoginProDataBase.Instance.FullLogs)
                        result.Print();

                    tokenResult = JsonUtility.FromJson<TokenResult>(result.RawText);
                    if (tokenResult != null && !string.IsNullOrEmpty(tokenResult.access_token))
                    {
                        GetUserCredentials(tokenResult.access_token);
                    }
                    else
                    {
                        bl_LoginPro.Instance.SetLogText(result.RawText);
                        bl_ULoginLoadingWindow.Instance?.SetActive(false);

                        if (!bl_LoginProDataBase.Instance.FullLogs)
                            result.Print();
                    }
                }
                else
                {
                    Debug.Log(result.RawText);
                    result.PrintError();
                    bl_ULoginLoadingWindow.Instance?.SetActive(false);
                }
            }, headers);
        }

        /// <summary>
        /// 
        /// </summary>
        void GetUserCredentials(string accessToken)
        {
            string url = $"{API_ENDPOINT}/users/@me?access_token={accessToken}";
            bl_ULoginLoadingWindow.Instance?.SetText("Obtaining data...", true);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"{tokenResult.token_type} {accessToken}");
            headers.Add("Content-Type", "application/json");

            WebRequest.GET(url, (result) =>
            {
                if (!result.isError)
                {
                    if (bl_LoginProDataBase.Instance.FullLogs)
                        result.Print();

                    accountProfile = JsonUtility.FromJson<UserData>(result.Text);
                    if(accountProfile != null)
                    {
                        var credentials = new CustomAuthCredentials();
                        credentials.UniqueID = accountProfile.id;//set the uniqueID as the user name
                        credentials.UserName = accountProfile.id;
                        credentials.Email = accountProfile.email;
                        credentials.authenticationType = AuthenticationType.Discord;
                        credentials.RequireNickName = true;
                        bl_LoginPro.Instance.Authenticate(credentials);
                        return;
                    }
                    else
                    {
                        result.Print(true);
                    }
                }
                else
                {
                    result.PrintError();
                }
                bl_ULoginLoadingWindow.Instance?.SetActive(false);
            }, headers);
        }

        [Serializable]
        public class UserData
        {
            public string id;
            public string username;
            public string email;
            public string discriminator;
            public string avatar;
            public bool verified;
            public int flags;
            public string banner;
            public int accent_color;
            public int premium_type;
            public int public_flags;
        }

        [Serializable]
        public class TokenResult
        {
            public string access_token;
            public string token_type;
            public long expires_in;
            public string refresh_token;
            public string scope;
        }
    }
}