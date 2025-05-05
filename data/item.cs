using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;

namespace gw2stacks_blish.data
{
    class Item
    {

        public int itemId;
        public List<Source> sources;
        public bool isCharacterBound;
		public bool isAccountBound;
		public string name;
		public string description;
        public int iconId;
        public ApiEnum<ItemRarity> rarity;
		public bool isStackable;
		public bool isDeletable;
		public bool isRareForSalvage;
        public string wikiLink;
        public int price;
        public bool isFoodOrUtility;
        public bool hasInformation;
		public int VendorValue;
		public bool isSellable;
		public bool isSalvagable;
		public string chatLink;

		public Item(int id_, bool isCharacterBound_, bool isAccountBound_, bool delayedCreate=false)
        {
            this.itemId = id_;
            this.sources = new List<Source>();
            this.isFoodOrUtility = false;
            this.isCharacterBound = isCharacterBound_;
			this.isAccountBound = isAccountBound_;
		    name= null;
		    description= null;
			this.iconId = 63369; //when in doubt cabbage
            rarity = null;
			isStackable =true;
		    isDeletable = false;
		    isRareForSalvage = false;
            price = 0;
            this.hasInformation = false;
			this.VendorValue = 0;
			this.isSellable=true;
			this.isSalvagable=true;
			this.chatLink = "";
			if(delayedCreate==false)
			{
				this.build_basic_item_info();
			}
			
		}

		public void build_basic_item_info()
		{

			ItemInfo info_;
			if (Magic.jsonLut.itemLut.ContainsKey(this.itemId))
			{
				info_ = Magic.jsonLut.itemLut[this.itemId];
			}
			else
			{
				
				info_ = Magic.unknown;
				info_.Id = this.itemId;
			}

			//adjust name of Essence of luck items to include the rarity
			if (Magic.is_luck_essence(this.itemId))
			{
				this.name = Magic.get_local_name(this.itemId);
			}
			else
			{
				this.name = info_.Name;
			}

			this.iconId = info_.IconId;
			this.rarity = (ItemRarity)info_.Rarity;
			this.description = info_.Description;
			this.isFoodOrUtility = info_.isFoodOrUtility;
			this.VendorValue = info_.VendorValue;
			var urlName = this.name.Replace(" ", "_");
			this.wikiLink = $"wiki.guildwars2.com/wiki/{urlName}";

			bool salvagable = true;


			if (Magic.is_non_stackable_type((ItemType)info_.Type) == true)
			{
				this.isStackable = false;
			}
			
			
			foreach (var flag in info_.Flags)
			{
				if ((ItemFlag)flag == ItemFlag.NoSalvage)
				{
					salvagable = false;
				}
				if ((ItemFlag)flag == ItemFlag.SoulbindOnAcquire)
				{
					this.isAccountBound = true;
					this.isCharacterBound = true;
					this.isStackable = false;

				}
				if((ItemFlag) flag ==ItemFlag.AccountBound)
				{
					this.isAccountBound = true;
				}
				if((ItemFlag)flag == ItemFlag.NoSalvage)
				{
					this.isSalvagable = false;
				}
				if((ItemFlag)flag == ItemFlag.NoSell)
				{
					this.isSellable = false;
				}

			}


			if (Magic.collectionOnlyIds.Contains(info_.Id))
			{
				this.isDeletable = true;
			}

			if (Magic.is_salvagable_equipment((ItemType)info_.Type) && (ItemRarity)info_.Rarity == ItemRarity.Rare && salvagable && info_.Level > 67)
			{
				this.isRareForSalvage = true;
				


			}



			this.hasInformation = true;
		}

		//condense sources 
		public void add_source(Source source_)
        {
			foreach (var source in this.sources)
			{
				if(source.place==source_.place)
                {
                    source.count += source_.count;
					source.stacks++;
                    return;
                }
			}
			this.sources.Add(source_);
        }

		
       
		public List<Source> get_advice_stacks(int materialStorageSize_)
        {
            if(this.isCharacterBound==true||this.isStackable==false)
            {
                return new List<Source>();
            }
            else
            {
				
				List<Source> stackableSources = this.get_partial_stacks(materialStorageSize_);
				
				int numberOfPartialStacks = 0;
				int partialStackAmount = 0;
				foreach (var source in stackableSources)
				{
					numberOfPartialStacks++;
					partialStackAmount += Convert.ToInt32(source.count%250);
				}
				
				int remainder = 0;
				int numberOfConsolidatedStacks =Math.DivRem(partialStackAmount, 250, out remainder);
				if(remainder!=0)
				{
					numberOfConsolidatedStacks++;
				}
				
				if ((numberOfPartialStacks > 1) && (numberOfPartialStacks > numberOfConsolidatedStacks))
				{
					return stackableSources;
				}
				return new List<Source>();
			}
            
        }

