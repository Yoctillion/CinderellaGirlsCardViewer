using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinderellaGirlsCardViewer.Models
{
    public class CharacterClient
    {
        public MobageClient Client { get; set; }

        private List<Character> _allCharacters;

        public async Task Update()
        {
            this._allCharacters = (await this.Client.SearchAll()).ToList();
        }

        public async Task<IEnumerable<Character>> Search(string keyword)
        {
            if (this._allCharacters == null)
            {
                await this.Update();
            }

            keyword = keyword.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                return this._allCharacters.AsReadOnly();
            }
            var searchResult = await this.Client.Search(keyword);

            return this._allCharacters.Where(character => character.Name.Contains(keyword) || searchResult.Contains(character));
        }
    }
}
