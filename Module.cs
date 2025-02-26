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

        private StandardWindow ui;

        protected override void DefineSettings(SettingCollection settings) {
            
        }

        protected override void Initialize() {
			
		}

        protected override async Task LoadAsync() {
            
        }

        protected override void OnModuleLoaded(EventArgs e) {
			
			// Base handler must be called
			base.OnModuleLoaded(e);
			

			ui = new StandardWindow(GameService.Content.GetTexture("controls/window/155985"), new Rectangle(40, 26, 913, 691), new Rectangle(70, 71, 839, 605))
			{
				Parent = gw2stacks_root,
				Title = "StandardWindow",
				Emblem = GameService.Content.GetTexture("controls/window/156022"),
				Subtitle = "Example Subtitle",
				SavesPosition = true,
				Id = $"_ExampleModule_38d37290-b5f9-447d-97ea-45b0b50e5f56"
			};
			gw2stacks_root.AddChild(ui);
			ui.Show();
			var exampleButton = new StandardButton()
			{
				Text = "Click Me!",
				Size = new Point(400, 400),
				Location = new Point(30, 30),
				Parent = gw2stacks_root,
			};
			gw2stacks_root.AddChild(exampleButton);
			exampleButton.Show();

			GameService.Graphics.SpriteScreen.AddChild(gw2stacks_root);
			gw2stacks_root.Parent = GameService.Graphics.SpriteScreen;
			gw2stacks_root.Show();
			
        }

        protected override void Update(GameTime gameTime) {


			
		}

        /// <inheritdoc />
        protected override void Unload() {
			// Unload here
			//gw2stacks_root.ClearChildren();
			gw2stacks_root?.Dispose();
            // All static members must be manually unset
        }

    }

}
