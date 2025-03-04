//using Blish_HUD.PersistentStore;
using Gw2Sharp.WebApi.V2.Models;
using reader;
using data;
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

namespace data
{
    class Model
    {
        public Dictionary<int, Item> items;
        int materialStorageSize;
        List<Recipe> recipes;
        Dictionary<int, Item> recipeResults;
        public bool includeConsumables;
        int ectoSalvagePrice;
		public bool validData;
        Magic magicValues;

		public Model ()
		{
			this.items = new Dictionary<int, Item>();
			this.materialStorageSize = 0;
			this.recipes = new List<Recipe>();
			this.recipeResults = new Dictionary<int, Item>();
			this.includeConsumables = true;
			this.ectoSalvagePrice = 0;
			this.magicValues = new Magic();
			this.validData = false;
		}

		public async Task setup(Gw2Api api_)
		{
			this.validData = false;
			//await this.build_material_storage_size(api_);
			await this.build_inventory(api_);
			await this.build_item_info(api_);
			await this.build_ecto_price(api_);
			await this.build_recipe_info(api_);
			this.validData = true;
		}

		public void add_item(int id_, bool isAccountBound_, Source source_)
        {
            if(this.items.ContainsKey(id_)==false)
            {
                this.items.Add(id_, new Item(id_));
			}
            else//item already in collection
            {
                foreach(Source current in this.items[id_].sources)//condense sources
                {
                    //check if sources are on the same character and can be stacked (account bound and non account bound are not stackable
                    if(current.place==source_.place&&isAccountBound_==this.items[id_].isAccountBound)
                    {
                        current.count += source_.count;
                    }
				}
			}
			this.items[id_].add(source_);
			this.items[id_].isAccountBound = isAccountBound_;
		}

