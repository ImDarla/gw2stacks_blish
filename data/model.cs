﻿//using Blish_HUD.PersistentStore;
using Gw2Sharp.WebApi.V2.Models;
using gw2stacks_blish.reader;
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
using Microsoft.Xna.Framework.Graphics;
using Blish_HUD;
using System.IdentityModel;

namespace gw2stacks_blish.data
{
    class Model
    {
        public Dictionary<int, Item> items;
        int materialStorageSize;
        List<RecipeInfo> craftableRecipes;
        Dictionary<int, Item> recipeResults;
		private List<int> appraisedItemIds;
        public bool includeConsumables;
        int ectoSalvagePrice;
		public bool validData;
		private Logger log;
		private Dictionary<int, List<Source>> upgradableBags;

		public void reset_state()
		{
			foreach (var item in this.items)
			{
				item.Value.sources.Clear();
			}
			this.materialStorageSize = 0;
			this.craftableRecipes = new List<RecipeInfo>();
			this.recipeResults = new Dictionary<int, Item>();
			this.appraisedItemIds = new List<int>();
			this.includeConsumables = true;
			this.ectoSalvagePrice = 0;
			this.upgradableBags = new Dictionary<int, List<Source>>();
			this.validData = false;
		}

		public Model (Logger log_)
		{
			this.log = log_;
			this.items = new Dictionary<int, Item>();
			
			this.reset_state();
		}

		public async Task setup(Gw2Api api_)
		{
			this.validData = false;
			log.Info("started building ecto price");
			await this.build_ecto_price(api_);
			log.Info("started building inventory");
			await this.build_inventory(api_);
			log.Info("started building recipes");
			await this.build_recipe_info();
			log.Info("started building prices");
			await this.build_item_prices(api_);
			Magic.inventoryBag.build_basic_item_info();
			this.validData = true;
		}

		#region build backend
		public void add_item(int id_, bool isAccountBound_, bool isCharacterBound_, Source source_)
        {
            if(this.items.ContainsKey(id_)==false)
            {
				
                this.items.Add(id_, new Item(id_, isCharacterBound_, isAccountBound_));
			}
			this.items[id_].add_source(source_);

			if (this.items[id_].isRareForSalvage==true)
			{
				this.appraisedItemIds.Add(id_);
			}
			else
			{
				this.items[id_].price = 0;
			}
			

		}

			

        public bool has_item(int id_)
        {
            if(this.items.ContainsKey(id_))
            {
				if(this.items[id_]?.total_count() > 0)
				{
					return true;
				}
            }
            return false;
        }

        public bool has_item(List<int> ids_)
        {
            bool result = false;
			foreach (var item in ids_)
			{
                result |= this.has_item(item);
			}
            return result;
		}


		

