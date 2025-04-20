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
using System.Text.RegularExpressions;

namespace views
{
	class ItemView : View
	{
		FlowPanel panel = new FlowPanel()
		{
			WidthSizingMode = SizingMode.Fill,
			HeightSizingMode = SizingMode.Fill,
			FlowDirection = ControlFlowDirection.SingleTopToBottom,
			CanScroll = true,
		};
		Dictionary<int, AsyncTexture2D> itemTextures;

		public List<ItemForDisplay> combinedAdvice = new List<ItemForDisplay>();

		TextBox search = new TextBox()
		{
			PlaceholderText = "Enter item name here ...",
			Size = new Point(830, 43),
			Font = GameService.Content.DefaultFont16,
			Location = new Point(0, 0)
		};
		string hunt = "";

		

		private void handle_text_input(object s_, EventArgs e_)
		{

			//TODO add removal of numbers
			this.hunt = search.Text;//.Replace("[", string.Empty).Replace("]", string.Empty);
			this.update();
		}

		private ViewContainer GetStandardPanel(Panel rootPanel, string title, int id_)
		{
			return new ViewContainer()
			{
				Icon = this.itemTextures[id_],
				//WidthSizingMode = SizingMode.Fill,
				Width=830,
				HeightSizingMode = SizingMode.AutoSize,
				Title = title,
				ShowBorder = true,
				Parent = rootPanel


			};
		}

		private void build_item_panels(Panel rootPanel)
		{
			this.search.Parent = rootPanel;
			this.search.TextChanged += handle_text_input;
			foreach (var item in this.combinedAdvice)
			{
				if (string.IsNullOrEmpty(this.hunt) || Magic.get_local_name(item.get_id()).ToLower().Contains(this.hunt.ToLower()))//||item.get_chatlink()==hunt
				{
					if (this.itemTextures.ContainsKey(item.get_id()) == false)
					{
						this.itemTextures.Add(item.get_id(), AsyncTexture2D.FromAssetId(item.get_iconId()));//Magic.jsonLut.itemLut[item.get_id()].IconId
					}
					var container = GetStandardPanel(rootPanel, Magic.get_local_name(item.get_id()), item.get_id());
					container.BasicTooltipText = item.ToString();
					container.Show();
				}

			}
		}

		public void update()
		{
			this.search.Parent = null;
			this.panel.ClearChildren();
			this.search.TextChanged -= handle_text_input;
			this.build_item_panels(panel);
			this.panel.Title = "Gw2stacks";

		}

		public void set_values(Dictionary<int, AsyncTexture2D> itemTextures_, List<ItemForDisplay> excludedItemIds_)
		{
			this.itemTextures = itemTextures_;
			this.combinedAdvice = excludedItemIds_;
		}

		protected override void Build(Container buildPanel)
		{
			this.panel.Parent = buildPanel;
			build_item_panels(this.panel);
		}
	}

	class CharacterView : View
	{
		string characterName = "";
		Dictionary<int, AsyncTexture2D> itemTextures;
		List<ItemForDisplay> adviceList;
		FlowPanel panel = new FlowPanel()
		{
			WidthSizingMode = SizingMode.Fill,
			HeightSizingMode = SizingMode.Fill,
			FlowDirection = ControlFlowDirection.SingleTopToBottom,
			CanScroll = true,
		};

		
		public void update(Dictionary<int, AsyncTexture2D> itemTextures_, List<ItemForDisplay> items_, string names_)
		{
			this.itemTextures = itemTextures_;
			this.adviceList = items_;
			this.characterName = names_;
			build_inventory_panel();
		}

		private void build_inventory_panel()
		{
			var parent = this.panel.Parent;
			this.panel.Parent = null;
			this.panel = new FlowPanel()
			{
				WidthSizingMode = SizingMode.Fill,
				HeightSizingMode = SizingMode.Fill,
				FlowDirection = ControlFlowDirection.LeftToRight,
				CanScroll = true,
			};
			this.panel.Parent = parent;
			var filtered = this.adviceList.Where(item => item.sources.Any(source => source.place==this.characterName)).ToList();
			this.panel.Title = this.characterName;
			foreach (var advice in filtered)
			{
				if (this.itemTextures.ContainsKey(advice.get_id()) == false)
				{
					this.itemTextures.Add(advice.get_id(), AsyncTexture2D.FromAssetId(advice.get_iconId()));
				}
				

				var container = new Image()
				{
					Texture = this.itemTextures[advice.get_id()],
					Size = new Point(40, 40),
					Location = new Point(0, 0),
					Parent = this.panel
				};
				container.BasicTooltipText = Magic.get_local_name(advice.get_id()) + "\n"+advice.ToString();
				container.Show();
			}
		}

		
		

		protected override void Build(Container buildPanel)
		{
			this.panel.Parent = buildPanel;
			build_inventory_panel();
		}
	}


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
						if(Magic.jsonLut.itemLut.ContainsKey(item)==true)
						{
							this.itemTextures.Add(item, AsyncTexture2D.FromAssetId(Magic.jsonLut.itemLut[item].IconId));
						}
						else
						{
							this.itemTextures.Add(item, AsyncTexture2D.FromAssetId(Magic.unknown.IconId));
						}
						
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
				if (this.itemTextures.ContainsKey(item.get_id()) == false)
				{
					this.itemTextures.Add(item.get_id(), AsyncTexture2D.FromAssetId(item.get_iconId()));
				}

				if (string.IsNullOrEmpty(this.hunt)||Magic.get_local_name(item.get_id()).ToLower().Contains(this.hunt.ToLower()))
				{
					if(this.excludedItemIds.Contains(item.get_id())==false)
					{
						var container = GetStandardPanel(rootPanel, Magic.get_local_name(item.get_id()), item.get_id());
						container.BasicTooltipText = item.ToString();
						if (this.ignoredItemsFlag == true)
						{
							container.Click += (s, e) => this.callback(item.get_id(), true);
						}
						
						
						container.Show();
					}
					
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
