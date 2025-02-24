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
    


    
	class gw2api
    {
       

        string api_key;
		Gw2Sharp.Connection connection;
        

        public gw2api(string api_key)
        {
            this.api_key = api_key;
			this.connection= new Gw2Sharp.Connection(this.api_key);
		}

        public void validate()
        {
            
        }

		public async Task<IApiV2ObjectList<AccountItem>> shared_inventory(string name)
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);
			var response = await client.WebApi.V2.Account.Inventory.GetAsync();//shared inventory
			client.Dispose();
			return response;
		}

		public async Task<CharactersInventory> character_inventory(string name)
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);

			var response = await client.WebApi.V2.Characters[name].Inventory.GetAsync();


			client.Dispose();
			return response;
		}

		public async Task<IApiV2ObjectList<AccountItem>> bank()
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);

			var response = await client.WebApi.V2.Account.Bank.GetAsync()	;//bank inventory


			client.Dispose();
			return response;
		}

		public async Task<IApiV2ObjectList<AccountMaterial>> materials()
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);
			
			var response = await client.WebApi.V2.Account.Materials.GetAsync();//materials
			client.Dispose();
			return response;
		}

		public async Task<IApiV2ObjectList<Character>> characters()
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);
			var response = await client.WebApi.V2.Characters.AllAsync();//characters
			client.Dispose();
			return response;
		}

		public  async Task<CommercePrices> item_price(int id)
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);
			var response = await client.WebApi.V2.Commerce.Prices.GetAsync(id);//price of specific ID
			client.Dispose();
			return response;
		}

		public async Task<Itemstat> item_information(int id)
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);
			var response = await client.WebApi.V2.Itemstats.GetAsync(id);//stats of specific ID
			client.Dispose();
			return response;
		}

		public async Task<IApiV2ObjectList<int>> recipe_ids()
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);
			var response = await client.WebApi.V2.Recipes.IdsAsync();
			client.Dispose();
			return response;
		}

		public async Task<IReadOnlyList<Recipe>> recipes(IApiV2ObjectList<int> ids)
		{
			var client = new Gw2Sharp.Gw2Client(this.connection);
			var response = await client.WebApi.V2.Recipes.ManyAsync(ids);
			client.Dispose();
			return response;
		}
	}
}
