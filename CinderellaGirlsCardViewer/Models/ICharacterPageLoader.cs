using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using CsQuery.EquationParser;

namespace CinderellaGirlsCardViewer.Models
{
    public interface ICharacterPageLoader
    {
        IEnumerable<Character> Characters { get; }
        Task<bool> LoadNextPage();
    }

    public static class PageLoaderHelper
    {
        private class GalleryPageLoader : ICharacterPageLoader
        {
            public IEnumerable<Character> Characters { get; private set; }

            private readonly HttpClient _client;

            private const string UrlTemplate = "http://sp.pf.mbga.jp/12008305/?guid=ON&url=http%3A%2F%2F125.6.169.35%2Fidolmaster%2Fidol_gallery%2Findex%2F0%2F1%2F2%2F{0}%3Fl_frm%3DIdol_gallery_1%26rnd%3D{1}";

            private int _count;

            private bool _isLastPage;

            internal GalleryPageLoader(HttpClient client)
            {
                this._client = client;
            }

            public async Task<bool> LoadNextPage()
            {
                if (this._isLastPage) return false;

                var url = string.Format(UrlTemplate, this._count, new Random().Next().ToString());

                var p = await this._client.GetStringAsync(url);
                CQ page = p;
                this.Characters = page.GetCharacters(ref this._count).ToList();

                var nextBtn = page[".btn_pager_next"];
                this._isLastPage = nextBtn.Length == 0 || nextBtn.HasClass("_hover");
                return true;
            }
        }

        private class SearchPageLoader : ICharacterPageLoader
        {
            public IEnumerable<Character> Characters { get; private set; }

            private readonly HttpClient _client;

            private readonly OrderedDictionary<string, string> _parameters;

            private const string UrlTemplate = "http://sp.pf.mbga.jp/12008305/?guid=ON&url=http%3A%2F%2F125.6.169.35%2Fidolmaster%2Fidol_gallery%2Findex%2F0%2F1%2F2%2F{0}%3Fl_frm%3DIdol_gallery_1%26rnd%3D{1}";
            // seems both GET and POST are OK : http://sp.pf.mbga.jp/12008305/?guid=ON&url=http%3A%2F%2F125.6.169.35%2Fidolmaster%2Fidol_gallery%2Findex%2F0%2F1%2F1%2F{0}%3Fname%3D{1}%26is_search%3D1%26l_frm%3DIdol_gallery_1%26rnd%3D{2}

            private int _count;

            private bool _isLastPage;

            internal SearchPageLoader(HttpClient client, string keyword)
            {
                keyword = keyword.Trim();
                this._parameters = new OrderedDictionary<string, string>
                {
                    ["name"] = keyword,
                    ["is_search"] = "1",
                };

                this._client = client;
            }

            public async Task<bool> LoadNextPage()
            {
                if (this._isLastPage) return false;

                var url = string.Format(UrlTemplate, this._count, new Random().Next().ToString());
                using (var response = await this._client.PostAsync(url, new FormUrlEncodedContent(this._parameters)))
                {
                    CQ page = await response.Content.ReadAsStringAsync();
                    this.Characters = page.GetCharacters(ref this._count).ToArray();

                    var nextBtn = page[".btn_pager_next"];
                    this._isLastPage = nextBtn.Length == 0 || nextBtn.HasClass("_hover");
                }
                return true;
            }
        }

        public static async Task<IEnumerable<Character>> SearchAll(this MobageClient client)
        {
            var c = client.GetClient();
            var pageLoader = new GalleryPageLoader(c);
            var result = await pageLoader.GetAll();
            c.Dispose();
            return result;
        }

        public static ICharacterPageLoader SearchAllPaging(this MobageClient client)
        {
            return new GalleryPageLoader(client.GetClient());
        }

        public static async Task<IEnumerable<Character>> Search(this MobageClient client, string keyword)
        {
            var c = client.GetClient();
            var pageLoader = new SearchPageLoader(c, keyword);
            var result = await pageLoader.GetAll();
            c.Dispose();
            return result;
        }

        private static async Task<IEnumerable<Character>> GetAll(this ICharacterPageLoader pageLoader)
        {
            var result = Enumerable.Empty<Character>();

            while (await pageLoader.LoadNextPage())
            {
                result = result.Concat(pageLoader.Characters);
            }

            return result;
        }
    }
}
