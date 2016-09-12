using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace CinderellaGirlsCardViewer.Models
{
    public class CharacterPageLoader
    {
        public IEnumerable<Character> Characters { get; private set; }

        private readonly HttpClient _client;

        private string _nextPageUrl;


        internal CharacterPageLoader(HttpClient client, string keyword)
        {
            const string queryUrl =
                "http://sp.pf.mbga.jp/12008305/?guid=ON&url=http%3A%2F%2F125.6.169.35%2Fidolmaster%2Fgallery%2Findex%2F0%2F0%2F%3Fkeyword%3D";

            this._client = client;
            this._nextPageUrl = queryUrl + WebUtility.UrlEncode(keyword);
        }

        public async Task<bool> LoadNextPage()
        {
            if (this._nextPageUrl == null) return false;

            CQ page = await this._client.GetStringAsync(this._nextPageUrl);
            this.Characters = page.GetCharacters();
            this._nextPageUrl = page.GetNextPageUrl();

            return true;
        }
    }
}