		public async Task build_inventory(Gw2Api api_)
		{
			
			UInt64 emptySlots = 0;
			
			foreach (var character in await api_.characters())
			{
                foreach(var bag in character.Bags)
                {
                    if(bag!=null)
                    {
						if(bag.Size<32)
						{
							if(this.upgradableBags.ContainsKey(bag.Size)==true)
							{
								bool found = false;
								foreach (var source in this.upgradableBags[bag.Size])
								{
									if(source.place==character.Name)
									{
										source.count++;
										found = true;
									}
								}
								if(found==false)
								{
									this.upgradableBags[bag.Size].Add(new Source(1, character.Name));
								}
							}
							else
							{
								this.upgradableBags.Add(bag.Size, new List<Source> { new Source(1, character.Name) });
							}
						}
						foreach (var item in bag?.Inventory)
						{
							if (item==null)
							{
                                emptySlots++;
							}
                            else
                            {
								bool accountBound = false;
								bool characterBound = false;
								if(item.Binding!=null)
								{
									if(item.Binding==ItemBinding.Account)
									{
										accountBound = true;
									}
									if(item.Binding==ItemBinding.Character)
									{
										accountBound = true;
										characterBound = true;
									}
								}

                                this.add_item(item.Id, accountBound, characterBound, new Source(Convert.ToUInt64(item.Count), character.Name));
                            }
						}
					}
                    
                }
            }

			UInt64 maxCount = 0;
			//get items from material storage and set material storage max size
			foreach (var item in await api_.material_storage())
			{
				bool accountBound = false;
				bool characterBound = false;
				if (item.Binding != null)
				{
					if (item.Binding == ItemBinding.Account)
					{
						accountBound = true;
					}
					if (item.Binding == ItemBinding.Character)
					{
						accountBound = true;
						characterBound = true;
					}
				}
				this.add_item(item.Id, accountBound, characterBound, new Source(Convert.ToUInt64(item.Count), ("Material Storage")));
				maxCount = Math.Max(maxCount, (UInt64)item.Count);
			}
			this.materialStorageSize = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(maxCount / 250)) * 250);
			foreach (var item in await api_.bank())
			{
                if(item==null)
                {
                    emptySlots++;
                }
                else
                {
					bool accountBound = false;
					bool characterBound = false;
					if (item.Binding != null)
					{
						if (item.Binding == ItemBinding.Account)
						{
							accountBound = true;
						}
						if (item.Binding == ItemBinding.Character)
						{
							accountBound = true;
							characterBound = true;
						}
					}
					this.add_item(item.Id, accountBound, characterBound, new Source(Convert.ToUInt64(item.Count), ("Bank Storage")));
				}
            }
			foreach (var item in await api_.shared_inventory())
			{
				if (item == null)
				{
					emptySlots++;
				}
				else
				{
					bool accountBound = false;
					bool characterBound = false;
					if (item.Binding != null)
					{
						if (item.Binding == ItemBinding.Account)
						{
							accountBound = true;
						}
						if (item.Binding == ItemBinding.Character)
						{
							accountBound = true;
							characterBound = true;
						}
					}
					this.add_item(item.Id, accountBound, characterBound, new Source(Convert.ToUInt64(item.Count), ("Shared Storage")));
				}
			}

			
		}

		

		public async Task build_item_prices(Gw2Api api_)
		{
			var fullIds = this.appraisedItemIds.Distinct().ToList();
			if(fullIds.Count>0)
			{
				foreach (var price in await api_.item_prices(fullIds))
				{
					if (this.items.ContainsKey(price.Id))
					{
						this.items[price.Id].price = price.Sells.UnitPrice;
					}
					if (this.recipeResults.ContainsKey(price.Id))
					{
						this.recipeResults[price.Id].price = price.Sells.UnitPrice;
					}
				}
			}
			
		}

		
		

		public async Task build_ecto_price(Gw2Api api_)
		{
			var task = await api_.item_price(Magic.ectoId);

			int ectoPrice = task.Sells.UnitPrice;
			//create ecto salvage price based on cost and taxes
			this.ectoSalvagePrice = Convert.ToInt32((ectoPrice * Magic.tax * Magic.ectoChance - Magic.salvagePrice) / Magic.tax);
        }

		public async Task build_recipe_info()
		{
			
			
			List<int> outputItemIds = new List<int>();
			this.craftableRecipes = new List<RecipeInfo>();

			//filter recipes depending on if all inputs are available and then add recipes and output ids to a list
			foreach (var recipe in Magic.jsonLut.recipeLut.Values)
			{
				if (Magic.is_pertinent_recipe((RecipeType)recipe.Type))
				{
					
					bool valid = true;
					foreach (var ingredient in recipe.Ingredients)
					{
						int id = ingredient.ItemId;
						if (this.has_item(id) == false || (this.has_item(id) && this.items?[id].total_count() < Convert.ToUInt64(ingredient.Count)))
						{
							valid = false;
							break;
						}
					}
					if (valid == true)
					{
						this.craftableRecipes.Add(recipe);
						outputItemIds.Add(recipe.OutputItemId);
						foreach (var item in recipe.Ingredients)
						{
							if(this.appraisedItemIds.Contains(item.ItemId)==false&&this.items[item.ItemId].isAccountBound==false)
							{
								this.appraisedItemIds.Add(item.ItemId);
							}
						}
					}
				}
			}

			this.recipeResults = new Dictionary<int, Item>();


			foreach (var id in outputItemIds)
			{
				Item item = new Item(id, false, false);
				
				if(this.recipeResults.ContainsKey(item.itemId)==false)
				{
					this.recipeResults.Add(item.itemId, item);
				}
				if(item.isAccountBound==false&&this.appraisedItemIds.Contains(item.itemId)==false)
				{
					this.appraisedItemIds.Add(item.itemId);
				}
				
			}
		}
		#endregion

		#region advice
		public List<ItemForDisplay> get_stacks_advice()
        {
            List<ItemForDisplay> result = new List<ItemForDisplay>();
            var filter = this.items.Values.Where(list_item => list_item.get_advice_stacks(this.materialStorageSize).Count > 0);
			foreach (var item in filter)
			{
				
				result.Add(new ItemForDisplay(item, item.get_advice_stacks(this.materialStorageSize), ("Combine these items into stacks")));
				
				
			}
            return result;
		}

		public List<ItemForDisplay> get_vendor_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.rarity==ItemRarity.Junk || (list_item.isSellable == true && list_item.isSalvagable == false && list_item.isDeletable==true));
			//var filter = this.items.Values.Where(list_item => list_item.rarity==ItemRarity.Junk || (list_item.isSellable == true && list_item.isSalvagable == false && list_item.VendorValue > 0&& list_item.isAccountBound==true&&list_item.rarity!=ItemRarity.Ascended&&list_item.rarity!=ItemRarity.Legendary));
			foreach (var item in filter)
			{
				result.Add(new ItemForDisplay(item, null, ("Sell these items to a vendor")));
			}
			return result;
		}

		public List<ItemForDisplay> get_rare_salvage_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.isRareForSalvage);
			foreach (var item in filter)
			{
				
				if (item.price < this.ectoSalvagePrice)
				{
					
					result.Add(new ItemForDisplay(item, null, ("Salvage these items")));
				}
				else
				{
					if (item.isAccountBound != false)
					{
						
						result.Add(new ItemForDisplay(item, null, ("Sell these items on the TP")));
					}
				}
				


			}
			return result;
		}

		public List<ItemForDisplay> get_craft_luck_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
            
			foreach (var id in Magic.luckIds)
			{
				if (this.has_item(id) && this.items[id].total_count() > 250)
				{
					result.Add(new ItemForDisplay(this.items[id], this.items[id].sources, ("Craft these items into higher luck tiers")));
				}
				
			}
			return result;
		}

		public List<ItemForDisplay> get_just_delete_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.isDeletable&&list_item.isSellable==false&&list_item.isSalvagable==false);
			foreach (var item in filter)
			{
				result.Add(new ItemForDisplay(item, null, ("Delete these items")));
			}
			return result;
		}

		public List<ItemForDisplay> get_just_salvage_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => Magic.salvageIds.Contains(list_item.itemId)&&list_item.itemId!=Magic.ectoId||(list_item.isDeletable==true&&list_item.isSalvagable==true));
			foreach (var item in filter)
			{
				result.Add(new ItemForDisplay(item, null, ("Salvage these items")));
			}
			return result;
		}

		public List<ItemForDisplay> get_play_to_consume_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var id in Magic.gameplayConsumables.Keys)
			{
				if(this.has_item(id))
                {
                    result.Add(new ItemForDisplay(this.items[id], null, Magic.gameplayConsumables[id]));

				}
			}
			return result;
		}

		public List<ItemForDisplay> get_gobbler_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var gobbler in Magic.gobblers)
			{
				if(this.has_item(gobbler.itemId)&&this.has_item(gobbler.food))
                {
					foreach (var food in gobbler.food)
					{
						if (this.items[food].total_count() > Convert.ToUInt64(this.materialStorageSize))
						{
							result.Add(new ItemForDisplay(this.items[food], this.items[food].sources, ("Feed these items to gobblers"), gobbler.itemId));

						}
					}

					
				}
			}
			return result;
		}

		public List<ItemForDisplay> get_misc_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var advice in Magic.miscAdvices)
			{
				if (this.has_item(advice.itemId)&&this.items[advice.itemId].total_count()>=Convert.ToUInt64(advice.minCount))
				{
					result.Add(new ItemForDisplay(this.items[advice.itemId], null, (advice.advice)));
					
				}
			}
			foreach (var slots in this.upgradableBags)
			{
				if(slots.Key<18)
				{
					result.Add(new ItemForDisplay(Magic.inventoryBag, slots.Value, "Upgrade these bags to 18 slots"));
				}
				if(this.has_item(83410))
				{
					if (slots.Key < 32 && this.items[83410].total_count() >= 12)
					{
						result.Add(new ItemForDisplay(Magic.inventoryBag, slots.Value, "Potentially replace these bags with boreal trunks"));
					}
				}
				
			}
			return result;
		}

		public List<ItemForDisplay> get_karma_consumables_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var item in Magic.karmaIds)
			{
				if(this.has_item(item)&&this.items[item].total_count()>0)
                {
					result.Add(new ItemForDisplay(this.items[item], null, ("Consume these items for karma")));
				}
			}
			return result;
		}

		public List<ItemForDisplay> get_living_world_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var item in Magic.lws3Id)
			{
				if (this.has_item(item) && this.items[item].total_count() > Convert.ToUInt64(this.materialStorageSize))
				{
					result.Add(new ItemForDisplay(this.items[item], this.items[item].sources, ("Consume these items for unbound magic")));
				}
				
			}

			foreach (var item in Magic.lws4Id)
			{
				if (this.has_item(item) && this.items[item].total_count() > Convert.ToUInt64(this.materialStorageSize))
				{
					result.Add(new ItemForDisplay(this.items[item], this.items[item].sources, ("Consume these items for volatile magic")));
				}
				
			}

			foreach (var item in Magic.ibsId)
			{

				if (this.has_item(item) && this.items[item].total_count() > Convert.ToUInt64(this.materialStorageSize))
				{
					result.Add(new ItemForDisplay(this.items[item], this.items[item].sources, ("Convert these items to LWS4 currency")));
				}
			}
			return result;
		}

		public List<ItemForDisplay> get_crafting_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			
			foreach (var recipe in this.craftableRecipes)
			{
				if(this.recipeResults.ContainsKey(recipe.OutputItemId)==false)
				{
					continue;
				}
				bool canCraft = true;
				bool hasMoreThanStackIngredient = false;
				List<IngredientSource> parsedIngredients = new List<IngredientSource>();
				List<string> parsedDisciplines = new List<string>();
				int cost = 0;
				int value = this.recipeResults[recipe.OutputItemId].price;
				foreach (var ingredient in recipe.Ingredients)
				{
					if (this.items[ingredient.ItemId].total_count() < Convert.ToUInt64(ingredient.Count))
					{
						canCraft = false;
					}

					if (this.items[ingredient.ItemId].total_count() > Convert.ToUInt64(this.materialStorageSize))
					{
						hasMoreThanStackIngredient = true;
					}
					cost += ingredient.Count * this.items[ingredient.ItemId].price;
					parsedIngredients.Add(new IngredientSource(Convert.ToUInt64(ingredient.Count), ingredient.ItemId));
				}
				foreach (var discipline in recipe.Disciplines)
				{
					parsedDisciplines.Add(Magic.get_local_discipline((CraftingDisciplineType)discipline));
				}

				if (canCraft && hasMoreThanStackIngredient&&cost<value)
				{
					result.Add(new ItemForDisplay(this.recipeResults[recipe.OutputItemId], new List<Source>{ new RecipeSource(parsedIngredients, parsedDisciplines) },  ("Craft these items")));
				}
				
				
			}
			return result;
		}
		#endregion
	}
}