        public List<Source> get_partial_stacks(int materialStorageSize_)
        {
            List<Source> partialStacks = new List<Source>();
			foreach (Source currentSource in this.sources)
			{
				if ((currentSource.count%Convert.ToUInt64(250) != Convert.ToUInt64(0)) || ((currentSource.place == "Material Storage") && (currentSource.count < Convert.ToUInt64(materialStorageSize_))))
				{
					partialStacks.Add(currentSource);
				}
			}
            return partialStacks;
		}

        public UInt64 total_count()
        {
            UInt64 total = 0;
			foreach (Source current_Source in this.sources)
			{
				total += current_Source.count;
			}

			return total;
        }

    
		public override string ToString()
        {
            return this.itemId.ToString() + " " + this.name + " " + string.Join(", ", this.sources);
		}

	}

    class ItemForDisplay
    {
        protected Item item;
		protected List<Source> sources;
		protected string advice;
		

        public ItemForDisplay(Item item_, List<Source> sources_, string advice_)
        {
            this.item = item_;
            if(sources_==null)
            {
                this.sources = item_.sources;
            }
            else
            {
                this.sources = sources_;
            }
            this.advice = advice_;
			
        }

		public virtual bool applicable_to_id(int id_)
		{
			if(this.item.itemId==id_)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		protected virtual string get_source_string()
		{
			return "Sources:\n" + string.Join("\n", this.sources);
		}

		public virtual bool has_source(string place_)
		{
			return this.sources.Any(source => source.place == place_);
		}

		public virtual int get_id()
		{
			return this.item.itemId;
		}

		public virtual string get_chatlink()
		{
			return this.item.chatLink;
		}

		public virtual int get_iconId()
		{
			return this.item.iconId;
		}

		public virtual string get_advice(string name = null)
		{
			return Magic.get_current_translated_string(this.advice);
		}

		public virtual string print(string name= null)
		{
			return Magic.get_local_name(this.get_id()) + "\n" + this.item.total_count() + "x\n" + Magic.get_current_translated_string(this.advice) + "\n" + this.get_source_string();
		}

		public override string ToString()
		{
			//return (this.item?.ToString() ?? " ") + " " + (this.advice?.ToString() ?? " ") + " " + string.Join(", ", this.sources);
			return this.print();
		}
    }

	class GobblerItemForDisplay : ItemForDisplay
	{
		public int gobblerId;
		public GobblerItemForDisplay(Item item_, List<Source> sources_ , string advice_ , int gobblerId_ ) :base(item_, sources_, advice_)
		{
			this.gobblerId = gobblerId_;
		}

		

		protected override string get_source_string()
		{
			return "Sources:\n" + string.Join("\n", this.sources);
		}

		public override int get_id()
		{
			return base.get_id();
		}

		public override int get_iconId()
		{
			return base.get_iconId();
		}

		public override string get_advice(string name = null)
		{
			return Magic.get_current_translated_string(this.advice) + " (" + Magic.get_local_name(this.gobblerId) + ")";
		}

		public override string print(string name = null)
		{
			return Magic.get_local_name(this.get_id()) + "\n" + Magic.get_current_translated_string(this.advice) + " (" + Magic.get_local_name(this.gobblerId) + ")" + "\n" + this.get_source_string();
		}

		public override string ToString()
		{
			return this.print();
		}
	}

	class CraftingItemForDisplay : ItemForDisplay
	{
		Dictionary<RecipeIngredient, List<Source>> ingredientSources;
		int outputId = 0;

		public CraftingItemForDisplay(Item item_,	Dictionary<RecipeIngredient, List<Source>> ingredientSources_, string advice_, int outputId_) : base(item_, null, advice_)
		{
			this.ingredientSources = ingredientSources_;
			this.outputId = outputId_;
		}

