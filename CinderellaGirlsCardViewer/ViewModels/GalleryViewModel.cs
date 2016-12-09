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
        private readonly MobageClient _client;
        private readonly CharacterClient _characterClient;

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

        public ObservableCollection<Card> Cards { get; } = new ObservableCollection<Card>();

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
            this._client = new MobageClient();
            this._client.LoadCookie();
            this._characterClient = new CharacterClient { Client = this._client };
            //this._characterClient.Update();
            this._cardsCache = new SimpleCache<Character, Card[]>();
        }

        public async Task QueryCharacter(string queryString)
        {
            this.IsQuerying = true;

            this.SelectedCharacter = null;
            this.Characters.Clear();
            this._cardsCache.Clear();

            await this.Characters.AddRange(await this._characterClient.Search(queryString), TimeSpan.FromSeconds(0.01));

            this.IsQuerying = false;
        }

        //private readonly SimpleTaskManager _getCardsTaskManager = new SimpleTaskManager();

        private Task<Card[]> _currentTask;

        public void GetCards(Character character)
        {
            this.Cards.Clear();

            if (character != null)
            {
                var task = this._cardsCache.GetAsync(character, async c => (await this._client.GetCardsOfCharacter(c)).ToArray());
                this._currentTask = task;

                this._currentTask.ContinueWith(async t =>
                {
                    if (this._currentTask == task)
                    {
                        await this.Cards.AddRange(t.Result, TimeSpan.FromSeconds(0.01));
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public async Task SaveImage(Uri url, string path)
        {
            await this._client.Download(url, path);
        }
    }
}
