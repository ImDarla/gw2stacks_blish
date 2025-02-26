using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using data;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;

namespace data
{
    class Item
    {

        public int item_id;
        public List<Source> Sources;

		public bool account_bound;
		public string name;
		public string description;
        public RenderUrl? icon;
        public ItemRarity? rarity;
		public bool stackable;
		public bool deletable;
		public bool rare_for_salvage;
        public string wiki_link;
        public int? price;

        public Item(int ID)
        {
            this.item_id = ID;
            this.Sources = new List<Source>();

			account_bound= false;
		    name= null;
		    description= null;
            icon = null;
            rarity = null;
			stackable =false;
		    deletable = false;
		    rare_for_salvage = false;
            price = null;
        }

        public void add(Source Source)
        {
            this.Sources.Add(Source);
        }

        //TODO fix item types
		public List<Source> get_advice_stacks(Dictionary<string, int> material_storage_size)
        {
            if (this.account_bound ==false)
            {
                List<Source> stackable_Source = this.get_partial_stacks(material_storage_size);
                UInt64 number_of_partial_stacks = Convert.ToUInt64(stackable_Source.Count());
                UInt64 number_of_stacks_consolidated = Convert.ToUInt64(Math.Ceiling(Convert.ToDouble(this.total_count() / 250)));
                if(this.stackable &&((number_of_partial_stacks>1)&&(number_of_partial_stacks >number_of_stacks_consolidated)))
                {
                    return stackable_Source;
                }
                else
                {
                    return new List<Source>();
                }

			}
            else
            {
                List<Source> stackable_Sources = new List<Source>();
                foreach(string account in material_storage_size.Keys)
                {
					List<Source> stackable_Source = this.get_partial_stacks(new Dictionary<string, int> { { account, material_storage_size.GetValueOrDefault(account) } });
					UInt64 number_of_partial_stacks = Convert.ToUInt64(stackable_Source.Count()); 
					UInt64 number_of_stacks_consolidated = Convert.ToUInt64(Math.Ceiling(Convert.ToDouble(this.total_count() / 250)));
					if (this.stackable && ((number_of_partial_stacks > 1) && (number_of_partial_stacks > number_of_stacks_consolidated)))
					{
                        stackable_Sources.AddRange(stackable_Source);
					}
				}
                return stackable_Sources;

			}
        }

        public List<Source> get_partial_stacks(Dictionary<string, int> material_storage_size)
        {
            List<Source> partial_stacks = new List<Source>();
			foreach (Source current_Source in this.Sources)
			{
				if(material_storage_size.ContainsKey(current_Source.account))
                {
                    if((current_Source.count!=0) &&((current_Source.count<250)||((current_Source.place=="$storage")&&(current_Source.count<Convert.ToUInt64(material_storage_size[current_Source.account])))))
                    {
                        partial_stacks.Add(current_Source);
                    }
                }
			}
            return partial_stacks;
		}

        public UInt64 total_count(string account=null)
        {
            UInt64 total = 0;
			foreach (Source current_Source in this.Sources)
			{
				if(account == null||current_Source.account==account)
                {
                    total += current_Source.count;
                }
			}

			return total;
        }

        public List<Source> get_Sources_for_account(string account)
        {
            List<Source> Sources_for_account = new List<Source>();
			foreach (Source current_Source in this.Sources)
			{
				if(current_Source.account==account)
                {
                    Sources_for_account.Add(current_Source);
                }
			}
            return Sources_for_account;
		}

		public override string ToString()
        {
            return this.item_id.ToString() + " " + this.name + " " + string.Join(", ", this.Sources);
        }

	}

    class ItemForDisplay
    {
        public Item item;
        public List<Source> Sources;
        public string advice;

        public ItemForDisplay(Item item, List<Source> sources=null, string advice = null)
        {
            if(!sources.Any())
            {
                this.Sources = item.Sources;
            }
            else
            {
                this.Sources = sources;
            }
            this.advice = advice;
        }

		public override string ToString()
		{
            return this.item.ToString() + " " + this.advice + " " + string.Join(", ", this.Sources); 
        }
    }

    class Gobbler
    {
        public int itemId;
        public int count;
		public List<int> food;

        public Gobbler(int id, List<int> food, int count)
        {
            this.itemId = id;
			this.food = food;
			this.count = count;
            
        }

        public Gobbler(int id, int food, int count)
        {
			this.itemId = id;
			this.food = new List<int> { food };
			this.count = count;
		}
    }

    class MiscAdvice
    {
		public int itemId;
		public int minCount;
        public string advice;

        public MiscAdvice(int itemId, int count, string advice)
		{
			this.itemId = itemId;
			this.minCount = count;
			this.advice = advice;
		}
	}
}
