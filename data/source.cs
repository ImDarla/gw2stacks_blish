using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data
{
   
    class Source
    {
		

		public UInt64 count;
		public string place;
		public string account;

		public Source(UInt64 count, string place, string account)
		{
			this.count = count;
			this.place = place;
			this.account = account;
		}

		

		public override string ToString() 
		{
			return this.count.ToString() + " " + this.place + "@" + this.account;
		}
	}
}
