using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CsQuery;
using Newtonsoft.Json.Linq;

namespace CinderellaGirlsCardViewer.Models
{
    internal static class GalleryHelper
    {
        public static IEnumerable<Character> GetCharacters(this CQ page, ref int count)
        {
            var idols = page[".idol"];
            var nameSet = new HashSet<string>();
            count += idols.Length;
            return idols.Map(idol =>
                {
                    var nameDiv = idol.NextElementSibling.ChildNodes.First(e => string.Equals(e.NodeName, "div", StringComparison.OrdinalIgnoreCase));
                    return new Character
                    {
                        Name = nameDiv.InnerHTML.HtmlDecode(),
                        Type = nameDiv.ClassName.ToEnum<CharacterType>(),
                        CoverUrl = new Uri(idol.Style["background"].GetImageUrl()),
                        CardsEntry = new Uri(idol.ParentNode.ParentNode.GetAttribute("href"))
                    };
                })
                .Where(idol => nameSet.Add(idol.Name));
        }

        public static IEnumerable<Card> GetCards(this string page, string idolName)
        {
            var json = Regex.Match(page, @"idol\.detail_list = (.*);").Groups[1].Value;
            var cards = JToken.Parse(json);
            foreach (var card in cards)
            {
                var data = card["data"];
                var profile = card["profile"];
                yield return new Card(new CardInfo
                {
                    Id = profile["card_id"].Value<string>(),
                    IdolName = idolName,
                    Kana = profile["card_kana"].Value<string>(),
                    CardName = data["card_name"].Value<string>(),
                    Rarity = (Rarity)((data["rarity"].Value<int>() + 1) / 2),
                    Type = data["attribute"].Value<string>().ToEnum<CharacterType>(),
                    HashCardId = data["hash_card_id"].Value<string>()
                });
            }
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
