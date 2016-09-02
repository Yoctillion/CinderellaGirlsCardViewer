namespace CinderellaGirlsCardViewer.Models
{
    public class CardInfo
    {
        internal CardInfo() { }

        public string IdolName { get; internal set; }

        public string Kana { get; internal set; }

        public string CardName { get; internal set; }

        public Rarity Rarity { get; internal set; }

        public string RarityString => this.Rarity + (this.CardName.Contains("+") ? "+" : string.Empty);

        public CharacterType Type { get; internal set; }

        internal string CardId { get; set; }
    }
}
