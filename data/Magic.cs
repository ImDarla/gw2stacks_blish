using Blish_HUD.GameServices.ArcDps.V2.Models;
using data;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data
{
    class Magic
    {
        public List<int> luckIds = new List<int> { 45175, 45176, 45177 };
		public Dictionary<int, string> luckNameMapping = new Dictionary<int, string>
		{
			{45175, "Essence of Luck (fine)" },
			{45176, "Essence of Luck (masterwork)" },
			{45177, "Essence of Luck (rare)" },
			{45178, "Essence of Luck (exotic)" },
			{45179, "Essence of Luck (legendary)" }
			
		};
        public int ectoId = 19721;
        public double salvagePrice = 0.10496; //seemingly from mystic salvage kit??
		public double ectoChance = 0.875;
		public double tax = 0.85;
        public List<ItemType> nonStackableTypes = new List<ItemType> { ItemType.Armor, ItemType.Back, ItemType.Gathering, ItemType.Tool, ItemType.Trinket, ItemType.Weapon, ItemType.Bag, ItemType.Container };
		public List<ItemType> salvagableEquipment = new List<ItemType> { ItemType.Armor, ItemType.Back, ItemType.Trinket, ItemType.Weapon };
        public List<ApiEnum<RecipeType>> pertinentRecipeTypes = new List<ApiEnum<RecipeType>> { RecipeType.Refinement, RecipeType.RefinementEctoplasm, RecipeType.RefinementObsidian, RecipeType.IngredientCooking };
		public Dictionary<int, string> gameplayConsumables = new Dictionary<int, string>{
			{ 78758, "Trade to get bounty for bandit leader."},
			{ 78886, "Have in inventory while defeating a bandit leader to spawn the Legendary Bandit Executioner"},
			{ 84335, "Use during a treasure hunt meta in Desert Highlands to spawn chests"},
			{ 67826, "Use in the Silverwastes after a meta completes to spawn chests. Make sure you have required keys."},
			{ 67979, "Open a greater nightmare pod in the Silverwastes after completing meta."},
			{ 67818, "Use during breach event in Silverwastes."},
			{ 67780, "Open Tarnished chest in Silverwastes."},
			{ 87517, "Open krait Sunken Chests to progress a Master Diver achievement."},
			{ 48716, "Open chests in the Aetherpath of the Twilight Arbor dungeon."},
			{ 78782, "Complete this bounty."},
			{ 78754, "Complete this bounty."},
			{ 78786, "Complete this bounty."},
			{ 78784, "Complete this bounty."},
			{ 78781, "Complete this bounty."},
			{ 78883, "Complete this bounty."},
			{ 78859, "Complete this bounty."},
			{ 78988, "Complete this bounty."},
			{ 78867, "Complete this bounty."},
			{ 78954, "Complete this bounty."},
			{ 71627, "Complete events in the Verdant Brink."},
			{ 75024, "Complete events in the Auric Basin."},
			{ 71207, "Complete events in the Tangled Depths."},
			{ 87630, "Contribute Spare Parts to kick off meta event in the Domain of Kourna."},
			{ 93407, "Use in the Drizzlewood Coast to spawn chests. Make sure you have required keys."},
			{ 93371, "Use to unlock achievements (and play in Drizzlewood Coast)"},
			{ 93817, "Use to unlock achievements (and play  Drizzlewood Coast)}, or just delete/tp when you are done."},
			{ 93842, "Use to unlock achievements (and play  Drizzlewood Coast)}, or just delete/tp when you are done."},
			{ 93799, "Use to unlock achievements (and play  Drizzlewood Coast)}, or just delete/tp when you are done."} };

		public List<Gobbler> gobblers = new List<Gobbler>{
			new Gobbler(46731, 77093, 250), // Herta
            new Gobbler(46731, 66999, 50),  // Mawdrey
            new Gobbler(46733, 69887, 50),  // Princess
            new Gobbler(46735, 68369, 50),  // Star

            new Gobbler(83103, 83305, 25),  // Spearmarshall 
			};

		public List<MiscAdvice> miscAdvices = new List<MiscAdvice>{
			new MiscAdvice(43773, 25, "Transform Quartz Crystals into a Charged Quartz Crystal at a place of power."),
			new MiscAdvice(66608, 100, "Sift through silky sand."),
			new MiscAdvice(48717, 4, "Craft 'Completed Aetherkey'."),
			new MiscAdvice(93472, 1, "Consume to get War Supplies"),
			new MiscAdvice(93649, 1, "Consume to get War Supplies"),
			new MiscAdvice(93455, 1, "Consume to get War Supplies"),
			new MiscAdvice(68531, 1, "Consume to get Mordrem parts which can be exchanged for map currency"),
			new MiscAdvice(39752, 250, "Convert to Bauble Bubble"),
			new MiscAdvice(36041, 1000, "Convert to Candy Corn Cob"),
			new MiscAdvice(43319, 1000, "Convert to Jorbreaker")

		};

		public List<int> karmaIds = new List<int>
		{
			86336, 85790, 41740, 38030, 86374, 86181, 36448, 36449, 92714, 95765, 36450, 36451, 
			41738, 36456, 77652, 36457, 36458, 36459, 36460, 70244, 69939, 39127, 41373, 36461
		};

		public List<int> lws3Id = new List<int>
		{
			79280, 79469, 79899, 80332, 81127, 81706
		};

		public List<int> lws4Id = new List<int>
		{
			86069, 86977, 87645, 88955, 89537, 90783
		};

		public List<int> ibsId = new List<int>
		{
			92272
		};
		
		public static int id_from_Render_URI(string uri_)
		{
			var initialSplit = uri_.Split('/');
			var file = initialSplit.Last();
			return Convert.ToInt32(file.Split('.')[0]);
		}

		

		public Magic()
		{
			//this.luckIds = luckIds;
			//this.ectoId = ectoId;
			//this.salvagePrice = salvagePrice;
			//this.ectoChance = ectoChance;
			//this.tax = tax;
			//this.nonStackableTypes = nonStackableTypes;
			//this.salvagableEquipment = salvagableEquipment;
			//this.pertinentRecipeTypes = pertinentRecipeTypes;
			//this.gameplayConsumables = gameplayConsumables;
			//this.gobblers = gobblers;
			//this.miscAdvices = miscAdvices;
			//this.karmaIds = karmaIds;
			//this.lws3Id = lws3Id;
			//this.lws4Id = lws4Id;
			//this.ibsId = ibsId;
		}
	}
}
