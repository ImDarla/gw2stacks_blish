using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.Http;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace reader
{
    


    
	class Gw2Api
    {
       

        
		
		public string name;
		Gw2ApiManager manager;

        public Gw2Api(Gw2ApiManager manager_)
        {
			this.manager = manager_;
			//var task = account_name();
			
			this.name = "REMOVE ACCOUNT NAME"; //task.Result.Name;
			
		}

        public bool validate() 
        {
            try
			{
				if(manager.HasPermissions(new List<TokenPermission>{TokenPermission.Account, TokenPermission.Characters, TokenPermission.Inventories }))
				{
					
				}
				return true;
			}
			catch(Gw2Sharp.WebApi.Exceptions.InvalidAccessTokenException e)
			{
				return false;
			}
			catch(Gw2Sharp.WebApi.Exceptions.MissingScopesException e)
			{
				return false;
			}
			catch(Gw2Sharp.WebApi.Exceptions.RequestException e)
			{
				return false;
			}

        }

		public async Task set_name()
		{
			var result = await this.get_account_name();
			this.name = result.Name;
		}

		private async Task<Account> get_account_name()
		{
			var response = await this.manager.Gw2ApiClient.V2.Account.GetAsync();//shared inventory
			return response;
		}

		public async Task<IApiV2ObjectList<AccountItem>> shared_inventory()
		{
			var response = await this.manager.Gw2ApiClient.V2.Account.Inventory.GetAsync();//shared inventory
			return response;
		}

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
			var response = await this.manager.Gw2ApiClient.V2.Commerce.Prices.ManyAsync(ids_);//price of specific ID
			return response;
		}

		public async Task<Itemstat> item_information(int id_)
		{
			var response = await this.manager.Gw2ApiClient.V2.Itemstats.GetAsync(id_);//stats of specific ID
			
			return response;
		}

		public async Task<IReadOnlyList<Item>> item_information_bulk(List<int> ids_)
		{
			var response = await this.manager.Gw2ApiClient.V2.Items.ManyAsync(ids_);//stats of specific ID
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
