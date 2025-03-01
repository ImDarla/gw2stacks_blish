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
		

		public Source(UInt64 count_, string place_)
		{
			this.count = count_;
			this.place = place_;
			
		}

		

		public override string ToString() 
		{
			return this.count.ToString() + " @ " + this.place;
		}
	}
}
