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

        public override bool Equals(object obj)
        {
            var other = obj as Character;
            if (other == null) return false;
            return this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
