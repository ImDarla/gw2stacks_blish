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


		private bool validData = false;

		private bool running = false;

		private bool fatalError = false;



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
				{"stack advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "stack advice") },
				{"vendor advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "vendor advice") },
				{"rare salvage advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "rare salvage advice") },
				{"craftable luck advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "craftable luck advice") },
				{"deletable advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "deletable advice") },
				{"salvagable  advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "salvagable  advice") },
				{"consumable  advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "consumable  advice") },
				{"gobbler  advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "gobbler  advice") },
				{"karma consumable  advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "karma consumable  advice") },
				{"crafting advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "crafting advice") },
				{"living world advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "living world advice") },
				{"miscellaneous  advice", new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "miscellaneous  advice") }
			};
		}

		

		
		

		

		private void update_advice()
		{
			if (this.adviceDictionary == null)
			{
				this.adviceDictionary = new Dictionary<string, List<ItemForDisplay>>();
			}
			this.adviceDictionary.Clear();
			this.adviceDictionary.Add("stack advice", model.get_stacks_advice());
			this.adviceDictionary.Add("vendor advice", model.get_vendor_advice());
			this.adviceDictionary.Add("rare salvage advice", model.get_rare_salvage_advice());
			this.adviceDictionary.Add("craftable luck advice", model.get_craft_luck_advice());
			this.adviceDictionary.Add("deletable advice", model.get_just_delete_advice());
			this.adviceDictionary.Add("salvagable  advice", model.get_just_salvage_advice());
			this.adviceDictionary.Add("consumable  advice", model.get_play_to_consume_advice());
			this.adviceDictionary.Add("gobbler  advice", model.get_gobbler_advice());
			this.adviceDictionary.Add("karma consumable  advice", model.get_karma_consumables_advice());
			this.adviceDictionary.Add("crafting advice", model.get_crafting_advice());
			this.adviceDictionary.Add("living world advice", model.get_living_world_advice());
			this.adviceDictionary.Add("miscellaneous  advice", model.get_misc_advice());
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


		private void create_values()
		{
			this.itemTextures = new Dictionary<int, AsyncTexture2D>();
			this.adviceDictionary = new Dictionary<string, List<ItemForDisplay>>();
			this.gw2stacks_root = new TabbedWindow2(
				AsyncTexture2D.FromAssetId(155997), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Microsoft.Xna.Framework.Rectangle(24, 30, 565, 630),              // The windowRegion
				new Microsoft.Xna.Framework.Rectangle(82, 30, 467, 600)               // The contentRegion
			);

			this.sourceWindow= new StandardWindow(
				AsyncTexture2D.FromAssetId(155985), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Microsoft.Xna.Framework.Rectangle(40, 26, 933, 691),              // The windowRegion
				new Microsoft.Xna.Framework.Rectangle(70, 71, 839, 605)               // The contentRegion
			);

			sourceWindow.Parent = GameService.Graphics.SpriteScreen;

			this.adviceView = new AdviceTabView();
			this.create_name_tab_mapping();
			
			foreach (var tab in this.nameTabMapping.Values)
			{
				this.gw2stacks_root.Tabs.Add(tab);
			}
			
			
			gw2stacks_root.Parent = GameService.Graphics.SpriteScreen;

			icon = new CornerIcon(AsyncTexture2D.FromAssetId(155052), "gw2stacks");
			
			icon.Parent = GameService.Graphics.SpriteScreen;
			icon.Click += onClick2;

			loadingSpinner = new LoadingSpinner();
			loadingSpinner.Parent = GameService.Graphics.SpriteScreen;
			
			loadingSpinner.Hide();
			loadingSpinner.Enabled = false;
			
			this.model = new Model();
			this.api = new Gw2Api(Gw2ApiManager);
			icon.Show();
		}

		private void start_api_update()
		{
			if (this.running == false)
			{
				this.validData = false;
				this.gw2stacks_root.Hide();
				loadingSpinner.Location = icon.Location;
				Logger.Warn("starting setup");
				model.includeConsumables = this.includeConsumableSetting.Value;
				task = Task.Run(() => this.model?.setup(this.api));
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
				var tabName = event_.NewValue.Name;
				this.update_views(tabName);
			}
		}

		protected override void Initialize() {
			
		}

		

		

        protected override async Task LoadAsync() {
			
		}

		


		protected override void OnModuleLoaded(EventArgs e) {
			
			// Base handler must be called
			base.OnModuleLoaded(e);

			try
			{
				this.create_values();
				this.gw2stacks_root.TabChanged += on_tab_change;
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
			if(this.loadingIntervalTicks>100)
			{
				if(task != null && this.validData == false && this.running == true&&this.fatalError==false)
				{
					if (task.IsCompleted)
					{
						Logger.Warn("task finished");
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
						}
						catch(Exception e_)
						{
							this.fatalError = true;
							Logger.Fatal("Unexpected exception: " + e_.Message);
						}
						
					}
				}
				if (this.fatalError == true)//hide UI elements until fatalError is set to false by validate_ai() upon subtoken change
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
