using System;
using System.Collections.Generic;

namespace CinderellaGirlsCardViewer.Models
{
    public class CardImage
    {
        private const string UrlTemplate = "http://125.6.169.35/idolmaster/image_sp/{0}/{1}/{2}.jpg";
        private const string FileNameTemplate = "{0}_{1}.jpg";

        private static readonly Dictionary<CardSignType, string> SignMap = new Dictionary<CardSignType, string>
        {
            [CardSignType.None] = "card",
            [CardSignType.Platinum] = "card_sign_p",
            [CardSignType.Normal] = "card_sign_b"
        };

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

        internal CardImage(CardInfo info, CardImageType type, CardSignType sign = CardSignType.None)
        {
            var signStr = SignMap[sign];
            var typeStr = TypeMap[type];
            this.Url = new Uri(string.Format(UrlTemplate, signStr, typeStr, info.HashCardId));
            this.FileName = string.Format(FileNameTemplate, info.CardName, typeStr);
        }
    }
}
