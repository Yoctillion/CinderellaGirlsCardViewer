using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CinderellaGirlsCardViewer.Models
{
    public class MobageClient : NotificationObject, IDisposable
    {
        private static readonly string[] Urls =
        {
            "http://mbga.jp",
            "http://sp.pf.mbga.jp",
            "http://sp.mbga.jp",
            "http://www.mbga.jp"
        };

        private readonly HttpClient _client;

        private readonly HttpClientHandler _handler;
        private const string UserAgent = "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.23 Mobile Safari/537.36";

        public MobageClient()
        {
            this._handler = new HttpClientHandler();
            this._client = new HttpClient(this._handler, false);
            this._client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        }

        private bool _loggedIn;

        public void LoadCookie()
        {
            if (!this._loggedIn)
            {
                this._loggedIn = true;
                this._handler.CookieContainer = Helper.GetCookieContainerFromUris(Urls);
            }
        }

        public HttpClient GetClient()
        {
            var client = new HttpClient(this._handler, false);
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            return client;
        }

        public async Task<IEnumerable<Card>> GetCardsOfCharacter(Character character)
        {
            var characterPage = await this._client.GetStringAsync(character.CardsEntry);

            return characterPage.GetCards(character.Name);
        }

        public async Task Download(Uri url, string path)
        {
            using (var response = await this._client.GetAsync(url))
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(path, content);

                var time = response.GetLastModifedTime();
                File.SetLastWriteTime(path, time);
            }
        }

        public void Dispose()
        {
            this._client.Dispose();
            this._handler.Dispose();
        }
    }
}
