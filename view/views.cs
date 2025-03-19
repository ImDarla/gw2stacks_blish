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
using gw2stacks_blish.data;
using Blish_HUD.Input;

namespace views
{
	
	


    class AdviceTabView : View
    {
		List<ItemForDisplay> adviceList = new List<ItemForDisplay>();
		FlowPanel panel;
		Dictionary<int, AsyncTexture2D> itemTextures;


		

		private ViewContainer GetStandardPanel(Panel rootPanel, string title, int id_)
		{
			return new ViewContainer()
			{
				Icon = this.itemTextures[id_],
				WidthSizingMode = SizingMode.Fill,
				HeightSizingMode = SizingMode.AutoSize,
				Title = title,
				ShowBorder = true,
				Parent = rootPanel
				

			};
		}

		//build the sub panels for the main window
		private void build_item_panels(Panel rootPanel)
		{
			foreach (var item in this.adviceList)
			{
				if (this.itemTextures.ContainsKey(item.item.itemId) == false)
				{
					this.itemTextures.Add(item.item.itemId, AsyncTexture2D.FromAssetId(item.item.iconId));
				}
				var container = GetStandardPanel(rootPanel, Magic.get_local_name(item.item.itemId), item.item.itemId);
				container.BasicTooltipText = Magic.get_current_translated_string( item.advice)+"\n"+ item.get_source_string();
				container.Show();
			}
		}

		


		//update the panel and sub panels for the main window
		public void update(List<ItemForDisplay> items_, string title_, Dictionary<int, AsyncTexture2D> itemTextures_)//, StandardWindow sourceWindow_
		{
			this.panel.ClearChildren();
			this.adviceList = items_;
			this.itemTextures = itemTextures_;
			this.build_item_panels(panel);
			this.panel.Title = title_;

		}

		//create the panel and sub panels for the main window
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

			build_item_panels(this.panel);
		}


	}
}
