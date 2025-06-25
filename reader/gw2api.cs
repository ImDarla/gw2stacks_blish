using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.Http;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace gw2stacks_blish.reader
{
    
	class Gw2Api
    {
     
		Gw2ApiManager manager;

        public Gw2Api(Gw2ApiManager manager_)
        {
			this.manager = manager_;
		}

		//depracted function for the multi account feature
		private async Task<Account> get_account_name()
		{
			var response = await this.manager.Gw2ApiClient.V2.Account.GetAsync();
			return response;
		}

		public async Task<IApiV2ObjectList<AccountLegendaryArmory>> get_legendary_armory()
		{
			var response = await this.manager.Gw2ApiClient.V2.Account.LegendaryArmory.GetAsync();
			return response;
		}

		public async Task<IApiV2ObjectList<AccountItem>> shared_inventory()
		{
			var response = await this.manager.Gw2ApiClient.V2.Account.Inventory.GetAsync();//shared inventory
			return response;
		}

		//depracted function for the multi account feature
		public async Task<CharactersInventory> character_inventory(string name_)
		{
			var response = await this.manager.Gw2ApiClient.V2.Characters[name_].Inventory.GetAsync();
			return response;
		}

		public async Task<IApiV2ObjectList<AccountItem>> bank()
		{
			var response = await this.manager.Gw2ApiClient.V2.Account.Bank.GetAsync()	;//bank inventory
			return response;
		}

		public async Task<IApiV2ObjectList<AccountMaterial>> material_storage()
		{
			var response = await this.manager.Gw2ApiClient.V2.Account.Materials.GetAsync();//materials
			return response;
		}

		public async Task<IApiV2ObjectList<Character>> characters()
		{
			var response = await this.manager.Gw2ApiClient.V2.Characters.AllAsync();//characters
			return response;
		}

		public  async Task<CommercePrices> item_price(int id_)
		{
			var response = await this.manager.Gw2ApiClient.V2.Commerce.Prices.GetAsync(id_);//price of specific ID
			return response;
		}

		public async Task<IReadOnlyList<CommercePrices>> item_prices(List<int> ids_)
		{
			var response = await this.manager.Gw2ApiClient.V2.Commerce.Prices.ManyAsync(ids_);//prices of specific IDs
			return response;
		}

		public async Task<IReadOnlyList<int>> item_price_ids()
		{
			var response = await this.manager.Gw2ApiClient.V2.Commerce.Prices.IdsAsync();
			return response;
		}

		public async Task<Itemstat> item_information(int id_)
		{
			var response = await this.manager.Gw2ApiClient.V2.Itemstats.GetAsync(id_);//stats of specific ID
			
			return response;
		}

		public async Task<IReadOnlyList<Item>> item_information_bulk(List<int> ids_)
		{
			var response = await this.manager.Gw2ApiClient.V2.Items.ManyAsync(ids_);//stats of specific IDs
			return response;
		}

		public async Task<IApiV2ObjectList<int>> recipe_ids()
		{
			var response = await this.manager.Gw2ApiClient.V2.Recipes.IdsAsync();
			return response;
		}

		public async Task<IReadOnlyList<Recipe>> recipes(List<int> ids_)
		{
			var response = await this.manager.Gw2ApiClient.V2.Recipes.ManyAsync(ids_);
			return response;
		}
	}
}
