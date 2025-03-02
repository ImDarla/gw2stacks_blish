using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD.Content;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Overlay.UI.Presenters;
using Blish_HUD.Overlay.UI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Blish_HUD.GameServices;
using Blish_HUD.Settings.UI.Views;
using data;

namespace views
{
	class AdviceTempView :View
	{
		protected override void Build(Container buildPanel)
		{
			_ = new Image(AsyncTexture2D.FromAssetId(1025164))
			{
				SpriteEffects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically,
				Location = new Point(buildPanel.Width - 969, buildPanel.Height - 220),
				ClipsBounds = false,
				Parent = buildPanel
			};

			var infoPanel = new Panel()
			{
				Width = buildPanel.Width,
				Height = 32,
				Bottom = buildPanel.Bottom,
				Parent = buildPanel
			};

			var aboutPanel = new Panel()
			{
				ShowBorder = true,
				Width = buildPanel.Width,
				Height = buildPanel.Height - infoPanel.Height,
				Parent = buildPanel,
				CanScroll = true,
			};

			

			var lovePanel = new Panel()
			{
				Size = new Point(aboutPanel.Width - 128, 128),
				Left = (aboutPanel.Width / 2) - ((aboutPanel.Width - 128) / 3) - 24,
				Parent = aboutPanel,
			};

			var heart = new Image(AsyncTexture2D.FromAssetId(156127))
			{
				Size = new Point(64, 64),
				Location = new Point(0, lovePanel.Height / 2 - 32),
				Parent = lovePanel
			};

			_ = new Label()
			{
				Font = GameService.Content.DefaultFont16,
				Text = "test label",
				AutoSizeWidth = true,
				Height = lovePanel.Height,
				Left = heart.Right,
				VerticalAlignment = VerticalAlignment.Middle,
				StrokeText = true,
				Parent = lovePanel
			};
		}
	}

	class SourceView : View
	{
		


	}

    class AdviceTab : View
    {
		//shamelessly taken from OverlaySettings
		private ViewContainer GetStandardPanel(Panel rootPanel, string title,  int id_)
		{
			return new ViewContainer()
			{
				Icon = this.itemTextures[id_],
				WidthSizingMode = SizingMode.Fill,
				HeightSizingMode = SizingMode.AutoSize,
				Title = title+" click to see sources",
				ShowBorder = true,
				Parent = rootPanel
				
			};
		}

		List<ItemForDisplay> adviceList = new List<ItemForDisplay>();
		FlowPanel panel;
		Dictionary<int, AsyncTexture2D> itemTextures;

		private void BuildOverlaySettings(Panel rootPanel)
		{
			foreach (var item in this.adviceList)
			{
				if(this.itemTextures.ContainsKey(item.item.itemId)==false)
				{
					this.itemTextures.Add(item.item.itemId, AsyncTexture2D.FromAssetId(Magic.id_from_Render_URI(item.item.icon)));
				}
				var container = GetStandardPanel(rootPanel, item.item.name, item.item.itemId);
				/*var icon = new Image(AsyncTexture2D.FromAssetId(item.item.itemId))
				{
					Height = container.Height,
					Parent = container
				};
				var exampleButton = new StandardButton()
				{
					Text = "Details",
					Width = 50,
					Height = container.Height,
					Left = container.Right,
					Parent = container,
				};*/
				container.Show();
			}
			
			
			
		}

		public void update(List<ItemForDisplay> items_, Dictionary<int, AsyncTexture2D> itemTextures_)
		{
			this.panel.ClearChildren();
			this.adviceList = items_;
			this.itemTextures = itemTextures_;
			this.BuildOverlaySettings(panel);
		}

		protected override void Build(Container buildPanel)
		{
			this.panel = new FlowPanel()
			{
				WidthSizingMode = SizingMode.Fill,
				HeightSizingMode = SizingMode.Fill,
				FlowDirection = ControlFlowDirection.SingleTopToBottom,
				CanScroll = true,
				Parent = buildPanel
			};

			BuildOverlaySettings(this.panel);
		}

	}
}
