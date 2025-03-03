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
using data;
using reader;
using System.Collections.Generic;
using Blish_HUD.Input;
using Blish_HUD.Content;
using views;
using System.Diagnostics;

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

		SettingEntry<bool> showStackAdvice;
		SettingEntry<bool> showVendorAdvice;
		SettingEntry<bool> showRareSalvageAdvice;
		SettingEntry<bool> showCraftLuckAdvice;
		SettingEntry<bool> showDeletableAdvice;
		SettingEntry<bool> showSalvageAdvice;
		SettingEntry<bool> showConsumableAdvice;
		SettingEntry<bool> showGobblerAdvice;
		SettingEntry<bool> showKarmaAdvice;
		SettingEntry<bool> showCraftAdvice;
		SettingEntry<bool> showLwsAdvice;
		SettingEntry<bool> showMiscAdvice;

		Tab showStackAdviceTab;
		Tab showVendorAdviceTab;
		Tab showRareSalvageAdviceTab;
		Tab showCraftLuckAdviceTab;
		Tab showDeletableAdviceTab;
		Tab showSalvageAdviceTab;
		Tab showConsumableAdviceTab;
		Tab showGobblerAdviceTab;
		Tab showKarmaAdviceTab;
		Tab showCraftAdviceTab;
		Tab showLwsAdviceTab;
		Tab showMiscAdviceTab;

		Dictionary<SettingEntry<bool>, Tab> settingTabMapping;
		Dictionary<int, AsyncTexture2D> itemTextures;
		Model model;
		Gw2Api api;
		Dictionary<string, List<ItemForDisplay>> adviceDictionary;
		Task task = null;
		
		private Dictionary<SettingEntry<bool>,Tab> createTabMapping()
		{
			var result = new Dictionary<SettingEntry<bool>, Tab>();
			result.Add(showStackAdvice, showStackAdviceTab);
			result.Add(showVendorAdvice, showVendorAdviceTab);
			result.Add(showRareSalvageAdvice, showRareSalvageAdviceTab);
			result.Add(showCraftLuckAdvice, showCraftLuckAdviceTab);
			result.Add(showDeletableAdvice, showDeletableAdviceTab);
			result.Add(showSalvageAdvice, showSalvageAdviceTab);
			result.Add(showConsumableAdvice, showConsumableAdviceTab);
			result.Add(showGobblerAdvice, showGobblerAdviceTab);
			result.Add(showKarmaAdvice, showKarmaAdviceTab);
			result.Add(showCraftAdvice, showCraftAdviceTab);
			result.Add(showLwsAdvice, showLwsAdviceTab);
			result.Add(showMiscAdvice, showMiscAdviceTab);
			return result;
		}



		AdviceTabView adviceView;

		

		
		

		private void createTabs()
		{
			this.showStackAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "stack advice");
			this.showVendorAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "vendor advice");
			this.showRareSalvageAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "rare salvage advice");
			this.showCraftLuckAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "craftable luck advice");
			this.showDeletableAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "deletable advice");
			this.showSalvageAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "salvagable  advice");
			this.showConsumableAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "consumable  advice");
			this.showGobblerAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "gobbler  advice");
			this.showKarmaAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "karma consumable  advice");
			this.showCraftAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "crafting advice");
			this.showLwsAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "living world advice");
			this.showMiscAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.adviceView, "miscellaneous  advice");
			
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





		

		
		

		

		

		protected override void DefineSettings(SettingCollection settings) {
			showStackAdvice=settings.DefineSetting("showStackAdvice", true, () => " show stack advice", () => "toggle advice for partial stacks");
			showVendorAdvice=settings.DefineSetting("showVendorAdvice", true, () => " show vendor advice", () => "toggle advice for vendor sellable items");
			showRareSalvageAdvice=settings.DefineSetting("showRareSalvageAdvice", false, () => " show rare salvage advice", () => "toggle advice for salvagable, rare items");
			showCraftLuckAdvice=settings.DefineSetting("showCraftLuckAdvice", false, () => " show craftable luck advice", () => "toggle advice for craftable luck items");
			showDeletableAdvice=settings.DefineSetting("showDeletableAdvice", false, () => " show deletable advice", () => "toggle advice for deletable items");
			showSalvageAdvice=settings.DefineSetting("showSalvageAdvice", false, () => " show salvagable  advice", () => "toggle advice for salvagable items");
			showConsumableAdvice=settings.DefineSetting("showConsumableAdvice", false, () => " show consumable  advice", () => "toggle advice for consumable items");
			showGobblerAdvice=settings.DefineSetting("showGobblerAdvice", false, () => " show gobbler  advice", () => "toggle advice for gobbler items");
			showKarmaAdvice=settings.DefineSetting("showKarmaAdvice", false, () => " show karma consumable  advice", () => "toggle advice for karma items");
			showCraftAdvice=settings.DefineSetting("showCraftAdvice", false, () => " show crafting advice", () => "toggle advice for crafting");
			showLwsAdvice=settings.DefineSetting("showLwsAdvice", false, () => " show living world advice", () => "toggle advice for living world items");
			showMiscAdvice=settings.DefineSetting("showMiscAdvice", false, () => " show miscellaneous  advice", () => "");

		}

		private void create_values()
		{
			this.itemTextures = new Dictionary<int, AsyncTexture2D>();
			this.adviceDictionary = new Dictionary<string, List<ItemForDisplay>>();
			this.gw2stacks_root = new TabbedWindow2(
				AsyncTexture2D.FromAssetId(155997), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Rectangle(24, 30, 545, 630),              // The windowRegion
				new Rectangle(82, 30, 467, 600)               // The contentRegion
			);

			this.sourceWindow= new StandardWindow(
				AsyncTexture2D.FromAssetId(155985), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Rectangle(40, 26, 913, 691),              // The windowRegion
				new Rectangle(70, 71, 839, 605)               // The contentRegion
			);

			sourceWindow.Parent = GameService.Graphics.SpriteScreen;

			this.adviceView = new AdviceTabView();
			this.createTabs();
			this.settingTabMapping = this.createTabMapping();
			
			foreach (var tab in this.settingTabMapping.Values)
			{
				this.gw2stacks_root.Tabs.Add(tab);
			}
			
			GameService.Graphics.SpriteScreen.AddChild(gw2stacks_root);
			gw2stacks_root.Parent = GameService.Graphics.SpriteScreen;

			icon = new CornerIcon(AsyncTexture2D.FromAssetId(155052), "gw2stacks");
			GameService.Graphics.SpriteScreen.AddChild(icon);
			icon.Parent = GameService.Graphics.SpriteScreen;
			icon.Click += onClick2;

			loadingSpinner = new LoadingSpinner();
			loadingSpinner.Parent = GameService.Graphics.SpriteScreen;
			loadingSpinner.Location = icon.Location;
			loadingSpinner.Hide();


			this.model = new Model();
			this.api = new Gw2Api(Gw2ApiManager);
			icon.Show();
		}

        protected override void Initialize() {
			
		}

		

		

        protected override async Task LoadAsync() {
			
		}

		private void start_api_update()
		{
			if(this.running==false)
			{
				this.validData = false;
				this.gw2stacks_root.Hide();

				Logger.Warn("starting setup");
				task = Task.Run(() => this.model?.setup(this.api));
				this.running = true;
				this.loadingSpinner.Show();
				this.icon.Enabled = false;
			}
			
		}


		private void onClick2(object sender_, MouseEventArgs event_)
		{
			this.start_api_update();
			
		}

		
		

		private void update_views(string tabName_)
		{
			this.gw2stacks_root.Title = tabName_;
			this.adviceView.update(this.adviceDictionary[tabName_],tabName_,  this.itemTextures, this.sourceWindow);
		}

		private void on_tab_change(object sender_, ValueChangedEventArgs<Tab> event_)
		{
			if(validData==true)
			{
				var tabName = event_.NewValue.Name;
				this.update_views(tabName);
			}
		}


		protected override void OnModuleLoaded(EventArgs e) {
			
			// Base handler must be called
			base.OnModuleLoaded(e);

			this.create_values();
			
			this.gw2stacks_root.TabChanged += on_tab_change;
			
		}

        protected override void Update(GameTime gameTime) {

			this.loadingIntervalTicks += gameTime.ElapsedGameTime.Milliseconds;
			if(this.loadingIntervalTicks>100&&task!=null&&this.validData==false&&this.running==true)
			{
				if(task.IsCompleted)
				{
					Logger.Warn("task finished");
					this.running = false;

					this.update_advice();
					this.validData = true;
					this.update_views(this.gw2stacks_root.SelectedTab.Name);
					this.loadingSpinner.Hide();
					this.icon.Enabled = true;
					this.gw2stacks_root.Show();
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
