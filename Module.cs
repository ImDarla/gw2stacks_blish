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

        private TabbedWindow2 gw2stacks_root;

		private StandardWindow sourceWindow;

		private CornerIcon icon;

		private LoadingSpinner loadingSpinner;

		private double loadingIntervalTicks = 0;

		private double cooldownIntervalTicks = 0;

		private bool isOnCooldown = false;

		private bool ready = true;
		private bool validData = false;

		private bool running = false;

		private bool fatalError = false;

		private bool hasLut = false;



		SettingEntry<bool> includeConsumableSetting;

		
		Dictionary<int, AsyncTexture2D> itemTextures;
		Model model;
		Gw2Api api;
		Dictionary<string, List<ItemForDisplay>> adviceDictionary;
		Task task = null;
		
		



		AdviceTabView adviceView;

		private Dictionary<string, Tab> nameTabMapping;
		
		private void create_name_tab_mapping()
		{
			this.nameTabMapping = new Dictionary<string, Tab>
			{
				{Magic.get_string(Magic.AdviceType.stackAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.stackAdvice)) },
				{Magic.get_string(Magic.AdviceType.vendorAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.vendorAdvice)) },
				{Magic.get_string(Magic.AdviceType.rareSalvageAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.rareSalvageAdvice)) },
				{Magic.get_string(Magic.AdviceType.craftLuckAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.craftLuckAdvice)) },
				{Magic.get_string(Magic.AdviceType.deletableAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.deletableAdvice)) },
				{Magic.get_string(Magic.AdviceType.salvageAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.salvageAdvice)) },
				{Magic.get_string(Magic.AdviceType.consumableAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.consumableAdvice)) },
				{Magic.get_string(Magic.AdviceType.gobblerAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.gobblerAdvice)) },
				{Magic.get_string(Magic.AdviceType.karmaAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.karmaAdvice)) },
				{Magic.get_string(Magic.AdviceType.craftingAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.craftingAdvice)) },
				{Magic.get_string(Magic.AdviceType.lwsAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.lwsAdvice)) },
				{Magic.get_string(Magic.AdviceType.miscAdvice), new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, Magic.get_string(Magic.AdviceType.miscAdvice)) }
			};
		}

		

		
		

		

		private void update_advice()
		{
			this.adviceDictionary = new Dictionary<string, List<ItemForDisplay>>();
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.stackAdvice), model.get_stacks_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.vendorAdvice), model.get_vendor_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.rareSalvageAdvice), model.get_rare_salvage_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.craftLuckAdvice), model.get_craft_luck_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.deletableAdvice), model.get_just_delete_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.salvageAdvice), model.get_just_salvage_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.consumableAdvice), model.get_play_to_consume_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.gobblerAdvice), model.get_gobbler_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.karmaAdvice), model.get_karma_consumables_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.craftingAdvice), model.get_crafting_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.lwsAdvice), model.get_living_world_advice());
			this.adviceDictionary.Add(Magic.get_string(Magic.AdviceType.miscAdvice), model.get_misc_advice());
		}

		

		private async Task load_LUT()
		{
		
			try
			{
				
				#if FALLBACK
				string input = System.IO.File.ReadAllText(@"G:\LUT.json");
				var fallbackObject = JsonConvert.DeserializeObject<LUT>(input);
				var fallback = new HttpTest();
				fallback.RespondWithJson(fallbackObject);
				#endif
				Magic.jsonLut= await "https://bhm.blishhud.com/gw2stacks_blish/item_storage/LUT.json".WithHeader("User-Agent", "Blish-HUD").GetJsonAsync<LUT>();
				#if FALLBACK
				fallback.Dispose();
				#endif
				this.hasLut = true;
				Logger.Info("Lut successfully parsed");
			}
			catch(Exception e_)
			{
				this.fatalError = true;
				this.hasLut = false;
				Logger.Fatal("Unexpected exception: " + e_.Message);
				
			}
			
		}









		protected override void DefineSettings(SettingCollection settings) 
		{
			this.includeConsumableSetting = settings.DefineSetting("includeConsumables", true, () => " include consumables", () => "toggle to include food and utility");
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
				Logger.Fatal("Unexpected exception: " + e_.Message);
			}
		}


		private void create_window()
		{
			this.gw2stacks_root = new TabbedWindow2(
				AsyncTexture2D.FromAssetId(155997), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Microsoft.Xna.Framework.Rectangle(24, 30, 565, 630),              // The windowRegion
				new Microsoft.Xna.Framework.Rectangle(82, 30, 467, 600)               // The contentRegion
			);

			this.sourceWindow = new StandardWindow(
				AsyncTexture2D.FromAssetId(155985), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Microsoft.Xna.Framework.Rectangle(40, 26, 933, 691),              // The windowRegion
				new Microsoft.Xna.Framework.Rectangle(70, 71, 839, 605)               // The contentRegion
			);

			sourceWindow.Parent = GameService.Graphics.SpriteScreen;


			

			gw2stacks_root.Parent = GameService.Graphics.SpriteScreen;
			this.adviceView = new AdviceTabView();
			this.gw2stacks_root.Tabs.Clear();
			this.create_name_tab_mapping();

			foreach (var tab in this.nameTabMapping.Values)
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
			icon.Click += onClick2;

			loadingSpinner = new LoadingSpinner();
			loadingSpinner.Parent = GameService.Graphics.SpriteScreen;
			
			loadingSpinner.Hide();
			loadingSpinner.Enabled = false;
			
			this.model = new Model(Logger);
			this.api = new Gw2Api(Gw2ApiManager);
			icon.Show();
		}

		private void start_api_update()
		{
			if (this.running == false&&this.hasLut==true)
			{
				this.validData = false;
				this.gw2stacks_root.Hide();
				this.sourceWindow.Hide();
				loadingSpinner.Location = icon.Location;
				Logger.Info("starting setup");
				model.includeConsumables = this.includeConsumableSetting.Value;
				if(this.isOnCooldown==false)
				{
					this.task = null;
					task = Task.Run(() => this.model?.setup(this.api));
				}
				else
				{
					Logger.Info("on cooldown");
				}

					this.running = true;
				this.loadingSpinner.Show();
				this.icon.Enabled = false;
			}

		}


		private void onClick2(object sender_, MouseEventArgs event_)
		{
			this.validate_api();
			if (fatalError == false)
			{
				try
				{
					this.start_api_update();
				}
				catch (Exception e_)
				{
					this.fatalError = true;
					Logger.Fatal("Unexpected exception: " + e_.Message);
				}
			}


		}

		


		private void update_views(string tabName_)
		{
			this.gw2stacks_root.Title = tabName_;
			this.adviceView.update(this.adviceDictionary[tabName_], tabName_, this.itemTextures, this.sourceWindow);
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
					Logger.Fatal("Unexpected exception: " + e_.Message);
				}
			}
		}

		private void on_locale_change(object sender_, ValueEventArgs<System.Globalization.CultureInfo> event_)
		{
			//TODO implement
			//var newLocale = GameService.Overlay.UserLocale;
			//update locale in magic
			//update all written text
		}

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
				Logger.Fatal("Unexpected exception: " + e_.Message);
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
				if(task != null && this.validData == false && this.running == true&&this.fatalError==false)
				{
					if (task.IsCompleted)
					{
						Logger.Info("task finished");
						this.running = false;
						try
						{
							
							model.includeConsumables = this.includeConsumableSetting.Value;
							
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
							Logger.Fatal("Unexpected exception: " + e_.Message);
						}
						
					}
				}
				if (this.fatalError == true)//hide UI elements until fatalError is set to false by validate_api() upon subtoken change
				{
					gw2stacks_root?.Hide();
					this.sourceWindow?.Hide();
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
			this.sourceWindow?.Dispose();
			icon?.Dispose();
			loadingSpinner?.Dispose();
            // All static members must be manually unset
        }

    }

}
