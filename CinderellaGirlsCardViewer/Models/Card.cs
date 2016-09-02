namespace CinderellaGirlsCardViewer.Models
{
    public class Card
    {
        public Card(CardInfo info)
        {
            this.Info = info;
            this.Large = new CardImage(info, CardImageType.Large);
            this.Medium = new CardImage(info, CardImageType.Medium);
            this.Small = new CardImage(info, CardImageType.Small);
            this.Avatar = new CardImage(info, CardImageType.Avatar);
            this.Slim = new CardImage(info, CardImageType.Slim);
            this.Quest = new CardImage(info, CardImageType.Quest);
            this.NoFrame = new CardImage(info, CardImageType.NoFrame);
        }

        public CardInfo Info { get; }

        public CardImage Large { get; }

        public CardImage Medium { get; }

        public CardImage Small { get; }

        public CardImage Avatar { get; }

        public CardImage Slim { get; }

        public CardImage Quest { get; }

        public CardImage NoFrame { get; }
    }
}
