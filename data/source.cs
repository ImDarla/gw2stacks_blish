using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data
{
   
    class Source
    {
		static Dictionary<string, string> sourceNames = new Dictionary<string, string>
		{
			{ "$bank", "Account Bank" },
			{ "$storage", "Material Storage" },
			{ "$shared_slot", "Shared Inventory Slot" }
		};

		public UInt64 count;
		public string place;
		public string account;

		public Source(UInt64 count, string place, string account)
		{
			this.count = count;
			this.place = place;
			this.account = account;
		}

		public string place_repr()
		{
			//TODO fix this
			return "source_names.get";
		}

		public override string ToString() 
		{
			return this.count.ToString() + " " + this.place + "@" + this.account;
		}
	}
}
