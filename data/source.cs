using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gw2stacks_blish.data
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

	class RecipeSource:Source
	{
		public List<Source> recipeIngredients;
		public List<string> disciplines;

		public RecipeSource(List<Source> recipeIngredients_, List<string> disciplines_) : base(0, null)
		{
			this.recipeIngredients = recipeIngredients_;
			this.disciplines = disciplines_;

		}

		public override string ToString()
		{
			
			return string.Join(", ", this.recipeIngredients) +"\n"+ string.Join(", ", this.disciplines);
		}
	}

	
	//TODO implement new source representing recipes, their input items and their profession, change recipe storage to use a map<outputitem, recipe>
}
