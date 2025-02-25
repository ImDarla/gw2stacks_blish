using Blish_HUD.PersistentStore;
using Gw2Sharp.WebApi.V2.Models;
using reader;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace data
{
    class model
    {
        Dictionary<int, Item> items;
        UInt64 material_storage_size;


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

        public void build_material_storage_size(gw2api api)
        {
            var task = api.material_storage();
            task.Wait();
            UInt64 max_count = 0;
			foreach (var item in task.Result)
			{
                max_count = Math.Max(max_count, (UInt64)item.Count);
			}

            this.material_storage_size = Convert.ToUInt64(Math.Ceiling(Convert.ToDouble(max_count / 250)) * 250);
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
    }
}
