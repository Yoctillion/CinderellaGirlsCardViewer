using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsQuery;
using CsQuery.ExtensionMethods.Internal;

namespace CinderellaGirlsCardViewer.Models
{
    public class CharacterClient : NotificationObject, IDisposable
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

        public CharacterClient()
        {
            this._handler = new HttpClientHandler();
            this._client = new HttpClient(this._handler);
            this._client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.23 Mobile Safari/537.36");
        }

        public void LoadCookie()
        {
            this._handler.CookieContainer = Helper.GetCookieContainerFromUris(Urls);
        }


        public CharacterPageLoader Query(string keyword)
        {
            return new CharacterPageLoader(this._client, keyword);
        }

        public async Task<IEnumerable<Card>> GetCardsOfCharacter(Character character)
        {
            var characterPage = await this._client.GetStringAsync(character.CardsEntry);

            CQ dom = characterPage;
            return dom.GetCards(character.Name);
        }

        public async Task Download(Uri url, string path)
        {
            var content = await this._client.GetByteArrayAsync(url);
            File.WriteAllBytes(path, content);
        }

        public void Dispose()
        {
            this._client.Dispose();
            this._handler.Dispose();
        }
    }
}
