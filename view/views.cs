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
		FlowPanel sourcePanel;
		Dictionary<int, AsyncTexture2D> itemTextures;
		StandardWindow sourceWindow;


		//event handler for subpanels in the main window
		private void on_click(object sender_, MouseEventArgs event_)
		{
			var containerSender = (ViewContainer)sender_;
			if(this.sourceWindow.Title==containerSender.Title)
			{
				this.sourceWindow.ToggleWindow();
			}
			else
			{
				this.sourceWindow.Title = containerSender.Title;
				this.update_source_window();
			}
				
		}

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
					this.itemTextures.Add(item.item.itemId, AsyncTexture2D.FromAssetId(Magic.id_from_Render_URI(item.item.icon)));
				}
				var container = GetStandardPanel(rootPanel, item.item.name, item.item.itemId);
				container.Click += this.on_click;
				container.Show();
			}
		}

		//build the sub panels for the source window
		private void build_source_panels(Panel rootPanel_, string name_)
		{
			ItemForDisplay item = null;
			foreach (var entry in  this.adviceList)
			{
				if(name_==entry.item.name)
				{
					item = entry;
					break;
				}
			}

			if(item == null)
			{
				
				throw new Exception("source panel fatal error: invalid item name supplied");
			}

			var advice = GetStandardPanel(rootPanel_, item.advice, item.item.itemId);
			advice.Show();

			foreach (var source in item.sources)
			{
				var container = GetStandardPanel(rootPanel_, source.ToString(), item.item.itemId);
				container.Show();
			}
			
		}

		//update the panel and sub panels for the source window
		private void update_source_window()
		{
			//this.sourceWindow.Hide();
			if(this.sourcePanel==null)
			{
				this.sourcePanel = new FlowPanel()
				{
					WidthSizingMode = SizingMode.Fill,
					HeightSizingMode = SizingMode.Fill,
					FlowDirection = ControlFlowDirection.SingleTopToBottom,
					CanScroll = true,
					Parent = this.sourceWindow
				};
			}
			this.sourcePanel.ClearChildren();
			this.build_source_panels(this.sourcePanel, this.sourceWindow.Title);
			this.sourceWindow.Show();
		}

		//update the panel and sub panels for the main window
		public void update(List<ItemForDisplay> items_, string title_, Dictionary<int, AsyncTexture2D> itemTextures_, StandardWindow sourceWindow_)
		{
			this.panel.ClearChildren();
			this.adviceList = items_;
			this.itemTextures = itemTextures_;
			this.build_item_panels(panel);
			this.panel.Title = title_;
			this.sourceWindow = sourceWindow_;
			this.sourceWindow.Hide();
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