		public override bool applicable_to_id(int id_)
		{
			foreach (var id in this.ingredientSources.Keys)
			{
				if (id.ItemId == id_)
				{
					return true;
				}
				
			}
			return false;
		}
		public string get_ingredient_string(string place_ =null)
		{
			string output = "";
			foreach (var item in this.ingredientSources)
			{
				List<Source> sufficientCount = new List<Source>();
				var sorted =item.Value.OrderByDescending(source => source.count).ToList();
				if(place_!=null)
				{
					var index = sorted.FindIndex(source => source.place == place_);
					if(index!=-1)
					{
						sufficientCount.Add(sorted.ElementAt(index));
						sorted.RemoveAt(index);
					}
					else
					{
						sufficientCount.Add(sorted.First());
						sorted.RemoveAt(0);
					}
					
				}
				else
				{
					sufficientCount.Add(sorted.First());
					sorted.RemoveAt(0);
				}
					
				while(sufficientCount.Sum(questionable => Convert.ToInt32(questionable.count))<item.Key.Count)
				{
					sufficientCount.Add(sorted.First());
					sorted.RemoveAt(0);
				}
				var partial = item.Key.Count+ " x " + Magic.get_local_name(item.Key.ItemId) + " / " + string.Join("\n", sufficientCount)+"\n";
				output += partial;
			}
			return output;
		}

		public override bool has_source(string place_)
		{
			
			bool result = false;
			foreach (var list in this.ingredientSources.Values)
			{
				result |= list.Any(source => source.place == place_);
			}
			return result;
		}

		public override int get_id()
		{
			return this.item.itemId;
		}

		public override string get_chatlink()
		{
			return this.item.chatLink;
		}

		public override int get_iconId()
		{
			return this.item.iconId;
		}

		public override string get_advice(string name = null)
		{
			return Magic.get_current_translated_string(this.advice) +": "+Magic.get_local_name(outputId)+ "\n" + this.get_ingredient_string(name);
		}

		public override string print(string name = null)
		{
			return Magic.get_local_name(this.get_id()) + "\n" + this.item.total_count() + "x\n" + Magic.get_current_translated_string(this.advice)+ ": " + Magic.get_local_name(outputId) +"\n" + this.get_ingredient_string(name);
		}

		public override string ToString()
		{

			return this.print();
		}
	}

	
	class MiscCraftingItemForDisplay : ItemForDisplay
	{
		private Item output;
		public MiscCraftingItemForDisplay(Item input_, Item output_, string advice_ ): base(input_, null, advice_)
		{
			this.output = output_;
		}

		public override bool applicable_to_id(int id_)
		{
			return this.item.itemId==id_;
		}
		protected override string get_source_string()
		{
			return "Sources:\n" + string.Join("\n", this.item.sources);
		}

		public override string print(string name = null)
		{
			//TODO add character based filtering
			return Magic.get_local_name(this.get_id()) + "\n" + Magic.get_current_translated_string(this.advice) + " (" + Magic.get_local_name(this.output.itemId) + ")" + "\n"  + this.get_source_string();
		}

		public override string ToString()
		{
			return this.print();
		}
	}

	class CombinedItemForDisplay: ItemForDisplay
	{
		List<ItemForDisplay> itemList;
		
		public CombinedItemForDisplay(Item item_, List<ItemForDisplay> itemList_): base(item_, null, null)
		{
			this.itemList = itemList_;
		}

		public override bool applicable_to_id(int id_)
		{
			if (this.item.itemId == id_)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		protected override string get_source_string()
		{
			//List<Source> combinedSources;
			return "Sources:\n" + string.Join("\n", this.item.sources);
		}

		public override int get_id()
		{
			return base.get_id();
		}

		public override int get_iconId()
		{
			return base.get_iconId();
		}

		public override string print(string name = null)
		{
			if(this.itemList.Any()==false)
			{
				return Magic.get_local_name(this.get_id()) + "\n" + Magic.get_current_translated_string("No current advice") + "\n" + this.get_source_string();
			}

			string combinedAdvice = "";
			foreach (var item in this.itemList)
			{
				combinedAdvice += item.get_advice() + "\n";
				//
			}

			return Magic.get_local_name(this.get_id()) + "\n" + combinedAdvice+ "\n" + this.get_source_string();
		}

		public override string ToString()
		{
			return this.print();
		}
	}



	class EmptyItemForDisplay : ItemForDisplay
	{
		public EmptyItemForDisplay() : base(Magic.borealTrunk, new List<Source>(), null)
		{

		}
		protected override string get_source_string()
		{
			return null;
		}

		public override bool applicable_to_id(int id_)
		{
			return false;
		}

		public override int get_id()
		{
			return 0;
		}

		public override int get_iconId()
		{
			return 1414044;
		}

		public override string print(string name = null)
		{
			return null;
		}

		public override string ToString()
		{
			return null;
		}
	
	}

