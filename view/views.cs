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
using Gw2Sharp.WebApi.V2.Models;
using SharpDX.DirectWrite;

namespace views
{
	
	class IgnoredView : View
	{
		FlowPanel panel =new FlowPanel()
		{
			WidthSizingMode = SizingMode.Fill,
				HeightSizingMode = SizingMode.Fill,
				FlowDirection = ControlFlowDirection.SingleTopToBottom,
				CanScroll = true,
			};
		Dictionary<int, AsyncTexture2D> itemTextures;

		
		Action<int, bool> callback;
		public List<int> excludedItemIds = new List<int>();

		TextBox search = new TextBox()
		{
			PlaceholderText = "Enter item name here ...",
			Size = new Point(358, 43),
			Font = GameService.Content.DefaultFont16,
			Location = new Point(0, 0)
		};
		string hunt = "";



		private void handle_text_input(object s_, EventArgs e_)
		{
			this.hunt = search.Text;
			this.refresh();
		}

		public void refresh()
		{
			this.update(this.panel.Title);
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

		private void build_ignored_panels(Panel rootPanel)
		{
			this.search.Parent = rootPanel;
			search.TextChanged += handle_text_input;
			foreach (var item in this.excludedItemIds)
			{
				if (string.IsNullOrEmpty(this.hunt) || Magic.get_local_name(item).ToLower().Contains(this.hunt.ToLower()))
				{
					if (this.itemTextures.ContainsKey(item) == false)
					{
						this.itemTextures.Add(item, AsyncTexture2D.FromAssetId(Magic.jsonLut.itemLut[item].IconId));
					}
					var container = GetStandardPanel(rootPanel, Magic.get_local_name(item), item);

					container.Click += (s, e) => this.callback(item, false);
					container.Show();
				}
				



			}
		}

		public void update(string title_)
		{
			this.search.Parent = null;
			this.search.TextChanged -= handle_text_input;
			this.panel.ClearChildren();
			this.build_ignored_panels(panel);
			this.panel.Title = title_;
			
		}

		public void set_values(Dictionary<int, AsyncTexture2D> itemTextures_, Action<int, bool> callback_, List<int> excludedItemIds_)
		{
			this.itemTextures = itemTextures_;
			this.callback = callback_;
			this.excludedItemIds = excludedItemIds_;
		}

		protected override void Build(Container buildPanel)
		{
			this.panel.Parent = buildPanel;
			build_ignored_panels(this.panel);
		}
	}


    class AdviceTabView : View
    {
		List<ItemForDisplay> adviceList = new List<ItemForDisplay>();
		FlowPanel panel = new FlowPanel()
		{
			WidthSizingMode = SizingMode.Fill,
			HeightSizingMode = SizingMode.Fill,
			FlowDirection = ControlFlowDirection.SingleTopToBottom,
			CanScroll = true
		};
		Dictionary<int, AsyncTexture2D> itemTextures;
		bool ignoredItemsFlag = false;

		
		Action<int, bool> callback;
		public List<int> excludedItemIds= new List<int>();

		TextBox search = new TextBox()
		{
			PlaceholderText = "Enter item name here ...",
			Size = new Point(358, 43),
			Font = GameService.Content.DefaultFont16,
			Location = new Point(0, 0)
		};
		string hunt = "";
		

		private void handle_text_input(object s_, EventArgs e_)
		{
			this.hunt = search.Text; 
			this.refresh();
		}

		public void refresh()
		{
			this.update(adviceList, this.panel.Title,this.ignoredItemsFlag);
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
			this.search.Parent = rootPanel;
			search.TextChanged += handle_text_input;
			
			search.Show();
			foreach (var item in this.adviceList)
			{
				if (this.itemTextures.ContainsKey(item.item.itemId) == false)
				{
					this.itemTextures.Add(item.item.itemId, AsyncTexture2D.FromAssetId(item.item.iconId));
				}

				if (string.IsNullOrEmpty(this.hunt)||Magic.get_local_name(item.item.itemId).ToLower().Contains(this.hunt.ToLower()))
				{
					if(this.excludedItemIds.Contains(item.item.itemId)==false)
					{
						var container = GetStandardPanel(rootPanel, Magic.get_local_name(item.item.itemId), item.item.itemId);
						if(item.gobblerId==0)
						{
							container.BasicTooltipText = Magic.get_current_translated_string(item.advice) + "\n" + item.get_source_string();
						}
						else
						{
							container.BasicTooltipText = Magic.get_current_translated_string(item.advice)+" ("+Magic.get_local_name(item.gobblerId)+")" + "\n" + item.get_source_string();
						}

						if (this.ignoredItemsFlag == true)
						{
							container.Click += (s, e) => this.callback(item.item.itemId, true);
						}
						
						
						container.Show();
					}
					/*
					if (this.ignoredItemsFlag == false)
					{
						var container = GetStandardPanel(rootPanel, Magic.get_local_name(item.item.itemId), item.item.itemId);
						container.BasicTooltipText = Magic.get_current_translated_string(item.advice) + "\n" + item.get_source_string();
						container.Show();
					}
					else
					{
						if (this.ignoredItemsFlag == true && this.ignoredItems.Contains(item.item.itemId) == false)
						{
							var container = GetStandardPanel(rootPanel, Magic.get_local_name(item.item.itemId), item.item.itemId);
							container.BasicTooltipText = Magic.get_current_translated_string(item.advice) + "\n" + item.get_source_string();
							container.Click += (s, e) => this.ignore(item.item.itemId);
							container.Show();
						}
					}*/
				}
				
			}
		}


		//update the panel and sub panels for the main window
		public void update(List<ItemForDisplay> items_, string title_, bool ignoredItemsFlag_)
		{
			this.search.Parent = null;
			this.search.TextChanged -= handle_text_input;
			this.panel.ClearChildren();
			this.ignoredItemsFlag = ignoredItemsFlag_;
			this.adviceList = items_;
			this.build_item_panels(panel);
			this.panel.Title = title_;

		}

		public void set_values(Dictionary<int, AsyncTexture2D> itemTextures_, Action<int, bool> callback_, List<int> excludedItemIds_)
		{
			this.itemTextures = itemTextures_;
			this.callback = callback_;
			this.excludedItemIds = excludedItemIds_;
		}

		//create the panel and sub panels for the main window
		protected override void Build(Container buildPanel)
		{
			this.panel.Parent = buildPanel;
			
			
			build_item_panels(this.panel);
		}


	}
}
