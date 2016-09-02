using System;

namespace CinderellaGirlsCardViewer.Models
{
    public class Character
    {
        public string Name { get; set; }

        public CharacterType Type { get; set; }

        public Uri CoverUrl { get; set; }

        public Uri CardsEntry { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
