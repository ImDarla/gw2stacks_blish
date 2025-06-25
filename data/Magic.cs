using Blish_HUD;
using Blish_HUD.Controls;
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
	class Translation
	{
		public Translation()
		{ }

		public void add_translation(string english_, Dictionary<Locale, string> translation_)
		{
			this.englishToEnglish.Add(english_, translation_[Locale.English]);
			this.englishToSpanish.Add(english_, translation_[Locale.Spanish]);
			this.englishToKorean.Add(english_, translation_[Locale.Korean]);
			this.englishToFrench.Add(english_, translation_[Locale.French]);
			this.englishToChinese.Add(english_, translation_[Locale.Chinese]);
			this.englishToGerman.Add(english_, translation_[Locale.German]);
		}

		public  Dictionary<string, string> englishToEnglish = new Dictionary<string, string>
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

		public  Dictionary<string, string> englishToSpanish = new Dictionary<string, string>
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

		public  Dictionary<string, string> englishToGerman = new Dictionary<string, string>
		{
			
			#region discipline names
			{"Scribe", "Schreiber" },
			{"Tailor", "Schneider" },
			{"Leatherworker", "Lederer" },
			{"Weaponsmith", "Waffenschmied" },
			{"Armorsmith", "Rüstungsschmied" },
			{"Artificer", "Konstrukteur" },
			{"Chef", "Chefkoch" },
			{"Jeweler", "Juwelier" },
			{"Huntsman", "Waidmann" },
			{"Unknown", "Unknown" },
			#endregion
			
			#region miscAdvice
			{"Transform Quartz Crystals into a Charged Quartz Crystal at a place of power.", "Verwandelt 25 Quarzkristalle an einem Ort der Macht in einen aufgeladenen Quarzkristall." },
			{"Sift through silky sand.", "Siebt 10 Haufen Sand für die Chance, etwas Wertvolles zu finden." },
			{"Craft 'Completed Aetherkey'.", "Kombiniere zu 'Kompletter Ätherschlüssel'" },
			{"Consume to get War Supplies", "Verwende für Kriegs-Vorräte" },
			{"Consume to get Mordrem parts which can be exchanged for map currency", "Verwende für Mordrem Teile und tausche diese geben Banditen-Wappen ein" },
			{"Convert to Bauble Bubble", "Kombiniere zu Sphären-Blase" },
			{"Convert to Candy Corn Cob", "Kombiniere zu Candy-Corn-Kolben" },
			{"Convert to Jorbreaker", "Kombiniere zu Stück Jorzipan" },
			#endregion
			
			#region gameplay advice
			{ "Trade to get bounty for bandit leader.", "Tausche für Banditeanführer Kopfgeldaufträge ein."},
			{ "Have in inventory while defeating a bandit leader to spawn the Legendary Bandit Executioner", "Habe es beim besiegen eines Banditenanführers im Inventar um einen Lengendären Banditen-Henker zu beschwören"},
			{ "Use during a treasure hunt meta in Desert Highlands to spawn chests", "Verwende im Wüsten-Hochland in der Schatzsuche"},
			{ "Use in the Silverwastes after a meta completes to spawn chests. Make sure you have required keys.", "Verwende in der Silberwüste um Kisten auszugraben. Das Öffnen benötigt einen Schlüssel."},
			{ "Open a greater nightmare pod in the Silverwastes after completing meta.", "Verwende in der Silberwüste um die Größere Albtraum-Kapsel zu öffnen"},
			{ "Use during breach event in Silverwastes.", "Verwende während dem Bresche Event in der Silberwüste"},
			{ "Open Tarnished chest in Silverwastes.", "Öffne Angelaufene Kisten in der Silberwüste."},
			{ "Open krait Sunken Chests to progress a Master Diver achievement.", "Öffne Versunkene Krait Kisten."},
			{ "Open chests in the Aetherpath of the Twilight Arbor dungeon.", "Öffne Kisten im Ätherpfad des Zwielichtgartens"},
			{ "Complete this bounty.", "Absolviere diese Kopfgeldjagd."},
			{ "Complete events in the Verdant Brink.", "Absolviere Events in der Grasgrünen Schwelle"},
			{ "Complete events in the Auric Basin.", "Absolviere Events in dem Güldenen Talkessel."},
			{ "Complete events in the Tangled Depths.", "Absolviere Events in den Verschlungenen Tiefen"},
			{ "Contribute Spare Parts to kick off meta event in the Domain of Kourna.", "Verwende um das Meta Event in Kourna zu starten"},
			{ "Use in the Drizzlewood Coast to spawn chests. Make sure you have required keys.", "Verwende in der Nieselwald-Küste um Kisten zu finden. Das Öffnen benötigt einen Schlüssel."},
			{ "Use to unlock achievements (and play in Drizzlewood Coast)", "Verwende um Erfolge zu erlangen( oder spiele in der Nieselwald-Küste)"},
			{ "Use to unlock achievements (and play  Drizzlewood Coast)}, or just delete/tp when you are done.", "Verwende um Erfolge zu erlangen( oder spiele in der Nieselwald-Küste)}, oder lösche sie."},
			#endregion
			
			#region luck names
			{"Essence of Luck (fine)", "Essenz des Glücks (Edel)" },
			{"Essence of Luck (masterwork)", "Essenz des Glücks (Meisterwerk)" },
			{"Essence of Luck (rare)", "Essenz des Glücks (Selten)" },
			{"Essence of Luck (exotic)", "Essenz des Glücks (Exotisch)" },
			{"Essence of Luck (legendary)", "Essenz des Glücks (Legendär)" },
			#endregion

			#region Model strings
			{"Material Storage", "Materialienlager" },
			{"Bank Storage", "Bank"},
			{"Shared Storage", "Gemeinsamer Inventarplatz" },
			{"This item only has value as part of a collection.", "Dieser Gegenstand ist nur als Teil einer Sammlung wertvoll." },//TODO check API response
			{"Combine these items into stacks", "Stapel diese Items" },
			{"Sell these items to a vendor", "Verkaufe diese Items an einen NPC Händler" },
			{"Salvage these items", "Verwerte diese Items wieder" },
			{"Sell these items on the TP", "Verkaufe diese Items auf dem Handelsposten" },
			{"Craft these items into higher luck tiers", "Bau höhere Glücksessenzen " },
			{"Delete these items", "Zerstöre diese Items" },
			{"Salvage Item","Wiederverwertbarer Gegenstand" }, //TODO check api response
			{"Feed these items to gobblers", "Füttere diese Items an einen Konvertierer" },
			{"Consume these items for karma", "Verwende diese Items für Karma" },
			{"Consume these items for unbound magic", "Verwende diese Items für Entfesselte Magie" },
			{"Consume these items for volatile magic", "Verwende diese Items für Flüchtige Magie" },
			{"Convert these items to LWS4 currency", "Tausche diese Items into LW4 Währungen" },
			{"Craft these items", "Baue diese Items" },
			#endregion

			#region advice names
			{"stack advice", "stapel vorschlag" },
			{"vendor advice", "verkäufer vorschlag" },
			{"rare salvage advice", "seltener wiedervertbarer vorschlag" },
			{"craftable luck advice", "baubare glückessenz vorschlag" },
			{"deletable advice", "zerstörbarer vorschlag" },
			{"salvagable  advice", "wiedervertbarer vorschlag" },
			{"consumable  advice", "verwendbarer vorschlag" },
			{"gobbler  advice", "konvertierer vorschlag" },
			{"karma consumable  advice", "karma verwendbarer vorschlag" },
			{"crafting advice", "bau vorschlag" },
			{"living world advice", "lebendige welt vorschlag" },
			{"miscellaneous  advice", "diverser vorschlag" }
			#endregion
		};

		public  Dictionary<string, string> englishToFrench = new Dictionary<string, string>
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

		public  Dictionary<string, string> englishToKorean = new Dictionary<string, string>
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

		public  Dictionary<string, string> englishToChinese = new Dictionary<string, string>
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
	}

	class Magic
	{
		#region variables
		private static Locale currentLocale = Locale.English;
		public static int ectoId = 19721;
		public static double salvagePrice = 0.10496; //seemingly from mystic salvage kit??
		public static double ectoChance = 0.875;
		public static double tax = 0.85;
		public static LUT jsonLut = null;
		public static localeLut localeItemNamesLut = null;
		public static Logger log = null;
		public static Translation translation= new Translation();
		public static ItemInfo unknown = new ItemInfo()
		{
			Id = 0,
			Name = "Item unknown to LUT",
			IconId = 63369,
			Rarity = 0,
			Description = null,
			Type = 0,
			isFoodOrUtility = false,
			Flags = new List<int>(),
			Level = 0,
		};

		public static Item silkBag = new Item(9566, false, false, true)
		{

		};

		public static Item borealTrunk = new Item(92292, false, false, true)
		{

		};
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

		public static Dictionary<AdviceType, string> adviceTypeNameMapping = new Dictionary<AdviceType, string>
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



		public static Dictionary<StorageType, string> storageTypeNameMapping = new Dictionary<StorageType, string>
		{
			{StorageType.materialStorage,  "Material Storage"},
			{StorageType.bankStorage, "Bank Storage" },
			{StorageType.sharedStorage, "Shared Storage" }
		};

		public static Dictionary<int, CraftingMiscAdvice> craftingMiscAdvices = new Dictionary<int, CraftingMiscAdvice>
		{
			{9251, new CraftingMiscAdvice( new Dictionary<int, int>{{ 20008, 1 }, { 8439, 1 }, { 19997, 1 }, { 49871, 1 }, {48951, 1 }, {37214, 1 }, {19999, 1 }, {49872, 1 }, {20006, 1 }, {8446, 1 } }, "Craft: ", 9251 ) },//Enchanted Reward Boost , {20001, 1 }, karma booster removed
			{38050, new CraftingMiscAdvice( new Dictionary<int, int>{{ 20016, 1 }, { 20010, 1 }, { 20015, 1 }, { 20013, 1 } }, "Craft: ", 38050 ) }//Enchanted Combat Boost
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
			new Gobbler(77093, 46731, 250, "Herta"), // Herta
            new Gobbler(66999, 46731, 50, "Mawdrey"),  // Mawdrey
            new Gobbler(69887, 46733, 50, "Princess"),  // Princess
            new Gobbler(68369, 46735, 50, "Star of Gratitude"),  // Star
			new Gobbler(81780,new List<int>{46733, 46731, 46735} , 25, "Gleam of Sentience"),
			new Gobbler(101771,new List<int>{46733, 46731, 46735}, 25, "Portable Wizard's Tower Exchange"),
			new Gobbler(83305, 83103, 25, "Spearmarshal's Plea"),  // Spearmarshall 
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

	

		public static List<int> salvageIds = new List<int>
		{
			19721, 21653, 21654, 21655, 21656, 21657, 21658, 21659, 21660, 21661, 21662, 21663, 21664, 21665, 21666, 21667,
			21668, 21669, 21670, 21671, 21672, 21673, 21674, 21675, 21676, 21677, 21678, 21679, 21680, 21681, 21682, 21683,
			21684, 21685, 21686, 21687, 21688, 21689, 21690, 21691, 21692, 21693, 21694, 21695, 21941, 22325, 22326, 22327,
			22328, 22329, 22330, 22331, 43552, 43553, 43554, 43555, 43556, 45039, 45040, 45041, 45042, 45043, 66670, 68378,
			68379, 68380, 79079, 79138, 79213, 79423, 80681, 96464
		};

		public static List<int> collectionOnlyIds = new List<int>
		{
			67188, 67189, 67193, 67194, 67195, 70429, 70520, 70539, 70550, 70559, 70560, 70563, 70616, 70620, 70636, 70653,
			70667, 70674, 70690, 70738, 70753, 70760, 70775, 70776, 70782, 70812, 70862, 70875, 70879, 70900, 70914, 70947,
			71009, 71048, 71052, 71054, 71056, 71087, 71099, 71100, 71111, 71126, 71147, 71160, 71177, 71196, 71199, 71212, 71214,
			71231, 71244, 71253, 71270, 71274, 71293, 71304, 71310, 71319, 71373, 71401, 71442, 71460, 71465, 71471, 71476, 71527,
			71562, 71568, 71572, 71591, 71611, 71619, 71626, 71634, 71636, 71646, 71657, 71660, 71672, 71673, 71682, 71696, 71707,
			71713, 71727, 71733, 71751, 71761, 71778, 71799, 71821, 71834, 71869, 71871, 71881, 71883, 71902, 71907, 71913, 71922,
			71960, 71967, 71976, 71987, 71991, 72001, 72019, 72042, 72054, 72101, 72110, 72140, 72141, 72160, 72168, 72174, 72178,
			72183, 72218, 72235, 72257, 72290, 72298, 72321, 72328, 72329, 72348, 72352, 72394, 72399, 72410, 72412, 72440,
			72453, 72471, 72474, 72483, 72489, 72491, 72492, 72514, 72545, 72555, 72574, 72578, 72592, 72600, 72620, 72621,
			72623, 72634, 72661, 72668, 72692, 72701, 72737, 72787, 72817, 72855, 72870, 72876, 72879, 72883, 72888, 72903,
			72906, 72911, 72944, 72972, 72994, 73008, 73021, 73025, 73026, 73055, 73066, 73069, 73074, 73089, 73103, 73136,
			73138, 73164, 73208, 73212, 73250, 73252, 73300, 73338, 73388, 73401, 73418, 73426, 73463, 73468, 73500, 73513,
			73525, 73540, 73564, 73568, 73574, 73588, 73640, 73647, 73650, 73672, 73690, 73727, 73742, 73760, 73786, 73799,
			73819, 73829, 73833, 73864, 73876, 73924, 73950, 73954, 73972, 74011, 74027, 74040, 74082, 74088, 74098, 74099,
			74129, 74138, 74141, 74149, 74152, 74165, 74175, 74208, 74228, 74250, 74252, 74269, 74277, 74281, 74292, 74318,
			74336, 74368, 74395, 74422, 74428, 74453, 74454, 74461, 74469, 74511, 74514, 74517, 74529, 74533, 74553, 74560,
			74571, 74579, 74608, 74613, 74641, 74649, 74660, 74663, 74688, 74690, 74701, 74710, 74713, 74732, 74733, 74736,
			74742, 74756, 74758, 74779, 74781, 74792, 74813, 74853, 74857, 74888, 74891, 74917, 74932, 74993, 75027, 75036,
			75045, 75059, 75070, 75073, 75098, 75118, 75121, 75138, 75149, 75150, 75151, 75168, 75197, 75209, 75223, 75257,
			75280, 75281, 75293, 75300, 75302, 75310, 75319, 75352, 75357, 75366, 75419, 75424, 75445, 75459, 75479, 75489,
			75494, 75497, 75514, 75526, 75547, 75563, 75568, 75571, 75594, 75621, 75634, 75657, 75673, 75674, 75700, 75717,
			75722, 75751, 75757, 75788, 75806, 75814, 75822, 75834, 75836, 75842, 75849, 75863, 75875, 75931, 75936, 75947,
			75959, 75960, 75970, 75994, 76011, 76013, 76023, 76034, 76050, 76066, 76074, 76088, 76097, 76099, 76126, 76154,
			76187, 76260, 76268, 76276, 76278, 76308, 76341, 76346, 76373, 76382, 76386, 76400, 76413, 76421, 76440, 76452,
			76464, 76489, 76539, 76540, 76554, 76576, 76596, 76605, 76637, 76647, 76673, 76684, 76720, 76724, 76747, 76791,
			76808, 76818, 76835, 76838, 76851, 76852, 76856, 76869, 76895, 76911, 76945, 76963, 76973, 76995, 76997, 77003,
			77016, 77019, 77023, 77033, 77047, 77059, 77094, 77126, 77140, 77145, 77151, 77154, 77155, 77158, 77184, 77195,
			77213, 77224, 77228, 77231, 77235, 77290, 77343, 77348, 77398, 77445, 77452, 77467, 77584, 77650, 77659, 79731,
			80169, 80234, 80313, 80319, 80600, 82153, 82240, 82592, 83105, 83125, 83580, 83982, 84165, 84797, 95694, 95735,
			95806, 96440, 96460, 96584, 96622, 96767, 96859, 96956, 96963, 97090, 97224, 97381, 97455, 97585, 97641, 97718,
			100277, 100735
		};

		public static List<int> gaetingSalvage = new List<int>()
		{
			103800, 103999, 104049, 103970, 103908, 103888, 103844, 104052, 103819, 103954, 103835, 104006, 103895,
			104046, 103866, 103955, 103818,  104025, 103780, 104015, 103897, 103944
		};

		public static List<int> magnetiteSalvage = new List<int>()
		{
		  63420,63434,63435,63441,63442,77271,77272,77273,77280,77281,77284,77286,77288,77293,77294,77295,77300,77304,77306,77308,77309,77312,77313,77319,77320,77322,77323,77325,77327,77331,77337,77338,77340,77341,77342,77344,77346,77347,77358,77361,77363,77371,77376,77377,77380,77382,77383,77387,77390,77391,77395,77396,77406,77408,77409,77410,77412,77413,77420,77422,77425,77426,77427,77429,77430,77434,77436,77438,77440,77444,77448,77453,77454,77459,77461,77464,77466,77813,77827,77829,77830,77831,77836,77837,77839,77841,77842,77844,77847,77849,77851,77852,77853,77854,77855,77864,77867,77868,77873,
		  77874,77875,77876,77882,77885,77887,77889,77891,77894,77895,77896,77903,77904,77905,77907,77909,77910,77917,77918,77920,77922,77924,77933,77936,77939,77942,77943,77944,77946,77949,77950,77952,77954,77958,77959,77963,77964,77965,77966,77970,77972,77974,77977,77979,77986,77987,77988,77990,77992,77993,77999,78001,78002,78795,78796,78799,78800,78802,78805,78808,78809,78817,78818,78819,78820,78822,78823,78829,78834,78835,78843,78844,78847,78848,78852,78853,78855,78856,78858,78860,78862,78863,78865,78869,78874,78875,78876,78877,78878,78880,78881,78885,78888,78892,78895,78896,78901,78903,78907,
		  78911,78915,78916,78919,78923,78925,78927,78931,78934,78940,78941,78943,78945,78948,78949,78953,78955,78963,78964,78966,78967,78969,78971,78974,78977,78980,78981,78984,78985,78986,78987,78991,78992,78995,78997,79000,80099,80123,80180,80186,80188,80193,80194,80197,80199,80206,80235,80243,80249,80273,80278,80310,80330,80331,80333,80337,80341,80414,80461,80476,80482,80493,80499,80510,80527,80531,80537,80559,80568,80581,80605,80606,80611,80647,80659,80678,85629,85635,85636,85638,85641,85645,85651,85652,85654,85655,85657,85660,85661,85664,85669,85673,85674,85676,85677,85680,85683,85688,85691,85692,
		  85693,85696,85697,85698,85702,85703,85704,85705,85712,85714,85716,85722,85729,85732,85735,85736,85739,85741,85749,85753,85761,85763,85764,85766,85768,85770,85774,85777,85778,85782,85789,85791,85792,85795,85803,85807,85808,85814,85816,85819,85822,85823,85825,85827,85829,85831,85832,85835,85837,85840,85843,85846,85847,85853,85858,85859,85861,85867,85871,85876,85879,85883,85888,85889,85891,85892,85899,85903,85904,85914,85916,85931,85932,85935,85938,85940,85941,85944,85949,85951,85952,85954,85956,85957,85962,85963,85966,85969,85972,85976,85980,85982,85984,85986,85988,85992,86000,86004,86005,
		  86011,86013,86017,86021,86024,86029,86032,86035,86037,86038,86042,86044,86045,86049,86052,86056,86073,86079,86080,86084,86090,86100,86102,86103,86110,86112,86121,86123,86124,86128,86129,86135,86136,86138,86139,86141,86143,86147,86152,86158,86161,86164,86165,86176,86179,86185,86186,86187,86194,86195,86197,86198,86206,86209,86213,86215,86220,86224,86227,86234,86237,86240,86244,86246,86253,86255,86256,86259,86262,86263,86264,86268,86270,86273,86278,86288,86289,86295,86301,86307,86308,86311,86324,86325,86326,86333,86335,86340,86344,86345,86352,86361,86362,86363,86364,86365,86368,86370,86373,86380,86382,
		  88473,88478,88487,88562,88564,88582,88652,88677,88750,88795,88799,88804,88822,88839,88849,88854,88858,88862,88881,88888,88915,88916,88944,88966,91142,91144,91150,91156,91163,91167,91168,91179,91190,91198,91199,91201,91219,91223,91229,91230,91238,91239,91249,91263,91264,91265,91269,91271,103999
		};

		public static List<int> wizardGobblers = new List<int>()
		{66999,69887,68369,67270,72606,81512,79995,77093,73718,79197,79558,80144,81120,81780,83305,88660,80672,86360,49501,84440,78177,101771};

		public static List<int> wizardScrolls = new List<int>()
		{76630,71577,76827,73024,76293,70968,76065,95026,98007,79073,79456,79905,80238,81060,81673,79744,83305,85884,86981,87624,88934,88765,89621,90336,92694,91975,92411,93343,97009,100939,20030,78657,103998,92108 };

		public static List<ItemWeaponType> singularWeaponTypes = new List<ItemWeaponType>()
		{ItemWeaponType.Rifle, ItemWeaponType.Greatsword, ItemWeaponType.Hammer, ItemWeaponType.LongBow, ItemWeaponType.ShortBow, ItemWeaponType.Speargun, ItemWeaponType.Staff, ItemWeaponType.Trident, ItemWeaponType.Harpoon, ItemWeaponType.Focus, ItemWeaponType.Scepter, ItemWeaponType.Torch, ItemWeaponType.Shield, ItemWeaponType.Warhorn };

		
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

		public static int levenshtein_distance(ReadOnlySpan<char> left_, ReadOnlySpan<char> right_)
		{
			if(left_.Length==0)
			{
				return right_.Length;
			}
			if(right_.Length==0)
			{
				return left_.Length;
			}
			if(left_[0]==right_[0])
			{
				return levenshtein_distance(left_.Slice(1), right_.Slice(1));
			}
			var a = levenshtein_distance(left_.Slice(1), right_);
			var b = levenshtein_distance(left_, right_.Slice(1));
			var c = levenshtein_distance(left_.Slice(1), right_.Slice(1));
			return 1 + Math.Min(a, Math.Min(b,c));
		}

		public static bool string_similar(string left_, string right_)
		{
			var splitLeft = left_.Split(' ');
			var splitRight = right_.Split(' ');
			if(splitLeft.Count()!=splitRight.Count())
			{
				return false;
			}
			int result = 0;
			foreach (var sub in splitLeft.Zip(splitRight, (leftSub, rightSub)=>levenshtein_distance(new ReadOnlySpan<char>(leftSub.ToCharArray()), new ReadOnlySpan<char>(rightSub.ToCharArray()))))
			{
				result += sub;
			}
			if(result<=5)
			{
				return true;
			}
			else
			{
				return false;
			}
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


		public static string get_local_storage_name(string storage_)
		{
			if(storageTypeNameMapping.ContainsValue(storage_))
			{
				return get_current_translated_string(storage_);
			}
			else
			{
				return storage_;
			}
		}

		public static  string get_local_storage(StorageType storage_)
		{
			return get_current_translated_string(storageTypeNameMapping[storage_]);
		}

		public static  string get_local_advice(AdviceType advice_)
		{
			return get_current_translated_string(adviceTypeNameMapping[advice_]);
		}

		public static  string get_local_discipline(CraftingDisciplineType discipline_)
		{
			return get_current_translated_string(disciplineNameMapping[discipline_]);
		}

		public static string get_local_consumable_id_name(int id_)
		{
			if (is_gameplay_consumable(id_))
			{
				return get_current_translated_string(gameplayConsumables[id_]);
			}
			else
			{
				return "invalid id requested: consumable localisation";
			}
		}

		public static string get_local_name(int id_)
		{
			if(is_luck_essence(id_))
			{
				return get_current_translated_string(luckNameMapping[id_]);
			}
			else
			{
				if(localeItemNamesLut.english.ContainsKey(id_)==false)
				{
					if(id_==97873)
					{
						return "Amber Runestone";
					}
					return "invalid id: " + id_;
				}

				switch (currentLocale)
				{
					case Locale.English:
						return localeItemNamesLut.english[id_];

					case Locale.Spanish:
						return localeItemNamesLut.spanish[id_];

					case Locale.German:
						return localeItemNamesLut.german[id_];

					case Locale.French:
						return localeItemNamesLut.french[id_];

					case Locale.Korean:
						return localeItemNamesLut.korean[id_];

					case Locale.Chinese:
						return localeItemNamesLut.chinese[id_];

					default:
						return localeItemNamesLut.english[id_];
				}
			}



				
		}

		public static  int id_from_Render_URI(string uri_)
		{
			if(uri_==null||uri_=="")
			{
				return 63369;//when in doubt then cabbage
			}
			var initialSplit = uri_.Split('/');
			var file = initialSplit.Last();
			return Convert.ToInt32(file.Split('.')[0]);
		}

		private static string handle_missing_translation(string tooltip_, string message_)
		{
			log.Warn(message_ + ": " + tooltip_);
			return message_;
		}

		public static  string get_current_translated_string(string tooltip_)
		{
			if(tooltip_==null)
			{
				return null;
			}
			switch(currentLocale)
			{
				case Locale.English:
					return translation.englishToEnglish.ContainsKey(tooltip_)? translation.englishToEnglish[tooltip_]: handle_missing_translation(tooltip_, "no english local");

				case Locale.Spanish:
					return translation.englishToSpanish.ContainsKey(tooltip_) ? translation.englishToSpanish[tooltip_] : handle_missing_translation(tooltip_, "no spanish local");

				case Locale.German:
					return translation.englishToGerman.ContainsKey(tooltip_) ? translation.englishToGerman[tooltip_] : handle_missing_translation(tooltip_, "no german local");

				case Locale.French:
					return translation.englishToFrench.ContainsKey(tooltip_) ? translation.englishToFrench[tooltip_] : handle_missing_translation(tooltip_, "no french local");

				case Locale.Korean:
					return translation.englishToKorean.ContainsKey(tooltip_) ? translation.englishToKorean[tooltip_] : handle_missing_translation(tooltip_, "no korean local");

				case Locale.Chinese:
					return translation.englishToChinese.ContainsKey(tooltip_) ? translation.englishToChinese[tooltip_] : handle_missing_translation(tooltip_, "no chinese local");

				default:
					return translation.englishToEnglish.ContainsKey(tooltip_) ? translation.englishToEnglish[tooltip_] : handle_missing_translation(tooltip_, "no english local");
			}
		}
		

		
	}

	class LUT
	{
		public Dictionary<int, ItemInfo> itemLut = new Dictionary<int, ItemInfo>();

		public Dictionary<int, RecipeInfo> recipeLut = new Dictionary<int, RecipeInfo>();

		public LUT()
		{

		}
	}

	class localeLut
	{
		public Dictionary<int, string> english = new Dictionary<int, string>();
		public Dictionary<int, string> german = new Dictionary<int, string>();
		public Dictionary<int, string> french = new Dictionary<int, string>();
		public Dictionary<int, string> spanish = new Dictionary<int, string>();
		public Dictionary<int, string> korean = new Dictionary<int, string>();
		public Dictionary<int, string> chinese = new Dictionary<int, string>();

		public localeLut()
		{
			
		}
	}
}