	class BagForDisplay
	{
		private int? id;
		private int size;
		public BagForDisplay(int? id_, int size_)
		{
			this.id = id_;
			this.size = size_;
		}

		public int get_id()
		{
			if(this.id==null)
			{
				return 0;
			}
			else
			{
				return this.id.Value;
			}
		}

		public int get_size()
		{
			return this.size;
		}

		public int get_icon_id()
		{
			if(this.id==null)
			{
				return 1414044;
			}
			else
			{
				return Magic.jsonLut.itemLut[this.id.Value].IconId;
			}
		}

		public string get_name()
		{
			if (this.id == null)
			{
				return "Empty";//TODO add translation
			}
			else
			{
				return Magic.get_local_name(this.id.Value);
			}
		}

		public string get_advice()
		{
			if(this.size==0)
			{
				return Magic.get_current_translated_string("Equip:") + " (" + Magic.get_local_name(Magic.silkBag.itemId) + ")";
			}

			if(this.size<18)
			{
				return Magic.get_current_translated_string("Upgrade these bags to") +" ("+ Magic.get_local_name(Magic.silkBag.itemId)+")";
			}
			else
			{
				return Magic.get_current_translated_string("Potentially replace these bags with") + " (" + Magic.get_local_name(Magic.borealTrunk.itemId)+")";
			}
		}
	}
    class Gobbler
    {
        public int itemId;
        public int count;
		public List<int> food;
        public string name;

        public Gobbler(int id_, List<int> food_, int count_, string name_)
        {
            this.itemId = id_;
			this.food = food_;
			this.count = count_;
            this.name = name_;
        }

        public Gobbler(int id_, int food_, int count_, string name_)
        {
			this.itemId = id_;
			this.food = new List<int> { food_ };
			this.count = count_;
            this.name = name_;
		}

		public Gobbler()
		{
			this.itemId = 0;
			this.food = null;
			this.count = 0;
            this.name = null;
		}
	}

    class MiscAdvice
    {
		public int itemId;
		public int minCount;
        public string advice;

        public MiscAdvice(int itemId_, int count_, string advice_)
		{
			this.itemId = itemId_;
			this.minCount = count_;
			this.advice = advice_;
		}

		public MiscAdvice()
		{
			this.itemId = 0;
			this.minCount = 0;
			this.advice = null;
		}
	}

	class CraftingMiscAdvice
	{
		public Dictionary<int, int> idCountMapping;
		public string advice;
		public int outputId;
		public CraftingMiscAdvice(Dictionary<int, int> idCountMapping, string advice, int outputId)
		{
			this.idCountMapping = idCountMapping;
			this.advice = advice;
			this.outputId = outputId;
		}

		public CraftingMiscAdvice()
		{
			this.idCountMapping = new Dictionary<int, int>();
			this.advice = null;
			this.outputId = 0;
		}
	}

	class InventoryBagSlot
	{
		private int id;
		private int size;
		public InventoryBagSlot(int id_, int size_=0)
		{
			this.id = id_;
			this.size = size_;
		}

		public int get_size()
		{
			return this.size;
		}
		public int get_id()
		{
			return this.id;
		}
	}

	class ItemInfo
	{
		public int Id;
		public string Name;
		public int IconId;
		public int Rarity;//ApiEnum<ItemRarity> Rarity;
		public string Description;
		public int Type;//ApiEnum<ItemType> Type;
		public bool isFoodOrUtility;
		public List<int> Flags;//ApiFlags<ItemFlag> Flags;
		public int Level;
		public int VendorValue;
		public string chatLink;
		public ItemInfo()
		{
			this.Id = 0;
			this.Name = null;
			this.IconId = 63369;
			this.Rarity = 0;
			this.Description = null;
			this.Type = 0;
			this.isFoodOrUtility = false;
			this.Flags = new List<int>();
			this.Level = 0;
			this.VendorValue = 0;
			this.chatLink = "";
		}



	}

	class RecipeInfo
	{
		public int Id;
		public int Type;//ApiEnum<RecipeType> Type;
		
		public IReadOnlyList<RecipeIngredient> Ingredients;
		public int OutputItemId;
		public List<int> Disciplines;//ApiFlags<CraftingDisciplineType> Disciplines;
		public string chatLink;
		public RecipeInfo()
		{
			this.Id = 0;
			this.Type = 0;
			this.Ingredients = null;
			this.OutputItemId = 0;
			this.Disciplines = new List<int>();
			this.chatLink = "";

		}
	}
}
