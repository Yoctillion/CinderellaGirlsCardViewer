using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsQuery;

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


        public async Task<IEnumerable<Character>> Query(string keyword)
        {
            const string queryUrl = "http://sp.pf.mbga.jp/12008305/?guid=ON&url=http%3A%2F%2F125.6.169.35%2Fidolmaster%2Fgallery";

            using (var queryResult = await this._client.PostAsync(queryUrl, ToContent(keyword)))
            {
                var str = await queryResult.Content.ReadAsStringAsync();
                CQ dom = str;

                var idols = dom.GetCharacters();

                string nextPageUrl;
                while ((nextPageUrl = dom.GetNextPageUrl()) != null)
                {
                    Debug.WriteLine("Query " + nextPageUrl);

                    dom = await this._client.GetStringAsync(nextPageUrl);
                    idols = idols.Concat(dom.GetCharacters());
                }

                return idols;
            }
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

        private static FormUrlEncodedContent ToContent(string keyword)
        {
            return new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("keyword", keyword) });
        }
    }
}