        public bool has_item(int id_)
        {
            if(this.items.ContainsKey(id_)&&this.items[id_]?.total_count()>0)
            {
                return true;
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
			
			foreach (var character in await api_.characters())//.Result
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
                                emptySlots++;
							}
                            else
                            {
                                this.add_item(item.Id, item.Binding?.Value == ItemBinding.Account, new Source(Convert.ToUInt64(item.Count), character.Name));
                            }
						}
					}
                    
                }
            }

			UInt64 maxCount = 0;
			
			//taskMaterials.Wait();
			foreach (var item in await api_.material_storage())
			{
				this.add_item(item.Id, item.Binding?.Value == ItemBinding.Account, new Source(Convert.ToUInt64(item.Count), "Material Storage"));
				maxCount = Math.Max(maxCount, (UInt64)item.Count);
			}
			this.materialStorageSize = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(maxCount / 250)) * 250);
			//taskBank.Wait();
			foreach (var item in await api_.bank())
			{
                if(item==null)
                {
                    emptySlots++;
                }
                else
                {
					this.add_item(item.Id, item.Binding?.Value == ItemBinding.Account, new Source(Convert.ToUInt64(item.Count), "Bank Storage"));
				}
            }

			//taskShared.Wait();
			foreach (var item in await api_.shared_inventory())
			{
				if (item == null)
				{
					emptySlots++;
				}
				else
				{
					this.add_item(item.Id, item.Binding?.Value == ItemBinding.Account, new Source(Convert.ToUInt64(item.Count), "Shared Storage"));
				}
			}
		}

        

		public void build_basic_item_info(Item item_, Gw2Sharp.WebApi.V2.Models.Item info_)
		{
			if(magicValues.luckNameMapping.ContainsKey(item_.itemId))
			{
				item_.name = magicValues.luckNameMapping[item_.itemId];
			}
			else
			{
				item_.name = info_.Name;
			}
				
            item_.icon = info_.Icon;
            item_.rarity = info_.Rarity;
            item_.description = info_.Description;
			if (info_.Type == ItemType.Consumable)
			{
				var temp = ((ItemConsumable)info_);
				if ((temp.Details.Type == ItemConsumableType.Food) || (temp.Details.Type == ItemConsumableType.Utility))
				{
					item_.isFoodOrUtility = true;
				}
			}
			
			var urlName = item_.name.Replace(" ", "_");
            item_.wikiLink = $"wiki.guildwars2.com/wiki/{urlName}";
		}

		public async Task build_recipe_info(Gw2Api api_)
		{
			var taskIds = await api_.recipe_ids();
			//taskIds.Wait();

           
            
            List<int> outputItemIds = new List<int>();
            
			foreach (var recipe in await api_.recipes(taskIds.ToList()))
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
                        outputItemIds.Add(recipe.OutputItemId);
                    }
				}
			}

            this.recipeResults = new Dictionary<int, Item>();


			foreach (var itemInformation in await api_.item_information_bulk(outputItemIds))
			{
                Item item = new Item(itemInformation.Id);
                this.build_basic_item_info(item, itemInformation);
                this.recipeResults.Add(item.itemId, item);
			}

			

            
		}

		public async Task build_item_info(Gw2Api api_)
		{
            List<int> appraisedItemIds = new List<int>();



			foreach (var itemInformation in await api_.item_information_bulk(this.items.Keys.ToList()))
			{
                var item = this.items[itemInformation.Id];
                this.build_basic_item_info(item, itemInformation);
                bool salvagable = true;
                if(magicValues.nonStackableTypes.Contains(itemInformation.Type)==false)//details can again not be querried via the GW2sharp api
                {
                    item.isStackable = true;
                }

				if(item.isFoodOrUtility)
				{
					item.isStackable = true;
				}

				foreach (var flag in itemInformation.Flags)
				{
                    if(flag ==ItemFlag.NoSalvage)
                    {
                        salvagable = false;
                    }
					if(flag == ItemFlag.SoulbindOnAcquire)
                    {
                        item.isStackable = false;
                        
                    }
				}

				if(itemInformation.Description!=null)
                {
                    if(itemInformation.Description== "This item only has value as part of a collection.")
                    {
                        item.isDeletable = true;
                    }

				}

                if(magicValues.salvagableEquipment.Contains(itemInformation.Type)&&itemInformation.Rarity==ItemRarity.Rare&&salvagable&&itemInformation.Level>77)
                {
                    item.isRareForSalvage = true;
                    appraisedItemIds.Add(itemInformation.Id);
                }


			}
			//loading market prices
			foreach (var price in await api_.item_prices(appraisedItemIds))
			{
                this.items[price.Id].price = price.Sells.UnitPrice;
			}
		}

		public async Task build_ecto_price(Gw2Api api_)
		{
			var task = await api_.item_price(magicValues.ectoId);

			int ectoPrice = task.Sells.UnitPrice;
			this.ectoSalvagePrice = Convert.ToInt32((ectoPrice * magicValues.tax * magicValues.ectoChance - magicValues.salvagePrice) / magicValues.tax);
        }

        public List<ItemForDisplay> get_stacks_advice()
        {
            List<ItemForDisplay> result = new List<ItemForDisplay>();
            var filter = this.items.Values.Where(list_item => list_item.get_advice_stacks(this.materialStorageSize).Count > 0);
			foreach (var item in filter)
			{
				if (this.includeConsumables)
				{
					result.Add(new ItemForDisplay(item, item.get_advice_stacks(this.materialStorageSize), "Combine these items into stacks"));
				}
				else
				{
					if (item.isFoodOrUtility == false)
					{
						result.Add(new ItemForDisplay(item, item.get_advice_stacks(this.materialStorageSize), "Combine these items into stacks"));
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
				result.Add(new ItemForDisplay(item, null, "Sell these items to a vendor"));
			}
			return result;
		}

		public List<ItemForDisplay> get_rare_salvage_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.isRareForSalvage);
			foreach (var item in filter)
			{
                if(item.price!=null)
                {
                    if(item.price>this.ectoSalvagePrice)
                    {
						result.Add(new ItemForDisplay(item, null, "Salvage these items"));
					}
                    else
                    {
                        if(item.isAccountBound!=false)
                        {
							result.Add(new ItemForDisplay(item,null, "Sell these items on the TP"));
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
				if (this.has_item(id) && this.items[id].total_count() > 250)
				{
					result.Add(new ItemForDisplay(this.items[id], this.items[id].sources, "Craft these items into higher luck tiers"));
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
				result.Add(new ItemForDisplay(item, null, "Delete these items"));
			}
			return result;
		}

		public List<ItemForDisplay> get_just_salvage_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			var filter = this.items.Values.Where(list_item => list_item.description=="Salvage Item"&&list_item.itemId!=magicValues.ectoId);
			foreach (var item in filter)
			{
				result.Add(new ItemForDisplay(item, null, "Salvage these items"));
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
					foreach (var food in gobbler.food)
					{
						if (this.items[food].total_count() > Convert.ToUInt64(this.materialStorageSize))
						{
							result.Add(new ItemForDisplay(this.items[food], this.items[food].sources, "Feed these items to gobblers"));

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
					result.Add(new ItemForDisplay(this.items[item], null, "Consume these items for karma"));
				}
			}
			return result;
		}

		public List<ItemForDisplay> get_living_world_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var item in magicValues.lws3Id)
			{
				if (this.has_item(item) && this.items[item].total_count() > Convert.ToUInt64(this.materialStorageSize))
				{
					result.Add(new ItemForDisplay(this.items[item], this.items[item].sources, "Consume these items for unbound magic"));
				}
				
			}

			foreach (var item in magicValues.lws4Id)
			{
				if (this.has_item(item) && this.items[item].total_count() > Convert.ToUInt64(this.materialStorageSize))
				{
					result.Add(new ItemForDisplay(this.items[item], this.items[item].sources, "Consume these items for volatile magic"));
				}
				
			}

			foreach (var item in magicValues.ibsId)
			{

				if (this.has_item(item) && this.items[item].total_count() > Convert.ToUInt64(this.materialStorageSize))
				{
					result.Add(new ItemForDisplay(this.items[item], this.items[item].sources, "Convert these items to LWS4 currency"));
				}
			}
			return result;
		}

		public List<ItemForDisplay> get_crafting_advice()
		{
			List<ItemForDisplay> result = new List<ItemForDisplay>();
			foreach (var recipe in this.recipes)
			{
				if(this.recipeResults.ContainsKey(recipe.OutputItemId)==false)
				{
					continue;
				}
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
					result.Add(new ItemForDisplay(this.recipeResults[recipe.OutputItemId], null, "Craft these items"));
				}

				
			}
			return result;
		}
	}
}
