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

		public Source(UInt64 count_, string place_, string account_)
		{
			this.count = count_;
			this.place = place_;
			this.account = account_;
		}

		

		public override string ToString() 
		{
			return this.count.ToString() + " " + this.place + "@" + this.account;
		}
	}
}
