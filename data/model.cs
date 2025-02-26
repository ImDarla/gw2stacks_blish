//using Blish_HUD.PersistentStore;
using Gw2Sharp.WebApi.V2.Models;
using reader;
using shakr.gw2stacks_blish.data;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace data
{
    class model
    {
        Dictionary<int, Item> items;
        Dictionary<string, int> material_storage_size;
        List<Recipe> recipes;
        Dictionary<int, Item> recipe_results;
        public bool include_consumables;
        int ecto_salvage_price;
        List<string> accounts;
        Magic magicValues;

		public model (string path)
		{
			this.items = new Dictionary<int, Item>();
			this.material_storage_size = new Dictionary<string, int>();
			this.recipes = new List<Recipe>();
			this.recipe_results = new Dictionary<int, Item>();
			this.include_consumables = false;
			this.ecto_salvage_price = 0;
			this.accounts = new List<string>();
			this.magicValues = new Magic(path);
		}

        public void add_item(int id, bool account_bound, Source source)
        {
            if(this.items.ContainsKey(id)==false)
            {
                this.items.Add(id, new Item(id));
				this.items[id].add(source);
				this.items[id].account_bound = account_bound;
			}
            else//item already in collection
            {
                foreach(Source current in this.items[id].Sources)//condense sources
                {
                    //check if sources are on the same character and can be stacked (account bound and non account bound are not stackable
                    if(current.place==source.place&&current.account==source.account&&account_bound==this.items[id].account_bound)
                    {
                        current.count += source.count;
                        return;
                    }
					
				}
				this.items[id].add(source);
				this.items[id].account_bound = account_bound;
			}
            
        }

        public bool has_item(int id)
        {
            if(this.items.ContainsKey(id)&&this.items[id]?.total_count()>0)
            {
                return true;
            }
            return false;
        }

        public bool has_item(List<int> ids)
        {
            bool result = false;
			foreach (var item in ids)
			{
                result |= this.has_item(item);
			}
            return result;
		}

        public void build_material_storage_size(gw2api api)
        {
            var task = api.material_storage();
            task.Wait();
            UInt64 max_count = 0;
			foreach (var item in task.Result)
			{
                max_count = Math.Max(max_count, (UInt64)item.Count);
			}

            this.material_storage_size[api.name] = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(max_count / 250)) * 250);
		}

        public void  build_inventory(gw2api api)
        {
            UInt64 empty_slots = 0;
            //Message loading characters @name
            var task_characters = api.characters();
            var task_materials = api.material_storage();
            var task_bank = api.bank();
            var task_shared = api.shared_inventory();
			task_characters.Wait();
            foreach(var character in task_characters.Result)
            {
                //Message loading character @name inventory 
                
                foreach(var bag in character.Bags)
                {
                    if(bag!=null)
                    {
						foreach (var item in bag?.Inventory)
						{
							if (item==null)
							{
                                empty_slots++;
							}
                            else
                            {
                                this.add_item(item.Id, item.Binding.Value == ItemBinding.Account, new Source(Convert.ToUInt64(item.Count), character.Name, api.name));
                            }
						}
					}
                    
                }
            }

            task_materials.Wait();
			foreach(var item in task_materials.Result)
            {
				this.add_item(item.Id, item.Binding.Value == ItemBinding.Account, new Source(Convert.ToUInt64(item.Count), "Material Storage", api.name));
			}

            task_bank.Wait();
            foreach(var item in task_bank.Result)
            {
                if(item==null)
                {
                    empty_slots++;
                }
                else
                {
					this.add_item(item.Id, item.Binding.Value == ItemBinding.Account, new Source(Convert.ToUInt64(item.Count), "Bank Storage", api.name));
				}
            }

            task_shared.Wait();
			foreach (var item in task_shared.Result)
			{
				if (item == null)
				{
					empty_slots++;
				}
				else
				{
					this.add_item(item.Id, item.Binding.Value == ItemBinding.Account, new Source(Convert.ToUInt64(item.Count), "Shared Storage", api.name));
				}
			}
		}

        

		public void build_basic_item_info(Item item, Gw2Sharp.WebApi.V2.Models.Item info)
		{
            item.name = info.Name;
            item.icon = info.Icon;
            item.rarity = info.Rarity;
            item.description = info.Description;
            //Details can not be gotten from GW2sharp api
            var urlName = item.name.Replace(" ", "_");
            item.wiki_link = $"wiki.guildwars2.com/wiki/{urlName}";
		}

		public void build_recipe_info(gw2api api)
        {
            var task = api.recipes(this.items.Keys.ToList());
            
            List<int> output_item_ids = new List<int>();
            task.Wait();
            var result = task.Result;
			foreach (var recipe in result)
			{
				if(magicValues.pertinentRecipeTypes.Contains(recipe.Type))
                {
                    bool valid = true;
					foreach (var ingredient in recipe.Ingredients)
					{
                        int id = ingredient.ItemId;
						if (this.has_item(id)==false || (this.has_item(id)&&this.items?[id].total_count()<Convert.ToUInt64(ingredient.Count)))
                        {
                            valid = false;
                            break;
                        }
					}
                    if(valid==true)
                    {
                        this.recipes.Add(recipe);
                        output_item_ids.Add(recipe.OutputItemId);
                    }
				}
			}

            this.recipe_results = new Dictionary<int, Item>();

            var task_output = api.item_information_bulk(output_item_ids);
            task_output.Wait();

			foreach (var itemInformation in task_output.Result)
			{
                Item item = new Item(itemInformation.Id);
                this.build_basic_item_info(item, itemInformation);
                this.recipe_results.Add(item.item_id, item);
			}

			

            
		}

        public void build_item_info(gw2api api)
        {
            List<int> appraise_item_id = new List<int>();
            
            
            var task = api.item_information_bulk(this.items.Keys.ToList());
            task.Wait();
			foreach (var itemInformation in task.Result)
			{
                var item = this.items[itemInformation.Id];
                this.build_basic_item_info(item, itemInformation);
                bool salvagable = true;
                if(magicValues.nonStackableTypes.Contains(itemInformation.Type)==false|| (this.include_consumables && (itemInformation.Type == ItemType.Consumable)))//details can again not be querried via the GW2sharp api
                {
                    item.stackable = true;
                }

				foreach (var flag in itemInformation.Flags)
				{
                    if(flag ==ItemFlag.NoSalvage)
                    {
                        salvagable = false;
                    }
					if(flag == ItemFlag.SoulbindOnAcquire)
                    {
                        item.stackable = false;
                        
                    }
				}

				if(itemInformation.Description!=null)
                {
                    if(itemInformation.Description== "This item only has value as part of a collection.")
                    {
                        item.deletable = true;
                    }

				}

                if(magicValues.salvagableEquipment.Contains(itemInformation.Type)&&itemInformation.Rarity==ItemRarity.Rare&&salvagable&&itemInformation.Level>77)
                {
                    item.rare_for_salvage = true;
                    appraise_item_id.Add(itemInformation.Id);
                }


			}
            //loading market prices
            var task_prices = api.item_prices(appraise_item_id);
            task_prices.Wait();
			foreach (var price in task_prices.Result)
			{
                this.items[price.Id].price = price.Sells.UnitPrice;
			}
		}

        public void build_ecto_price(gw2api api)
        {
            var task = api.item_price(magicValues.ectoId);
            task.Wait();
            int ectoPrice = task.Result.Sells.UnitPrice;
            ;//seemingly from mystic salvage kit??
           
            this.ecto_salvage_price = Convert.ToInt32((ectoPrice * magicValues.tax * magicValues.ectoChance - magicValues.salvagePrice) / magicValues.tax);
        }

        public List<ItemForDisplay> get_stacks_advice()
        {
            List<ItemForDisplay> result = new List<ItemForDisplay>();
            var filter = this.items.Values.Where(list_item => list_item.get_advice_stacks(this.material_storage_size).Count > 0);
			foreach (var item in filter)
			{
                result.Add(new ItemForDisplay(item, item.get_advice_stacks(this.material_storage_size)));
			}
            return result;
		}

		public List<ItemForDisplay> get_vendor_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.rarity==ItemRarity.Junk);
			foreach (var item in filter)
			{
				result.Add(new ItemForDisplay(item, null));
			}
			return result;
		}

		public List<ItemForDisplay> get_rare_salvage_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.rare_for_salvage);
			foreach (var item in filter)
			{
                if(item.price!=null)
                {
                    if(item.price>this.ecto_salvage_price)
                    {
						result.Add(new ItemForDisplay(item, null, "Salvage!"));
					}
                    else
                    {
                        if(item.account_bound!=false)
                        {
							result.Add(new ItemForDisplay(item,null, "Sell!"));
						}
                    }
                
                }

				
			}
			return result;
		}

		public List<ItemForDisplay> get_craft_luck_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
            
			foreach (var id in magicValues.luckIds)
			{
				foreach(var account in this.accounts)
                {
                    if(this.has_item(id)&&this.items[id].total_count(account)>250)
                    {
                        result.Add(new ItemForDisplay(this.items[id], this.items[id].get_Sources_for_account(account)));
                    }
                }
			}
			
			return result;
		}

		public List<ItemForDisplay> get_just_delete_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.deletable);
			foreach (var item in filter)
			{
				result.Add(new ItemForDisplay(item));
			}
			return result;
		}

		public List<ItemForDisplay> get_just_salvage_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.description=="Salvage Item"&&list_item.item_id!=magicValues.ectoId);
			foreach (var item in filter)
			{
				result.Add(new ItemForDisplay(item, null, "Salvage this item"));
			}
			return result;
		}

		public List<ItemForDisplay> get_play_to_consume_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var id in magicValues.gameplayConsumables.Keys)
			{
				if(this.has_item(id))
                {
                    result.Add(new ItemForDisplay(this.items[id], null, magicValues.gameplayConsumables[id]));

				}
			}
			return result;
		}

		public List<ItemForDisplay> get_gobbler_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var gobbler in magicValues.gobblers)
			{
				if(this.has_item(gobbler.itemId)&&this.has_item(gobbler.food))
                {
					foreach (var account in this.accounts)
					{
						foreach (var food in gobbler.food)
						{
							if (this.items[food].total_count(account)>Convert.ToUInt64(this.material_storage_size[account]))
                            {
                                result.Add(new ItemForDisplay(this.items[food], this.items[food].get_Sources_for_account(account)));

							}
						}
						
					}
				}
			}
			return result;
		}

		public List<ItemForDisplay> get_misc_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var advice in magicValues.miscAdvices)
			{
				if (this.has_item(advice.itemId)&&this.items[advice.itemId].total_count()>=Convert.ToUInt64(advice.minCount))
				{
					result.Add(new ItemForDisplay(this.items[advice.itemId], null, advice.advice));

				}
			}
			return result;
		}

		public List<ItemForDisplay> get_karma_consumables_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var item in magicValues.karmaIds)
			{
				if(this.has_item(item)&&this.items[item].total_count()>0)
                {
					result.Add(new ItemForDisplay(this.items[item], null, "Consume for karma"));
				}
			}
			return result;
		}

		public List<ItemForDisplay> get_living_world_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var item in magicValues.lws3Id)
			{
				foreach (var account in this.accounts)
				{
					if(this.has_item(item)&&this.items[item].total_count(account)>Convert.ToUInt64(this.material_storage_size[account]))
					{
						result.Add(new ItemForDisplay(this.items[item], this.items[item].get_Sources_for_account(account), "Consume for unbound magic"));
					}
				}
			}

			foreach (var item in magicValues.lws4Id)
			{
				foreach (var account in this.accounts)
				{
					if (this.has_item(item) && this.items[item].total_count(account) > Convert.ToUInt64(this.material_storage_size[account]))
					{
						result.Add(new ItemForDisplay(this.items[item], this.items[item].get_Sources_for_account(account), "Consume for volatile magic"));
					}
				}
			}

			foreach (var item in magicValues.ibsId)
			{
				foreach (var account in this.accounts)
				{
					if (this.has_item(item) && this.items[item].total_count(account) > Convert.ToUInt64(this.material_storage_size[account]))
					{
						result.Add(new ItemForDisplay(this.items[item], this.items[item].get_Sources_for_account(account), "Convert to LS4 currency"));
					}
				}
			}
			return result;
		}

		public List<ItemForDisplay> get_crafting_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var recipe in this.recipes)
			{
				if(this.recipe_results.ContainsKey(recipe.OutputItemId)==false)
				{
					continue;
				}

				bool hasAccountBoundIngredients = false;

				foreach (var ingredient in recipe.Ingredients)
				{
					if(this.items[ingredient.ItemId].account_bound)
					{
						hasAccountBoundIngredients = true;
						break;
					}
				}

				if(hasAccountBoundIngredients)
				{
					foreach (var account in this.accounts)
					{
						bool canCraft = true;
						bool hasMoreThanStackIngredient = false;
						foreach (var ingredient in recipe.Ingredients)
						{
							if (this.items[ingredient.ItemId].total_count(account) < Convert.ToUInt64(ingredient.Count))
							{
								canCraft = false;
							}

							if (this.items[ingredient.ItemId].total_count(account) > Convert.ToUInt64(250))
							{
								hasMoreThanStackIngredient = true;
							}

						}
						if (canCraft && hasMoreThanStackIngredient)
						{
							result.Add(new ItemForDisplay(this.recipe_results[recipe.OutputItemId], null, "Craft"));
						}
					}
				}
				else
				{
					bool canCraft = true;
					bool hasMoreThanStackIngredient = false;
					foreach (var ingredient in recipe.Ingredients)
					{
						if (this.items[ingredient.ItemId].total_count() < Convert.ToUInt64(ingredient.Count))
						{
							canCraft = false;
						}

						if (this.items[ingredient.ItemId].total_count() > Convert.ToUInt64(250))
						{
							hasMoreThanStackIngredient = true;
						}

					}
					if (canCraft && hasMoreThanStackIngredient)
					{
						result.Add(new ItemForDisplay(this.recipe_results[recipe.OutputItemId], null, "Craft"));
					}
				}
			}
			return result;
		}
	}
}
