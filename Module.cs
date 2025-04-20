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
using System.Linq;
using Blish_HUD.Controls.Intern;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using Key = Blish_HUD.Controls.Extern.VirtualKeyShort;
using Keyboard = Blish_HUD.Controls.Intern.Keyboard;
using Mouse = Blish_HUD.Controls.Intern.Mouse;
using Blish_HUD.Controls.Extern;

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

		private TabbedWindow2 gw2stacksWindow;

		private TabbedWindow2 ignoredItemsWindow;

		private StandardWindow characterBasedWindow;

		private CornerIcon icon;

		private LoadingSpinner loadingSpinner;

		private double loadingIntervalTicks = 0;

		private double cooldownIntervalTicks = 0;

		private bool isOnCooldown = false;

		private bool validData = false;

		private bool running = false;

		private bool fatalError = false;

		private bool hasLut = false;

		private bool ignoreItemsFlag = false;

		SettingEntry<bool> includeConsumableSetting;

		SettingEntry<bool> localJson;

		SettingEntry<bool> ignoreItemsFeature;

		SettingEntry<string> displayType;

		Dictionary<int, AsyncTexture2D> itemTextures = new Dictionary<int, AsyncTexture2D>();

		Model model;

		Gw2Api api;

		Dictionary<string, List<ItemForDisplay>> adviceDictionary = new Dictionary<string, List<ItemForDisplay>>();

		List<ItemForDisplay> combinedAdvice = new List<ItemForDisplay>();

		AdviceTabView adviceView;

		IgnoredView ignoredView;

		CharacterView characterBasedView;

		ItemView itemView;

		private Dictionary<Tab, string> tabNameMapping;

		Tab ignoredItemsTab;

		List<int> ignoredItemList = new List<int>();


		List<int> excludedItemIds = new List<int>();

		#endregion

		#region populate

		private void create_name_tab_mapping()
		{
			this.tabNameMapping = new Dictionary<Tab, string>
			{
				{new Tab(AsyncTexture2D.FromAssetId(358447), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.stackAdvice]),Magic.adviceTypeNameMapping[Magic.AdviceType.stackAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(255379), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.vendorAdvice]),Magic.adviceTypeNameMapping[Magic.AdviceType.vendorAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(157091), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.rareSalvageAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.rareSalvageAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(536054), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.craftLuckAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.craftLuckAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(102597), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.deletableAdvice]),Magic.adviceTypeNameMapping[Magic.AdviceType.deletableAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(156660), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.salvageAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.salvageAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(157123), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.consumableAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.consumableAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(156658), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.gobblerAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.gobblerAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(1494404), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.karmaAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.karmaAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(156685), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.craftingAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.craftingAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(156722), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.lwsAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.lwsAdvice] },
				{new Tab(AsyncTexture2D.FromAssetId(157099), () => this.adviceView, Magic.adviceTypeNameMapping[Magic.AdviceType.miscAdvice]), Magic.adviceTypeNameMapping[Magic.AdviceType.miscAdvice]  }
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
				
				DirectoryReader dir = new DirectoryReader(path);

				if (dir.FileExists("LUT.json") == false ||
				dir.FileExists("localeItemLUT.json") == false ||
				dir.FileExists("chineseLocal.json") == false ||
				dir.FileExists("englishLocal.json") == false ||
				dir.FileExists("germanLocal.json") == false ||
				dir.FileExists("koreanLocal.json") == false ||
				dir.FileExists("spanishLocal.json") == false ||
				dir.FileExists("frenchLocal.json") == false)
				{
					local = false;
				}

				DirectoryReader directory = new DirectoryReader(path);
				if(directory.FileExists("ignoredItemsList.json")==true)
				{
					var input = System.IO.File.ReadAllText(path + "/ignoredItemsList.json");
					this.ignoredItemList = JsonConvert.DeserializeObject<List<int>>(input);
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
				Logger.Warn("Unexpected exception: " + "can't create LUT" + " @" + e_.StackTrace);
			}

		}

		protected override void DefineSettings(SettingCollection settings)
		{
			this.includeConsumableSetting = settings.DefineSetting("includeConsumables", true, () => " include consumables", () => "toggle to include food and utility");
			this.localJson = settings.DefineSetting("localLut", false, () => "use a local item json", () => "will only have an effect if a LUT exists inside the gw2stacks folder");
			this.ignoreItemsFeature = settings.DefineSetting("ignoreItems", false, () => "blacklist", () => "enable the blacklist feature for item advice");
			this.displayType = settings.DefineSetting("UI version", "0", () => "", () => "Choose the UI version\n0 for classic gw2stacks\n1 for character based advice\n2 for item specific advice ");
		}

		private void create_window()
		{
			this.gw2stacksWindow = new TabbedWindow2(
				AsyncTexture2D.FromAssetId(155997), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Microsoft.Xna.Framework.Rectangle(24, 30, 565, 630),              // The windowRegion
				new Microsoft.Xna.Framework.Rectangle(82, 30, 467, 600)               // The contentRegion
			);

			this.gw2stacksWindow.Location = new Point((GameService.Graphics.SpriteScreen.Width / 4) * 1, GameService.Graphics.SpriteScreen.Height*1 / 4);
			this.gw2stacksWindow.Hidden += (s, e) => this.ignoredItemsWindow?.Hide();

			this.ignoredItemsWindow = new TabbedWindow2(
				AsyncTexture2D.FromAssetId(155997), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Microsoft.Xna.Framework.Rectangle(24, 30, 565, 630),              // The windowRegion
				new Microsoft.Xna.Framework.Rectangle(82, 30, 467, 600)               // The contentRegion
			);

			

			this.ignoredItemsWindow.Location = new Point((GameService.Graphics.SpriteScreen.Width / 4) * 2, GameService.Graphics.SpriteScreen.Height *1/ 4);
			this.ignoredView = new IgnoredView();
			this.ignoredItemsTab = new Tab(ContentService.Content.GetTexture("155052"), () => this.ignoredView, Magic.adviceTypeNameMapping[Magic.AdviceType.stackAdvice]);
			this.ignoredItemsWindow.Tabs.Add(ignoredItemsTab);
			this.ignoredItemsWindow.Parent = GameService.Graphics.SpriteScreen;
			this.ignoredView.set_values(this.itemTextures, this.refresh_views, this.excludedItemIds);

			gw2stacksWindow.Parent = GameService.Graphics.SpriteScreen;
			this.adviceView = new AdviceTabView();
			this.adviceView.set_values(this.itemTextures, this.refresh_views, this.excludedItemIds);
			this.gw2stacksWindow.Tabs.Clear();
			this.create_name_tab_mapping();

			foreach (var tab in this.tabNameMapping.Keys)
			{
				this.gw2stacksWindow.Tabs.Add(tab);
			}

			this.gw2stacksWindow.TabChanged += on_tab_change;

			this.characterBasedWindow = new StandardWindow(
				AsyncTexture2D.FromAssetId(155985), // The background texture of the window.
				new Microsoft.Xna.Framework.Rectangle(40, 26, 913, 691),              // The windowRegion
				new Microsoft.Xna.Framework.Rectangle(70, 71, 839, 605)               // The contentRegion
			);
			this.characterBasedWindow.Hide();
			this.characterBasedView = new CharacterView();
			this.characterBasedWindow.Parent= GameService.Graphics.SpriteScreen;
			this.characterBasedWindow.Title = "Gw2stacks";
			GameService.Gw2Mumble.PlayerCharacter.NameChanged += this.on_character_change;

			this.itemView = new ItemView();
			this.itemView.set_values(itemTextures, combinedAdvice);

		}
		private async void on_mouse_alt_click(object s = null, MouseEventArgs e = null)
		{
			
			if(GameService.Input.Keyboard.KeysDown.Contains(Keys.LeftAlt))
			{
				Logger.Warn("registered click combo");
				var position=GameService.Input.Mouse.Position;

				//TODO timings
				Mouse.Release(MouseButton.LEFT, -1, -1, true);
				Keyboard.Release(Key.LMENU, true);

				await Task.Delay(50);
				Keyboard.Press(Key.LSHIFT, true);
				await Task.Delay(50);
				Mouse.Click(MouseButton.LEFT, -1, -1, true);
				await Task.Delay(50);
				Keyboard.Release(Key.LSHIFT, true);
				
				await Task.Delay(50);
				Keyboard.Press(Key.LCONTROL, true);
				await Task.Delay(50);
				Keyboard.Stroke(Key.KEY_C, true);
				await Task.Delay(50);
				Keyboard.Release(Key.LCONTROL, true);
				await Task.Delay(50);
				Keyboard.Stroke(Key.BACK, true);
				Keyboard.Stroke(Key.RETURN, true);
				string entry = await ClipboardUtil.WindowsClipboardService.GetTextAsync();
				Logger.Info("Found entry: " + entry);
				var label = new Label()
				{
					Text = "I'm just a Label\nMultiline works too!",
					Size = new Point(300, 100),
					Location = position,
					Parent = GameService.Graphics.SpriteScreen,
					Enabled=false
				};

			}
		}

		private void create_values()
		{


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
			foreach (var tab in gw2stacksWindow.Tabs)
			{
				tab.Name = Magic.get_current_translated_string(this.tabNameMapping[tab]);
			}
			this.ignoredItemsTab.Name = Magic.get_current_translated_string("Ignored Items");
		}

		private void refresh_views(int id_, bool mainWindow_)
		{
			if(this.ignoredItemList.Contains(id_)==true)
			{
				this.ignoredItemList.Remove(id_);
			}
			else
			{
				if(mainWindow_==true)
				{
					this.ignoredItemList.Add(id_);
				}
				
			}
			this.update_excluded();
			this.ignoredView.refresh();
			this.adviceView.refresh();

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
			this.combinedAdvice.Clear();
			foreach (var item in this.adviceDictionary.Values)
			{
				this.combinedAdvice.AddRange(item);
			}
		}

		public void show_windows()
		{
			int mode = 0;
			GameService.Input.Mouse.LeftMouseButtonPressed -= on_mouse_alt_click;
			try
			{
				mode = Convert.ToInt32(this.displayType.Value);
			}
			catch(Exception e)
			{
				Logger.Warn("Invalid mode selection. Defaulting to mode 0");
				mode = 0;
			}

			switch(mode)
			{
				case 0:
					this.gw2stacksWindow.Show();
					if (this.ignoreItemsFlag == true)
					{
						this.ignoredItemsWindow.Show();
					}
					break;
				case 1:
					this.characterBasedWindow.Show(this.characterBasedView);
					break;
				case 2:
					this.characterBasedWindow.Show(this.itemView);
					//GameService.Input.Mouse.LeftMouseButtonPressed += on_mouse_alt_click;
					break;
				default:
					this.gw2stacksWindow.Show();
					if (this.ignoreItemsFlag == true)
					{
						this.ignoredItemsWindow.Show();
					}
					break;
			}
			
			
		}

		public void  hide_windows()
		{
			this.characterBasedWindow?.Hide();
			this.ignoredItemsWindow?.Hide();
			this.gw2stacksWindow?.Hide();
			//GameService.Input.Mouse.LeftMouseButtonPressed -= on_mouse_alt_click;
		}

		private void validate_api(object s = null, ValueEventArgs<IEnumerable<TokenPermission>> e =null)
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
						Logger.Warn("Missing Permissions");
					}
				}
				else
				{
					this.fatalError = true;
					Logger.Warn("No subtoken supplied");
				}
				
				
			}
			catch (Gw2Sharp.WebApi.Exceptions.InvalidAccessTokenException e_)
			{
				this.fatalError = true;
				Logger.Warn("Invalid access token: " + e_.Message);
			}
			catch (Gw2Sharp.WebApi.Exceptions.MissingScopesException e_)
			{
				this.fatalError = true;
				Logger.Warn("Missing scopes: " + e_.Message);
			}
			catch (Gw2Sharp.WebApi.Exceptions.RequestException e_)
			{
				this.fatalError = true;
				Logger.Warn("Request exception: " + e_.Message);
			}
			catch(Exception e_)
			{
				this.fatalError = true;
				Logger.Warn("Unexpected exception: " + e_.Message+ " @"+ e_.StackTrace);
			}
		}

		private async Task start_api_update()
		{
			if (this.running == false&&this.hasLut==true)
			{
				this.icon.Enabled = false;
				this.validData = false;
				this.hide_windows();
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
					Logger.Warn("Unexpected exception: " + e_.Message + " @" + e_.StackTrace);
				}
			}
		}

		private void update_views(string tabName_)
		{
			this.gw2stacksWindow.Title = tabName_;
			this.adviceView.update(this.adviceDictionary[tabName_], tabName_, this.ignoreItemsFlag);//, this.sourceWindow
			this.ignoredItemsWindow.Title = Magic.get_current_translated_string("Ignored Items");
			this.ignoredView.update(Magic.get_current_translated_string("Ignored Items"));
			this.characterBasedView.update(this.itemTextures, this.combinedAdvice, GameService.Gw2Mumble.PlayerCharacter.Name);
			this.itemView.update();
			
		}

		
		private void on_character_change(object sender_, ValueEventArgs<string> e_)
		{
			this.characterBasedView.update(this.itemTextures, this.combinedAdvice, e_.Value??"invalid name");
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
					Logger.Warn("Unexpected exception: " + e_.Message+ " @"+ e_.StackTrace);
				}
			}
		}

		private void update_excluded()
		{
			if(this.hasLut==true)
			{
				this.excludedItemIds.Clear();
				if(this.ignoreItemsFeature.Value==true)
				{
					this.excludedItemIds.AddRange(this.ignoredItemList);
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
				Magic.log = Logger;
			}
			catch (Exception e_)
			{
				this.fatalError = true;
				Logger.Warn("Unexpected exception: " + e_.Message+ " @"+ e_.StackTrace);
			}
			

			this.validate_api();
			this.Gw2ApiManager.SubtokenUpdated += this.validate_api;
			
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
							this.update_excluded();
							this.ignoreItemsFlag = this.ignoreItemsFeature.Value;
							this.validData = true;
							this.update_views(this.gw2stacksWindow.SelectedTab.Name);
							this.loadingSpinner.Hide();
							this.icon.Enabled = true;
							this.show_windows();
							if (this.isOnCooldown==false)
							{
								this.isOnCooldown = true;
								this.cooldownIntervalTicks = 0;
							}
							
							
							
							


						}
						catch(Exception e_)
						{
							this.fatalError = true;
							Logger.Warn("Unexpected exception: " + e_.Message+ " @"+ e_.StackTrace);
						}
						
					}
				}
				if (this.fatalError == true)//hide UI elements until fatalError is set to false by validate_api() upon subtoken change
				{
					this.hide_windows();
					icon?.Hide();
					loadingSpinner?.Hide();
				}
				this.loadingIntervalTicks = 0;
			}
			
		}

        /// <inheritdoc />
        protected override void Unload() {
			// Unload here
			gw2stacksWindow?.Dispose();
			this.ignoredItemsWindow?.Hide();
			this.characterBasedWindow?.Dispose();
			icon?.Dispose();
			loadingSpinner?.Dispose();
			try
			{
				var path = DirectoryUtil.RegisterDirectory("gw2stacks");
				var output = JsonConvert.SerializeObject(this.ignoredItemList);
				System.IO.File.WriteAllText(path + "/ignoredItemsList.json", output);
			}
			catch(Exception e_)
			{
				Logger.Warn("Unexpected exception: " + "can't save ignored items" + " @" + e_.StackTrace);
			}
			this.Gw2ApiManager.SubtokenUpdated -= this.validate_api;
			GameService.Input.Mouse.LeftMouseButtonPressed -= on_mouse_alt_click;
			GameService.Gw2Mumble.PlayerCharacter.NameChanged -= this.on_character_change;
			//TODO change event behaviour to unregister and take default arguments
			// All static members must be manually unset
		}

    }

}
