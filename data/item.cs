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


		public Item(int id_)
        {
            this.itemId = id_;
            this.sources = new List<Source>();
            this.isFoodOrUtility = false;
            this.isCharacterBound = false;
			isAccountBound = false;
		    name= null;
		    description= null;
			this.iconId = 63369; //when in doubt cabbage
            rarity = null;
			isStackable =false;
		    isDeletable = false;
		    isRareForSalvage = false;
            price = 0;
            this.hasInformation = false;
        }

        //condense sources 
        public void add_source(Source source_)
        {
			foreach (var source in this.sources)
			{
				if(source.place==source_.place)
                {
                    source.count += source_.count;
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
				List<Source> stackableSources = new List<Source>();
				List<Source> stackableSource = this.get_partial_stacks(materialStorageSize_);
				UInt64 numberOfPartialStacks = Convert.ToUInt64(stackableSource.Count());
				UInt64 numberOfConsolidatedStacks = Convert.ToUInt64(Math.Ceiling(Convert.ToDouble(this.total_count() / 250)));
				if (this.isStackable && ((numberOfPartialStacks > 1) && (numberOfPartialStacks > numberOfConsolidatedStacks)))
				{
					stackableSources.AddRange(stackableSource);
				}
				return stackableSources;
			}
            
        }

        public List<Source> get_partial_stacks(int materialStorageSize_)
        {
            List<Source> partialStacks = new List<Source>();
			foreach (Source currentSource in this.sources)
			{
				if ((currentSource.count%250 !=0) || ((currentSource.place == "Material Storage") && (currentSource.count < Convert.ToUInt64(materialStorageSize_))))
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
        public Item item;
        public List<Source> sources;
        public string advice;

        public ItemForDisplay(Item item_, List<Source> sources_=null, string advice_ = null)
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

		public string get_source_string()
		{
			return "Sources:\n" + string.Join("\n", this.sources);
		}

		public override string ToString()
		{
            return (this.item?.ToString() ?? " ") + " " + (this.advice?.ToString() ?? " ") + " " + string.Join(", ", this.sources);
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
		}



	}

	class RecipeInfo
	{
		public int Id;
		public int Type;//ApiEnum<RecipeType> Type;

		public IReadOnlyList<RecipeIngredient> Ingredients;
		public int OutputItemId;
		public List<int> Disciplines;//ApiFlags<CraftingDisciplineType> Disciplines;

		public RecipeInfo()
		{
			this.Id = 0;
			this.Type = 0;
			this.Ingredients = null;
			this.OutputItemId = 0;
			this.Disciplines = new List<int>();

		}
	}
}
