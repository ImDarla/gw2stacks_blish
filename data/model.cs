//using Blish_HUD.PersistentStore;
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
        

		public async Task reset_state()
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

			await this.reset_state();
			var ectoTask = this.build_ecto_price(api_);
			ectoTask.Wait();
			var task= this.build_inventory(api_);
			task.Wait();
			this.validData = true;
		}

		#region build backend
		public void add_item(int id_, bool isAccountBound_, bool isCharacterBound_, Source source_)
        {
            if(this.items.ContainsKey(id_)==false)
            {
                this.items.Add(id_, new Item(id_));
			}
			this.items[id_].add_source(source_);
			this.items[id_].isAccountBound = isAccountBound_;
			this.items[id_].isCharacterBound = isCharacterBound_;
			this.build_basic_item_info(this.items[id_], Magic.jsonLut.itemLut[id_]);
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


		//deprecated method, part of build_inventory
		public async Task build_material_storage_size(Gw2Api api_)
		{
			//task.Wait();
			UInt64 maxCount = 0;
			foreach (var item in await api_.material_storage()) //.Result
			{
				maxCount = Math.Max(maxCount, (UInt64)item.Count);
			}

			this.materialStorageSize = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(maxCount / 250)) * 250);
		}

		public async Task build_inventory(Gw2Api api_)
		{
			UInt64 emptySlots = 0;
			var characterTask = api_.characters();
			var materialTask = api_.material_storage();
			var bankTask = api_.bank();
			var sharedTask = api_.shared_inventory();

			characterTask.Wait();
			foreach (var character in characterTask.Result)
			{
                foreach(var bag in character.Bags)
                {
                    if(bag!=null)
                    {
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

			materialTask.Wait();
			//get items from material storage and set material storage max size
			foreach (var item in materialTask.Result)
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

			bankTask.Wait();
			foreach (var item in bankTask.Result)
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

			sharedTask.Wait();
			foreach (var item in sharedTask.Result)
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

			this.log.Warn("count: "+appraisedItemIds.Count.ToString());
			foreach (var price in await api_.item_prices(appraisedItemIds))
			{
				this.items[price.Id].price = price.Sells.UnitPrice;
				this.log.Warn("Item: " + price.Id + " Price: " + this.items[price.Id].price);
			}
		}

        

		public void build_basic_item_info(Item item_, ItemInfo info_)
		{
			//adjust name of Essence of luck items to include the rarity
			if(Magic.is_luck_essence(item_.itemId))
			{
				item_.name = Magic.get_string(item_.itemId);
			}
			else
			{
				item_.name = info_.Name;
			}
			
			item_.iconId = info_.IconId; 
			item_.rarity = (ItemRarity)info_.Rarity;
            item_.description = info_.Description;
			item_.isFoodOrUtility = info_.isFoodOrUtility;
			
			var urlName = item_.name.Replace(" ", "_");
            item_.wikiLink = $"wiki.guildwars2.com/wiki/{urlName}";

			bool salvagable = true;
			

			if (Magic.is_non_stackable_type((ItemType)info_.Type) == false)
			{
				item_.isStackable = true;
			}

			if (item_.isFoodOrUtility)
			{
				item_.isStackable = true;
			}

			foreach (var flag in info_.Flags)
			{
				if ((ItemFlag)flag == ItemFlag.NoSalvage)
				{
					salvagable = false;
				}
				if ((ItemFlag)flag == ItemFlag.SoulbindOnAcquire)
				{
					item_.isAccountBound = true;
					item_.isCharacterBound = true;
					item_.isStackable = false;

				}

			}


			if (Magic.collectionOnlyIds.Contains(info_.Id))
			{
				item_.isDeletable = true;
			}

			if (Magic.is_salvagable_equipment((ItemType)info_.Type) && (ItemRarity)info_.Rarity == ItemRarity.Rare && salvagable && info_.Level > 77)
			{
				item_.isRareForSalvage = true;
				this.appraisedItemIds.Add(item_.itemId);
				
			}

			

			item_.hasInformation = true;
		}

		

		public async Task build_item_info(Gw2Api api_)
		{
            List<int> appraisedItemIds = new List<int>();
			var filter = this.items.Keys;//.Where(id => this.items[id].hasInformation == false);
			foreach (var id in filter)
			{
				var item = this.items[id];
				this.build_basic_item_info(item, Magic.jsonLut.itemLut[id]);
				if (item.isRareForSalvage == true)
				{
					appraisedItemIds.Add(id);
				}
				else
				{
					if ((ItemType)Magic.jsonLut.itemLut[id].Type == ItemType.CraftingMaterial && item.isAccountBound == false && item.isCharacterBound == false)
					{
						appraisedItemIds.Add(id);
					}
				}

			}
			//loading market prices
			if (appraisedItemIds.Count() > 0)
			{
				foreach (var price in await api_.item_prices(appraisedItemIds))
				{
					//this.items[price.Id].price = price.Sells.UnitPrice;
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

		public void build_recipe_info()
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
					}
				}
			}

			this.recipeResults = new Dictionary<int, Item>();


			foreach (var id in outputItemIds)
			{
				Item item = new Item(id);
				this.build_basic_item_info(item, Magic.jsonLut.itemLut[id]);
				this.recipeResults.Add(item.itemId, item);
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
				if (this.includeConsumables)
				{
					result.Add(new ItemForDisplay(item, item.get_advice_stacks(this.materialStorageSize), ("Combine these items into stacks")));
				}
				else
				{
					if (item.isFoodOrUtility == false)
					{
						result.Add(new ItemForDisplay(item, item.get_advice_stacks(this.materialStorageSize), ("Combine these items into stacks")));
					}
				}
				
			}
            return result;
		}

		public List<ItemForDisplay> get_vendor_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.rarity==ItemRarity.Junk);
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
				result.Add(new ItemForDisplay(item, null, ("Salvage or sell these items on the TP")));
				/*
                if(item.price!=null)
                {
                    if(item.price>this.ectoSalvagePrice)
                    {
						result.Add(new ItemForDisplay(item, null, ("Salvage these items")));
					}
                    else
                    {
                        if(item.isAccountBound!=false)
                        {
							result.Add(new ItemForDisplay(item,null, ("Sell these items on the TP")));
						}
                    }
                
                }*/


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
			var filter = this.items.Values.Where(list_item => list_item.isDeletable);
			foreach (var item in filter)
			{
				result.Add(new ItemForDisplay(item, null, ("Delete these items")));
			}
			return result;
		}

		public List<ItemForDisplay> get_just_salvage_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => Magic.salvageIds.Contains(list_item.itemId)&&list_item.itemId!=Magic.ectoId);
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
							result.Add(new ItemForDisplay(this.items[food], this.items[food].sources, ("Feed these items to gobblers ("+gobbler.name+")")));

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
			this.build_recipe_info();
			foreach (var recipe in this.craftableRecipes)
			{
				if(this.recipeResults.ContainsKey(recipe.OutputItemId)==false)
				{
					continue;
				}
				bool canCraft = true;
				bool hasMoreThanStackIngredient = false;
				List<Source> parsedIngredients = new List<Source>();
				List<string> parsedDisciplines = new List<string>();
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
					parsedIngredients.Add(new Source(Convert.ToUInt64(ingredient.Count), this.items[ingredient.ItemId].name));
				}
				foreach (var discipline in recipe.Disciplines)
				{
					parsedDisciplines.Add(Magic.get_string((CraftingDisciplineType)discipline));
				}

				if (canCraft && hasMoreThanStackIngredient)
				{
					result.Add(new ItemForDisplay(this.recipeResults[recipe.OutputItemId], new List<Source>{ new RecipeSource(parsedIngredients, parsedDisciplines) },  ("Craft these items")));
				}
				
				
			}
			return result;
		}
		#endregion
	}
}
