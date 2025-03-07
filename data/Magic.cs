using Blish_HUD.GameServices.ArcDps.V2.Models;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gw2stacks_blish.data
{
    class Magic
    {
		#region variables
		private static Locale currentLocale;
		public static int ectoId = 19721;
		public static double salvagePrice = 0.10496; //seemingly from mystic salvage kit??
		public static double ectoChance = 0.875;
		public static double tax = 0.85;
		#endregion

		#region dictionaries
		//id-name mapping of all luck essence items as they are called the same in api calls with differing rarity
		private static Dictionary<int, string> luckNameMapping = new Dictionary<int, string>
		{
			{45175, "Essence of Luck (fine)" },
			{45176, "Essence of Luck (masterwork)" },
			{45177, "Essence of Luck (rare)" },
			{45178, "Essence of Luck (exotic)" },
			{45179, "Essence of Luck (legendary)" }

		};

		//advice map for items that are consumed in gameplay
		public static Dictionary<int, string> gameplayConsumables = new Dictionary<int, string>{
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

		private static Dictionary<CraftingDisciplineType, string> disciplineNameMapping = new Dictionary<CraftingDisciplineType, string>
		{
			{CraftingDisciplineType.Scribe, "Scribe" },
			{CraftingDisciplineType.Tailor, "Tailor" },
			{CraftingDisciplineType.Leatherworker, "Leatherworker" },
			{CraftingDisciplineType.Weaponsmith, "Weaponsmith" },
			{CraftingDisciplineType.Armorsmith, "Armorsmith" },
			{CraftingDisciplineType.Artificer, "Artificer" },
			{CraftingDisciplineType.Chef, "Chef" },
			{CraftingDisciplineType.Jeweler, "Jeweler" },
			{CraftingDisciplineType.Huntsman, "Huntsman" },
			{CraftingDisciplineType.Unknown,  "Unknown"}

		};

		private static Dictionary<AdviceType, string> adviceTypeNameMapping = new Dictionary<AdviceType, string>
		{
			{AdviceType.stackAdvice , "stack advice"},
			{AdviceType.vendorAdvice , "vendor advice"},
			{AdviceType.rareSalvageAdvice , "rare salvage advice"},
			{AdviceType.craftLuckAdvice , "craftable luck advice"},
			{AdviceType.deletableAdvice , "deletable advice"},
			{AdviceType.salvageAdvice , "salvagable  advice"},
			{AdviceType.consumableAdvice , "consumable  advice"},
			{AdviceType.gobblerAdvice , "gobbler  advice"},
			{AdviceType.karmaAdvice , "karma consumable  advice"},
			{AdviceType.craftingAdvice , "crafting advice"},
			{AdviceType.lwsAdvice , "living world advice"},
			{AdviceType.miscAdvice , "miscellaneous  advice"},
	};



		private static Dictionary<StorageType, string> storageTypeNameMapping = new Dictionary<StorageType, string>
		{
			{StorageType.materialStorage,  "Material Storage"},
			{StorageType.bankStorage, "Bank Storage" },
			{StorageType.sharedStorage, "Shared Storage" }
		};

		private static Dictionary<string, string> englishToEnglish = new Dictionary<string, string>
		{
			
			#region discipline names
			{"Scribe", "Scribe" },
			{"Tailor", "Tailor" },
			{"Leatherworker", "Leatherworker" },
			{"Weaponsmith", "Weaponsmith" },
			{"Armorsmith", "Armorsmith" },
			{"Artificer", "Artificer" },
			{"Chef", "Chef" },
			{"Jeweler", "Jeweler" },
			{"Huntsman", "Huntsman" },
			{"Unknown", "Unknown" },
			#endregion
			
			#region miscAdvice
			{"Transform Quartz Crystals into a Charged Quartz Crystal at a place of power.", "Transform Quartz Crystals into a Charged Quartz Crystal at a place of power." },
			{"Sift through silky sand.", "Sift through silky sand." },
			{"Craft 'Completed Aetherkey'.", "Craft 'Completed Aetherkey'." },
			{"Consume to get War Supplies", "Consume to get War Supplies" },
			{"Consume to get Mordrem parts which can be exchanged for map currency", "Consume to get Mordrem parts which can be exchanged for map currency" },
			{"Convert to Bauble Bubble", "Convert to Bauble Bubble" },
			{"Convert to Candy Corn Cob", "Convert to Candy Corn Cob" },
			{"Convert to Jorbreaker", "Convert to Jorbreaker" },
			#endregion
			
			#region gameplay advice
			{ "Trade to get bounty for bandit leader.", "Trade to get bounty for bandit leader."},
			{ "Have in inventory while defeating a bandit leader to spawn the Legendary Bandit Executioner", "Have in inventory while defeating a bandit leader to spawn the Legendary Bandit Executioner"},
			{ "Use during a treasure hunt meta in Desert Highlands to spawn chests", "Use during a treasure hunt meta in Desert Highlands to spawn chests"},
			{ "Use in the Silverwastes after a meta completes to spawn chests. Make sure you have required keys.", "Use in the Silverwastes after a meta completes to spawn chests. Make sure you have required keys."},
			{ "Open a greater nightmare pod in the Silverwastes after completing meta.", "Open a greater nightmare pod in the Silverwastes after completing meta."},
			{ "Use during breach event in Silverwastes.", "Use during breach event in Silverwastes."},
			{ "Open Tarnished chest in Silverwastes.", "Open Tarnished chest in Silverwastes."},
			{ "Open krait Sunken Chests to progress a Master Diver achievement.", "Open krait Sunken Chests to progress a Master Diver achievement."},
			{ "Open chests in the Aetherpath of the Twilight Arbor dungeon.", "Open chests in the Aetherpath of the Twilight Arbor dungeon."},
			{ "Complete this bounty.", "Complete this bounty."},
			{ "Complete events in the Verdant Brink.", "Complete events in the Verdant Brink."},
			{ "Complete events in the Auric Basin.", "Complete events in the Auric Basin."},
			{ "Complete events in the Tangled Depths.", "Complete events in the Tangled Depths."},
			{ "Contribute Spare Parts to kick off meta event in the Domain of Kourna.", "Contribute Spare Parts to kick off meta event in the Domain of Kourna."},
			{ "Use in the Drizzlewood Coast to spawn chests. Make sure you have required keys.", "Use in the Drizzlewood Coast to spawn chests. Make sure you have required keys."},
			{ "Use to unlock achievements (and play in Drizzlewood Coast)", "Use to unlock achievements (and play in Drizzlewood Coast)"},
			{ "Use to unlock achievements (and play  Drizzlewood Coast)}, or just delete/tp when you are done.", "Use to unlock achievements (and play  Drizzlewood Coast)}, or just delete/tp when you are done."},
			#endregion
			
			#region luck names
			{"Essence of Luck (fine)", "Essence of Luck (fine)" },
			{"Essence of Luck (masterwork)", "Essence of Luck (masterwork)" },
			{"Essence of Luck (rare)", "Essence of Luck (rare)" },
			{"Essence of Luck (exotic)", "Essence of Luck (exotic)" },
			{"Essence of Luck (legendary)", "Essence of Luck (legendary)" },
			#endregion

			#region Model strings
			{"Material Storage", "Material Storage" },
			{"Bank Storage", "Bank Storage"},
			{"Shared Storage", "Shared Storage" },
			{"This item only has value as part of a collection.", "This item only has value as part of a collection." },//TODO check API response
			{"Combine these items into stacks", "Combine these items into stacks" },
			{"Sell these items to a vendor", "Sell these items to a vendor" },
			{"Salvage these items", "Salvage these items" },
			{"Sell these items on the TP", "Sell these items on the TP" },
			{"Craft these items into higher luck tiers", "Craft these items into higher luck tiers" },
			{"Delete these items", "Delete these items" },
			{"Salvage Item","Salvage Item" }, //TODO check api response
			{"Feed these items to gobblers", "Feed these items to gobblers" },
			{"Consume these items for karma", "Consume these items for karma" },
			{"Consume these items for unbound magic", "Consume these items for unbound magic" },
			{"Consume these items for volatile magic", "Consume these items for volatile magic" },
			{"Convert these items to LWS4 currency", "Convert these items to LWS4 currency" },
			{"Craft these items", "Craft these items" },
			#endregion

			#region advice names
			{"stack advice", "stack advice" },
			{"vendor advice", "vendor advice" },
			{"rare salvage advice", "rare salvage advice" },
			{"craftable luck advice", "craftable luck advice" },
			{"deletable advice", "deletable advice" },
			{"salvagable  advice", "salvagable  advice" },
			{"consumable  advice", "consumable  advice" },
			{"gobbler  advice", "gobbler  advice" },
			{"karma consumable  advice", "karma consumable  advice" },
			{"crafting advice", "crafting advice" },
			{"living world advice", "living world advice" },
			{"miscellaneous  advice", "miscellaneous  advice" }
			#endregion
		};

		private static Dictionary<string, string> englishToSpanish = new Dictionary<string, string>
		{

		};

		private static Dictionary<string, string> englishToGerman = new Dictionary<string, string>
		{

		};

		private static Dictionary<string, string> englishToFrench = new Dictionary<string, string>
		{

		};

		private static Dictionary<string, string> englishToKorean = new Dictionary<string, string>
		{

		};

		private static Dictionary<string, string> englishToChinese = new Dictionary<string, string>
		{

		};

		#endregion

		#region list
		//ids of craftable luck items
		public static List<int> luckIds = new List<int> { 45175, 45176, 45177 };

		private static List<ItemType> nonStackableTypes = new List<ItemType> { ItemType.Armor, ItemType.Back, ItemType.Gathering, ItemType.Tool, ItemType.Trinket, ItemType.Weapon, ItemType.Bag, ItemType.Container };

		private static List<ItemType> salvagableEquipment = new List<ItemType> { ItemType.Armor, ItemType.Back, ItemType.Trinket, ItemType.Weapon };

		//recipe types pertinent to reducing inventory clutter
		private static List<ApiEnum<RecipeType>> pertinentRecipeTypes = new List<ApiEnum<RecipeType>> { RecipeType.Refinement, RecipeType.RefinementEctoplasm, RecipeType.RefinementObsidian, RecipeType.IngredientCooking };

		public static List<Gobbler> gobblers = new List<Gobbler>{
			new Gobbler(46731, 77093, 250), // Herta
            new Gobbler(46731, 66999, 50),  // Mawdrey
            new Gobbler(46733, 69887, 50),  // Princess
            new Gobbler(46735, 68369, 50),  // Star

            new Gobbler(83103, 83305, 25),  // Spearmarshall 
			};

		public static List<MiscAdvice> miscAdvices = new List<MiscAdvice>{
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

		public static List<int> karmaIds = new List<int>
		{
			86336, 85790, 41740, 38030, 86374, 86181, 36448, 36449, 92714, 95765, 36450, 36451,
			41738, 36456, 77652, 36457, 36458, 36459, 36460, 70244, 69939, 39127, 41373, 36461
		};

		public static List<int> lws3Id = new List<int>
		{
			79280, 79469, 79899, 80332, 81127, 81706
		};



		public static List<int> lws4Id = new List<int>
		{
			86069, 86977, 87645, 88955, 89537, 90783
		};

		public static List<int> ibsId = new List<int>
		{
			92272
		};
		#endregion

		#region function
		public static void set_locale(Locale newLocale_)
		{
			currentLocale = newLocale_;
		}

		public static bool is_luck_essence(int id_)
		{
			return luckNameMapping.ContainsKey(id_);
		}

		public static bool is_non_stackable_type(ItemType type_)
		{
			return nonStackableTypes.Contains(type_);
		}



		public static bool is_salvagable_equipment(ItemType type_)
		{
			return salvagableEquipment.Contains(type_);
		}

		public static bool is_pertinent_recipe(RecipeType recipe_)
		{
			return pertinentRecipeTypes.Contains(recipe_);
		}

		public static bool is_gameplay_consumable(int id_)
		{
			return gameplayConsumables.ContainsKey(id_);
		}

		public static bool is_karma_item(int id_)
		{
			return karmaIds.Contains(id_);
		}
		#endregion



		public static bool is_lws3_item(int id_)
		{
			return lws3Id.Contains(id_);
		}

		public static bool is_lws4_item(int id_)
		{
			return lws4Id.Contains(id_);
		}

		

		public static bool is_ibs_item(int id_)
		{
			return ibsId.Contains(id_);
		}

		
		
		public enum AdviceType
		{
			stackAdvice,
			vendorAdvice,
			rareSalvageAdvice,
			craftLuckAdvice,
			deletableAdvice,
			salvageAdvice,
			consumableAdvice,
			gobblerAdvice,
			karmaAdvice,
			craftingAdvice,
			lwsAdvice,
			miscAdvice
		}


		public enum StorageType
		{
			materialStorage,
			bankStorage,
			sharedStorage
		}


		

		public static  string get_string(StorageType storage_)
		{
			return get_current_translated_string(storageTypeNameMapping[storage_]);
		}

		public static  string get_string(AdviceType advice_)
		{
			return get_current_translated_string(adviceTypeNameMapping[advice_]);
		}

		public static  string get_string(CraftingDisciplineType discipline_)
		{
			return get_current_translated_string(disciplineNameMapping[discipline_]);
		}

		public static string get_string(int id_)
		{
			if(is_luck_essence(id_))
			{
				return get_current_translated_string(luckNameMapping[id_]);
			}

			if(is_gameplay_consumable(id_))
			{
				return get_current_translated_string(gameplayConsumables[id_]);
			}

			return "";
		}

		public static  int id_from_Render_URI(string uri_)
		{
			var initialSplit = uri_.Split('/');
			var file = initialSplit.Last();
			return Convert.ToInt32(file.Split('.')[0]);
		}

		
		public static  string get_current_translated_string(string tooltip_)
		{
			switch(currentLocale)
			{
				case Locale.English:
					return englishToEnglish[tooltip_];

				case Locale.Spanish:
					return englishToSpanish[tooltip_];

				case Locale.German:
					return englishToGerman[tooltip_];

				case Locale.French:
					return englishToFrench[tooltip_];

				case Locale.Korean:
					return englishToKorean[tooltip_];

				case Locale.Chinese:
					return englishToChinese[tooltip_];

				default:
					return englishToEnglish[tooltip_];
			}
		}
		

		
	}
}
