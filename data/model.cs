using reader;
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
            }
            this.items[id].add(source);
            this.items[id].account_bound = account_bound;
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
            
        }
    }
}
