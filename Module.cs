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

		private CornerIcon icon;

		private double loadingIntervalTicks = 0;


		private bool validData = false;

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
		Dictionary<string, AdviceTab> stringAdviceTabMapping;
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



		AdviceTab stackView = new AdviceTab();

		private void createTabs()
		{
			this.showStackAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => this.stackView, "stack advice");
			this.showVendorAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "vendor advice");
			this.showRareSalvageAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "rare salvage advice");
			this.showCraftLuckAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "craftable luck advice");
			this.showDeletableAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "deletable advice");
			this.showSalvageAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "salvagable  advice");
			this.showConsumableAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "consumable  advice");
			this.showGobblerAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "gobbler  advice");
			this.showKarmaAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "karma consumable  advice");
			this.showCraftAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "crafting advice");
			this.showLwsAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "living world advice");
			this.showMiscAdviceTab=new Tab(ContentService.Content.GetTexture("155052"), () => new OverlaySettingsView(), "miscellaneous  advice");
			
		}

		private void selectTabs()
		{
			if(this.settingTabMapping==null)
			{
				this.settingTabMapping = this.createTabMapping();
			}
			foreach (var item in this.settingTabMapping.Keys)
			{
				if(item.Value)
				{
					if(this.gw2stacks_root.Tabs.Contains(this.settingTabMapping[item])==false)
					{
						this.gw2stacks_root.Tabs.Add(this.settingTabMapping[item]);
					}
					
					
				}
				else
				{
					if(this.gw2stacks_root.Tabs.Contains(this.settingTabMapping[item]))
					{
						this.gw2stacks_root.Tabs.Remove(this.settingTabMapping[item]);
						}
					
				}
				
			}
		}

		

		protected override void DefineSettings(SettingCollection settings) {
			showStackAdvice=settings.DefineSetting("showStackAdvice", true, () => " show stack advice", () => "toggle advice for partial stacks");
			showVendorAdvice=settings.DefineSetting("showVendorAdvice", false, () => " show vendor advice", () => "toggle advice for vendor sellable items");
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

        protected override void Initialize() {
			
		}

		

		

        protected override async Task LoadAsync() {
			
		}

		

		private void onClick(object sender_, MouseEventArgs event_)
		{
			if(this.gw2stacks_root.Visible==false)
			{
				this.selectTabs();
			}
			else
			{
				this.gw2stacks_root.Hide();
				this.selectTabs();
			}
			this.gw2stacks_root.Show();
			

		}

		private void onClick2(object sender_, MouseEventArgs event_)
		{
			this.validData = false;
			this.gw2stacks_root.Hide();
			
			Logger.Warn("starting setup");
			task=Task.Run(()=>this.model?.setup(this.api));
			//Logger.Warn(result[0].Name);
		}

		
		protected override void OnModuleLoaded(EventArgs e) {
			
			// Base handler must be called
			base.OnModuleLoaded(e);
			this.itemTextures = new Dictionary<int, AsyncTexture2D>();

			this.gw2stacks_root = new TabbedWindow2(
				AsyncTexture2D.FromAssetId(155997), // The background texture of the window.155997 1909316 GameService.Content.GetTexture("controls/window/502049")
				new Rectangle(24, 30, 545, 630),              // The windowRegion
				new Rectangle(82, 30, 467, 600)               // The contentRegion
			);

			this.createTabs();
			this.settingTabMapping=this.createTabMapping();
			
			
			
			
			GameService.Graphics.SpriteScreen.AddChild(gw2stacks_root);
			gw2stacks_root.Parent = GameService.Graphics.SpriteScreen;
			
			
			//gw2stacks_root.Show();
			this.selectTabs();
			icon = new CornerIcon(AsyncTexture2D.FromAssetId(155052), "gw2stacks");
			GameService.Graphics.SpriteScreen.AddChild(icon);
			icon.Show();
			icon.Parent = GameService.Graphics.SpriteScreen;
			icon.Click += onClick2;
			
			


			this.model = new Model();
			this.api = new Gw2Api(Gw2ApiManager);
			//this.task = Task.Run(() => model.setup(api));

		}

        protected override void Update(GameTime gameTime) {

			this.loadingIntervalTicks += gameTime.ElapsedGameTime.Milliseconds;
			if(this.loadingIntervalTicks>300&&task!=null&&this.validData==false)
			{
				if(task.IsCompleted)
				{
					Logger.Warn("task finished");
					this.validData = true;
					
					this.stackView.update(this.model.get_stacks_advice(), this.itemTextures);
					this.gw2stacks_root.Show();
				}
				this.loadingIntervalTicks = 0;
			}
			
		}

        /// <inheritdoc />
        protected override void Unload() {
			// Unload here
			//gw2stacks_root.ClearChildren();
			gw2stacks_root?.Dispose();
			icon?.Dispose();
            // All static members must be manually unset
        }

    }

}
