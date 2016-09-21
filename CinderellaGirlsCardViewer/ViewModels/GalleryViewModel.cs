using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CinderellaGirlsCardViewer.Models;
using CsQuery.ExtensionMethods.Internal;

namespace CinderellaGirlsCardViewer.ViewModels
{
    public class GalleryViewModel : NotificationObject
    {
        private readonly CharacterClient _client;

        #region Characters

        public ObservableCollection<Character> Characters { get; } = new ObservableCollection<Character>();

        #endregion

        #region SelectedCharacter

        private Character _selectedCharacter;

        public Character SelectedCharacter
        {
            get { return this._selectedCharacter; }
            set
            {
                if (this._selectedCharacter != value)
                {
                    this._selectedCharacter = value;
                    this.OnPropertyChanged();
                    this.GetCards(value);
                }
            }
        }

        #endregion

        #region Cards

        private Card[] _cards;

        public Card[] Cards
        {
            get { return this._cards; }
            private set
            {
                if (this._cards != value)
                {
                    this._cards = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        #region SelectedCard

        private Card _selectedCard;

        public Card SelectedCard
        {
            get { return this._selectedCard; }
            set
            {
                if (this._selectedCard != value)
                {
                    this._selectedCard = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        #region  IsQuerying

        private bool _isQuerying;

        public bool IsQuerying
        {
            get { return this._isQuerying; }
            private set
            {
                if (this._isQuerying != value)
                {
                    this._isQuerying = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        private readonly SimpleCache<Character, Card[]> _cardsCache;

        public GalleryViewModel()
        {
            this._client = new CharacterClient();
            this._client.LoadCookie();
            this._cardsCache = new SimpleCache<Character, Card[]>();
        }

        public async Task QueryCharacter(string queryString)
        {
            this.IsQuerying = true;

            this.SelectedCharacter = null;
            this.Characters.Clear();
            this._cardsCache.Clear();

            var pages = this._client.Query(queryString);
            while (await pages.LoadNextPage())
            {
                this.Characters.AddRange(pages.Characters);
            }

            if (Characters.Count == 1)
            {
                this.SelectedCharacter = this.Characters[0];
            }

            this.IsQuerying = false;
        }

        private readonly SimpleTaskManager _getCardsTaskManager = new SimpleTaskManager();

        public void GetCards(Character character)
        {
            this._getCardsTaskManager.Do(new SpecialTask<Card[]>
            {
                BeforeCancelling = () =>
                {
                    this.Cards = null;
                    return character != null
                        ? this._cardsCache.Get(character, c => this._client.GetCardsOfCharacter(c).Result.ToArray())
                        : null;
                },
                IfNotCancelled = cards => this.Cards = cards
            });
        }

        public async Task SaveImage(Uri url, string path)
        {
            await this._client.Download(url, path);
        }
    }
}
