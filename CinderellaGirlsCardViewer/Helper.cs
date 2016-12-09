using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public static DateTime GetLastModifedTime(this HttpResponseMessage response)
        {
            const string format = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";

            IEnumerable<string> lastModified;

            return response.Headers.TryGetValues("last-modified", out lastModified)
                ? DateTime.ParseExact(lastModified.First(), format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal)
                : DateTime.Now;
        }

        public static double GetOneRowHeight(this ListBox listBox)
        {
            var contentHeight = GetAllListBoxItems(listBox)
                .Select(i => i.ActualHeight)
                .DefaultIfEmpty()
                .Max();

            var scrollBarHeight = SystemParameters.HorizontalScrollBarHeight;

            var border = listBox.BorderThickness;
            var borderHeight = border.Top + border.Bottom;

            return contentHeight + scrollBarHeight + borderHeight;
        }

        public static double GetOneRowWidth(this ListBox listBox)
        {
            var contentWidth = GetAllListBoxItems(listBox)
                .Select(i => i.ActualWidth)
                .DefaultIfEmpty()
                .Max();

            var scrollBarWidth = SystemParameters.VerticalScrollBarWidth;

            var border = listBox.BorderThickness;
            var borderWidth = border.Left + border.Right;

            return contentWidth + scrollBarWidth + borderWidth;
        }

        private static IEnumerable<ListBoxItem> GetAllListBoxItems(ListBox listBox)
        {
            var items = listBox.ItemsSource;
            foreach (var item in items)
            {
                yield return (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(item);
            }
        }

        public static async Task AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> elements, TimeSpan interval)
        {
            foreach (var e in elements)
            {
                collection.Add(e);
                await Task.Delay(interval);
            }
        }
    }
}
