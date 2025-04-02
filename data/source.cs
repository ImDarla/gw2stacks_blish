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
		public UInt64 stacks;

		public Source(UInt64 count_, string place_)
		{
			this.count = count_;
			this.place = place_;
			this.stacks = 1;
		}

		

		public override string ToString() 
		{
			return this.count.ToString() + " @ " + Magic.get_local_storage_name(this.place);
		}
	}

	class IngredientSource : Source
	{
		public int id;


		public IngredientSource(UInt64 count_, int id_):base(count_, null)
		{

			this.id = id_;

		}



		public override string ToString()
		{
			return this.count.ToString() + " x " + Magic.get_local_name(this.id);
		}
	}


	class RecipeSource:Source
	{
		public List<IngredientSource> recipeIngredients;
		public List<string> disciplines;

		public RecipeSource(List<IngredientSource> recipeIngredients_, List<string> disciplines_) : base(0, null)
		{
			this.recipeIngredients = recipeIngredients_;
			this.disciplines = disciplines_;

		}

		public override string ToString()
		{
			
			return string.Join(", ", this.recipeIngredients) +"\n"+ string.Join(", ", this.disciplines);
		}
	}

	
	
}
