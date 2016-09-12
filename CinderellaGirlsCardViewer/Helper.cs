﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace CinderellaGirlsCardViewer
{
    public static class Helper
    {
        #region Win32

        private const int InternetCookieHttponly = 0x2000;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            int dwFlags,
            IntPtr lpReserved);


        private const int UrlmonOptionUseragent = 0x10000001;

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        #endregion

        public static CookieContainer GetCookieContainerFromUris(IEnumerable<string> uris)
        {
            var cookies = new CookieContainer();
            foreach (var uri in uris)
            {
                var datasize = 8192 * 16;
                var cookieData = new StringBuilder(datasize);
                if (InternetGetCookieEx(uri, null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
                {
                    cookies.SetCookies(new Uri(uri), cookieData.ToString().Replace(';', ','));
                }
            }

            return cookies;
        }

        public static void RegisterBrowserVersion()
        {
            const string key = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
            try
            {
                var valueName = Process.GetCurrentProcess().ProcessName + ".exe";
                Registry.SetValue(key, valueName, 11000, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static void ChangeBrowserUserAgent(string userAgent)
        {
            UrlMkSetSessionOption(UrlmonOptionUseragent, userAgent, userAgent.Length, 0);
        }
    }
}