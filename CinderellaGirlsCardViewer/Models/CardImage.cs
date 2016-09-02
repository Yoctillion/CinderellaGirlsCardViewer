using System;
using System.Collections.Generic;

namespace CinderellaGirlsCardViewer.Models
{
    public class CardImage
    {
        private const string UrlTemplate = "http://125.6.169.35/idolmaster/image_sp/card/{0}/{1}.jpg";
        private const string FileNameTemplate = "{0}_{1}.jpg";

        private static readonly Dictionary<CardImageType, string> TypeMap = new Dictionary<CardImageType, string>
        {
            [CardImageType.Large] = "l",
            [CardImageType.Medium] = "m",
            [CardImageType.Small] = "s",
            [CardImageType.Avatar] = "xs",
            [CardImageType.Slim] = "ls",
            [CardImageType.Quest] = "quest",
            [CardImageType.NoFrame] = "l_noframe"
        };

        public Uri Url { get; }

        internal string FileName { get; }

        internal CardImage(CardInfo info, CardImageType type)
        {
            var typeStr = TypeMap[type];
            this.Url = new Uri(string.Format(UrlTemplate, typeStr, info.CardId));
            this.FileName = string.Format(FileNameTemplate, info.CardName, typeStr);
        }
    }
}
