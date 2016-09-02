using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using CsQuery;
using Newtonsoft.Json.Linq;

namespace CinderellaGirlsCardViewer.Models
{
    internal static class GalleryHelper
    {
        public static IEnumerable<Character> GetCharacters(this CQ page)
        {
            var idols = page[".idol"];
            return idols.Map(idol =>
            {
                var nameDiv = idol.NextElementSibling.FirstChild;
                return new Character
                {
                    Name = nameDiv.InnerHTML.HtmlDecode(),
                    Type = nameDiv.ClassName.ToEnum<CharacterType>(),
                    CoverUrl = new Uri(idol.Style["background"].GetImageUrl()),
                    CardsEntry = new Uri(idol.ParentNode.ParentNode.GetAttribute("href"))
                };
            });
        }

        public static IEnumerable<Card> GetCards(this CQ page, string idolName)
        {
            var idols = page[".idol"];
            return idols.Map(idol =>
            {
                var dom = CQ.Create(idol);
                var profile = JToken.Parse(dom.Find("input[name=profile]").Attr("data-profile"));
                var baseData = dom.Find("input[name=basedata]");

                return new Card(new CardInfo
                {
                    IdolName = idolName,
                    Kana = profile["kana"].Value<string>().UrlDecode(),
                    CardName = baseData.Attr("data-name").UrlDecode(),
                    Rarity = baseData.Attr("data-rarity").ToEnum<Rarity>(),
                    Type = baseData.Attr("data-attribute").ToEnum<CharacterType>(),
                    CardId = dom.Find("div").Css("background").GetCardId()
                });
            });
        }

        public static string GetNextPageUrl(this CQ page)
        {
            var nextPaging = page[".a_link[accesskey=#]"];
            return nextPaging.Length > 0 ? nextPaging.Attr("href") : null;
        }

        private static readonly Regex UrlRegex = new Regex(@"https?://[^()]*");
        private static readonly Regex CardIdRegex = new Regex(@"/([^/]*)\.jpg");

        private static string GetImageUrl(this string str)
        {
            return UrlRegex.Match(str).Value;
        }

        private static string GetCardId(this string str)
        {
            return CardIdRegex.Match(str.UrlDecode()).Groups[1].Value;
        }

        private static string HtmlDecode(this string str)
        {
            return WebUtility.HtmlDecode(str);
        }

        private static string UrlDecode(this string str)
        {
            return WebUtility.UrlDecode(str);
        }
    }
}
