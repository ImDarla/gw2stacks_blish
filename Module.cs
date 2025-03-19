//#define FALLBACK
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Blish_HUD.GameServices;
using Blish_HUD.Gw2WebApi;
using Blish_HUD.Overlay.UI.Views;
using System.Runtime.Remoting.Lifetime;
using System.Collections.Generic;
using Blish_HUD.Input;
using Blish_HUD.Content;
using views;
using System.Diagnostics;
using Gw2Sharp.WebApi.V2.Models;
using System.Collections;
using gw2stacks_blish.data;
using gw2stacks_blish.reader;
using Gw2Sharp.WebApi.V2.Clients;
using Newtonsoft.Json;
using Flurl.Http.Testing;
using Flurl.Http;

namespace gw2stacks_blish {

    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module {

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        
        #region Service Managers
        internal SettingsManager    SettingsManager    => this.ModuleParameters.SettingsManager;
        internal ContentsManager    ContentsManager    => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager      Gw2ApiManager      => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

		#region variables

		private TabbedWindow2 gw2stacks_root;

		private CornerIcon icon;

		private LoadingSpinner loadingSpinner;

		private double loadingIntervalTicks = 0;

		private double cooldownIntervalTicks = 0;

		private bool isOnCooldown = false;

		private bool validData = false;

		private bool running = false;

		private bool fatalError = false;

		private bool hasLut = false;

		SettingEntry<bool> includeConsumableSetting;

		SettingEntry<bool> localJson;

		Dictionary<int, AsyncTexture2D> itemTextures;

		Model model;

		Gw2Api api;

		Dictionary<string, List<ItemForDisplay>> adviceDictionary;

		AdviceTabView adviceView;

		private Dictionary<Tab, string> tabNameMapping;

		#endregion

		#region populate

		private void create_name_tab_mapping()
		{
			this.tabNameMapping = new Dictionary<Tab, string>
			{
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.stackAdvice]),Magic.adviceTypeNameMapping[Magic.AdviceType.stackAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.vendorAdvice]),Magic.adviceTypeNameMapping[Magic.AdviceType.vendorAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.rareSalvageAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.rareSalvageAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.craftLuckAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.craftLuckAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.deletableAdvice]),Magic.adviceTypeNameMapping[Magic.AdviceType.deletableAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.salvageAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.salvageAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.consumableAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.consumableAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.gobblerAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.gobblerAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.karmaAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.karmaAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.craftingAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.craftingAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.lwsAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.lwsAdvice] },
				{new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.miscAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.miscAdvice]  }
			};


		}

		private async Task load_LUT()
		{

			try
			{
				bool local = this.localJson.Value;
				var path = this.DirectoriesManager.GetFullDirectoryPath("gw2stacks");
				if (path == null)
				{
					path =DirectoryUtil.RegisterDirectory("gw2stacks");
				}
				else
				{
					DirectoryReader dir = new DirectoryReader(path);

					if (dir.FileExists("LUT.json") == false||
					dir.FileExists("localeItemLUT.json")==false||
					dir.FileExists("chineseLocal.json")==false||
					dir.FileExists("englishLocal.json")==false||
					dir.FileExists("germanLocal.json")==false||
					dir.FileExists("koreanLocal.json")==false||
					dir.FileExists("spanishLocal.json")==false||
					dir.FileExists("frenchLocal.json")==false)
					{
						local = false;
					}

				}

				if (local ==true)
				{
					Logger.Info("Loading local LUT");
					var input = System.IO.File.ReadAllText(path + "/LUT.json");
					Magic.jsonLut = JsonConvert.DeserializeObject<LUT>(input);
					var localeInput = System.IO.File.ReadAllText(path + "/localeItemLUT.json");
					Magic.localeItemNamesLut = JsonConvert.DeserializeObject<localeLut>(localeInput);
					var chineseLocal = System.IO.File.ReadAllText(path + "/chineseLocal.json");
					Magic.englishToChinese = JsonConvert.DeserializeObject<Dictionary<string,string>>(chineseLocal);
					var englishLocal = System.IO.File.ReadAllText(path + "/englishLocal.json");
					Magic.englishToEnglish = JsonConvert.DeserializeObject<Dictionary<string, string>>(englishLocal);
					var germanLocal = System.IO.File.ReadAllText(path + "/germanLocal.json");
					Magic.englishToGerman = JsonConvert.DeserializeObject<Dictionary<string, string>>(germanLocal);
					var koreanLocal = System.IO.File.ReadAllText(path + "/koreanLocal.json");
					Magic.englishToKorean = JsonConvert.DeserializeObject<Dictionary<string, string>>(koreanLocal);
					var spanishLocal = System.IO.File.ReadAllText(path + "/spanishLocal.json");
					Magic.englishToSpanish = JsonConvert.DeserializeObject<Dictionary<string, string>>(spanishLocal);
					var frenchLocal = System.IO.File.ReadAllText(path + "/frenchLocal.json");
					Magic.englishToFrench = JsonConvert.DeserializeObject<Dictionary<string, string>>(frenchLocal);
				}
				else
				{
					Logger.Info("Loading remote LUT");
					Magic.jsonLut = await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/LUT.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<LUT>();
					Magic.localeItemNamesLut = await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/localeItemLUT.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<localeLut>();
					Magic.englishToChinese = await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/chineseLocal.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<Dictionary<string, string>>();
					Magic.englishToEnglish = await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/englishLocal.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<Dictionary<string, string>>();
					Magic.englishToGerman = await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/germanLocal.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<Dictionary<string, string>>();
					Magic.englishToKorean = await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/koreanLocal.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<Dictionary<string, string>>();
					Magic.englishToSpanish = await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/spanishLocal.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<Dictionary<string, string>>();
					Magic.englishToFrench = await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/frenchLocal.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<Dictionary<string, string>>();
				}

				

				this.hasLut = true;
				Logger.Info("Lut successfully parsed");
			}
			catch (Exception e_)
			{
				this.fatalError = true;
				this.hasLut = false;
				Logger.Fatal("Unexpected exception: " + e_.Message + " @" + e_.StackTrace);
			}

		}

		protected override void DefineSettings(SettingCollection settings)
		{
			this.includeConsumableSetting = settings.DefineSetting("includeConsumables", true, () => " include consumables", () => "toggle to include food and utility");
			this.localJson = settings.DefineSetting("localLut", false, () => "use a local item json", () => "will only have an effect if a LUT exists inside the gw2stacks folder");
		}

		private void create_window()
		{
			this.gw2stacks_root = new TabbedWindow2(
				AsyncTexture2D.FromAssetId(155997), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Microsoft.Xna.Framework.Rectangle(24, 30, 565, 630),              // The windowRegion
				new Microsoft.Xna.Framework.Rectangle(82, 30, 467, 600)               // The contentRegion
			);


			gw2stacks_root.Parent = GameService.Graphics.SpriteScreen;
			this.adviceView = new AdviceTabView();
			this.gw2stacks_root.Tabs.Clear();
			this.create_name_tab_mapping();

			foreach (var tab in this.tabNameMapping.Keys)
			{
				this.gw2stacks_root.Tabs.Add(tab);
			}

			this.gw2stacks_root.TabChanged += on_tab_change;
		}

		private void create_values()
		{
			this.itemTextures = new Dictionary<int, AsyncTexture2D>();
			this.adviceDictionary = new Dictionary<string, List<ItemForDisplay>>();


			icon = new CornerIcon(AsyncTexture2D.FromAssetId(155052), "gw2stacks");

			icon.Parent = GameService.Graphics.SpriteScreen;
			icon.Click += async (s, e) => await on_click();

			loadingSpinner = new LoadingSpinner()
			{
				Size = new Point(48, 48)
			};
			loadingSpinner.Parent = GameService.Graphics.SpriteScreen;

			loadingSpinner.Hide();
			loadingSpinner.Enabled = false;

			this.model = new Model(Logger);
			this.api = new Gw2Api(Gw2ApiManager);
			icon.Show();
		}
		#endregion

		#region update

		private void update_tab_locale()
		{
			Magic.set_locale(GameService.Overlay.UserLocale.Value);
			foreach (var tab in gw2stacks_root.Tabs)
			{
				tab.Name = Magic.get_current_translated_string(this.tabNameMapping[tab]);
			}
		}

		private void update_advice()
		{
			//Magic.set_locale(GameService.Overlay.UserLocale.Value);
			this.adviceDictionary = new Dictionary<string, List<ItemForDisplay>>();
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.stackAdvice), model.get_stacks_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.vendorAdvice), model.get_vendor_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.rareSalvageAdvice), model.get_rare_salvage_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.craftLuckAdvice), model.get_craft_luck_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.deletableAdvice), model.get_just_delete_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.salvageAdvice), model.get_just_salvage_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.consumableAdvice), model.get_play_to_consume_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.gobblerAdvice), model.get_gobbler_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.karmaAdvice), model.get_karma_consumables_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.craftingAdvice), model.get_crafting_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.lwsAdvice), model.get_living_world_advice());
			this.adviceDictionary.Add(Magic.get_local_advice(Magic.AdviceType.miscAdvice), model.get_misc_advice());
		}

		private void validate_api()
		{
			try
			{
				if(Gw2ApiManager.HasSubtoken)
				{
					if (Gw2ApiManager.HasPermissions(new List<TokenPermission> { TokenPermission.Account, TokenPermission.Characters, TokenPermission.Inventories }))
					{
						this.fatalError = false;
						Logger.Info("Api validated successfully");
						this.icon.Show();
					}
					else
					{
						this.fatalError = true;
						Logger.Fatal("Missing Permissions");
					}
				}
				else
				{
					this.fatalError = true;
					Logger.Fatal("No subtoken supplied");
				}
				
				
			}
			catch (Gw2Sharp.WebApi.Exceptions.InvalidAccessTokenException e_)
			{
				this.fatalError = true;
				Logger.Fatal("Invalid access token: " + e_.Message);
			}
			catch (Gw2Sharp.WebApi.Exceptions.MissingScopesException e_)
			{
				this.fatalError = true;
				Logger.Fatal("Missing scopes: " + e_.Message);
			}
			catch (Gw2Sharp.WebApi.Exceptions.RequestException e_)
			{
				this.fatalError = true;
				Logger.Fatal("Request exception: " + e_.Message);
			}
			catch(Exception e_)
			{
				this.fatalError = true;
				Logger.Fatal("Unexpected exception: " + e_.Message+ " @"+ e_.StackTrace);
			}
		}

		private async Task start_api_update()
		{
			if (this.running == false&&this.hasLut==true)
			{
				this.icon.Enabled = false;
				this.validData = false;
				this.gw2stacks_root.Hide();
				loadingSpinner.Location = icon.Location;
				Logger.Info("starting setup");
				model.includeConsumables = this.includeConsumableSetting.Value;
				this.loadingSpinner.Show();
				if (this.isOnCooldown==false)
				{
					this.model?.reset_state();
					await this.model?.setup(this.api);
				}
				else
				{
					Logger.Info("on cooldown");
				}

					this.running = true;
			}

		}

		private async Task on_click()
		{
			this.validate_api();
			if (fatalError == false)
			{
				try
				{
					await this.start_api_update();
				}
				catch (Exception e_)
				{
					this.fatalError = true;
					Logger.Fatal("Unexpected exception: " + e_.Message + " @" + e_.StackTrace);
				}
			}
		}

		private void update_views(string tabName_)
		{
			this.gw2stacks_root.Title = tabName_;
			this.adviceView.update(this.adviceDictionary[tabName_], tabName_, this.itemTextures);//, this.sourceWindow
		}

		private void on_tab_change(object sender_, ValueChangedEventArgs<Tab> event_)
		{
			if (validData == true)
			{
				
				try
				{
					var tabName = event_.NewValue.Name;
					this.update_views(tabName);
				}
				catch (Exception e_)
				{
					this.fatalError = true;
					Logger.Fatal("Unexpected exception: " + e_.Message+ " @"+ e_.StackTrace);
				}
			}
		}

		#endregion


		protected override void Initialize() {
			
		}

		

		

        protected override async Task LoadAsync() {
			await this.load_LUT();
		}

		


		protected override void OnModuleLoaded(EventArgs e) {
			
			// Base handler must be called
			base.OnModuleLoaded(e);

			try
			{
				this.create_window();
				this.create_values();
				
			}
			catch (Exception e_)
			{
				this.fatalError = true;
				Logger.Fatal("Unexpected exception: " + e_.Message+ " @"+ e_.StackTrace);
			}
			

			this.validate_api();
			this.Gw2ApiManager.SubtokenUpdated += (s_, e_) => { this.validate_api(); };
			
		}

        protected override void Update(GameTime gameTime) {

			this.loadingIntervalTicks += gameTime.ElapsedGameTime.Milliseconds;

			this.cooldownIntervalTicks += gameTime.ElapsedGameTime.Milliseconds;

			if(this.cooldownIntervalTicks>300000)//5 min cooldown
			{
				if(this.isOnCooldown == true)
				{
					
					this.isOnCooldown = false;
					Logger.Info("Cooldown over");
				}
				this.cooldownIntervalTicks = 0;
			}

			if(this.loadingIntervalTicks>100)
			{
				if(this.validData == false && this.running == true&&this.fatalError==false)
				{
					if (this.model.validData)
					{
						Logger.Info("task finished");
						this.running = false;
						try
						{
							
							model.includeConsumables = this.includeConsumableSetting.Value;
							this.update_tab_locale();
							this.update_advice();
							
							
							this.validData = true;
							this.update_views(this.gw2stacks_root.SelectedTab.Name);
							this.loadingSpinner.Hide();
							this.icon.Enabled = true;
							this.gw2stacks_root.Show();
							if(this.isOnCooldown==false)
							{
								this.isOnCooldown = true;
								this.cooldownIntervalTicks = 0;
							}
							
						}
						catch(Exception e_)
						{
							this.fatalError = true;
							Logger.Fatal("Unexpected exception: " + e_.Message+ " @"+ e_.StackTrace);
						}
						
					}
				}
				if (this.fatalError == true)//hide UI elements until fatalError is set to false by validate_api() upon subtoken change
				{
					gw2stacks_root?.Hide();
					
					icon?.Hide();
					loadingSpinner?.Hide();
				}
				this.loadingIntervalTicks = 0;
			}
			
		}

        /// <inheritdoc />
        protected override void Unload() {
			// Unload here
			gw2stacks_root?.Dispose();
			
			icon?.Dispose();
			loadingSpinner?.Dispose();
            // All static members must be manually unset
        }

    }

}
