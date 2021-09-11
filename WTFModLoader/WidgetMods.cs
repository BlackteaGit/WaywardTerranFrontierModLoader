using CoOpSpRpG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WaywardExtensions;
using WTFModLoader.Manager;

namespace WTFModLoader
{
	public class WidgetMods
	{
		public WidgetMods(WidgetMods.CloseEvent closeMethod)
		{
			this.closeEvent = closeMethod;
			this.screenWidth = SCREEN_MANAGER.Device.Viewport.Width;
			this.screenHeight = SCREEN_MANAGER.Device.Viewport.Height;
			this.inputFieldList = new List<GuiElement>();
			this.popupCanvas = new List<GuiElement>();
			this.createElements();
		}

		public void Resize()
		{
			this.screenWidth = SCREEN_MANAGER.Device.Viewport.Width;
			this.screenHeight = SCREEN_MANAGER.Device.Viewport.Height;
			if (this.popupListingsRoot.isVisible)
			{
				this.popupListingsRoot.reposition(this.screenWidth / 2 - (int)this.popupModsSize.X / 2, this.screenHeight / 2 - (int)this.popupModsSize.Y / 2, false);
				return;
			}
			this.popupListingsRoot.reposition(this.screenWidth / 2 - (int)this.popupModsSize.X / 2 - this.popupListingsRoot.width, this.screenHeight / 2 - (int)this.popupModsSize.Y / 2, false);
		}

		private void createElements()
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			Color color = new Color(196, 250, 255, 210);
			Color baseColor = new Color(0, 0, 0, 198);
			int bheight = (int)popupModsSize.Y-40-48;//360;
			this.popupCanvas.Add(new Canvas("SettingsUnderlay", SCREEN_MANAGER.white, this.screenWidth / 2 - (int)this.popupModsSize.X / 2, this.screenHeight / 2 - (int)this.popupModsSize.Y / 2 + 400, 0, 0, (int)this.popupModsSize.X, (int)this.popupModsSize.Y, SortType.vertical, baseColor));
			this.popupListingsRoot = this.popupCanvas.Last<GuiElement>();
			popupListingsRoot.AddSelectorCanvas("Sort Labels", SCREEN_MANAGER.white, 0, 0, (int)this.popupModsSize.X, 40, SortType.horizontal);
			typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("renderBrackets", flags).SetValue(this.popupListingsRoot.elementList.Last<GuiElement>(), false);
			this.popupListingsRoot.elementList.Last<GuiElement>().addLabel("Available Mods", SCREEN_MANAGER.FF20, 8, 0, (int)this.popupModsSize.X / 2, 40, CONFIG.textColorDark);
			this.popupListingsRoot.elementList.Last<GuiElement>().addLabel("Mod Settings", SCREEN_MANAGER.FF20, 0, 0, (int)this.popupModsSize.X / 2 - 8, 40, CONFIG.textColorDark);
			popupListingsRoot.AddSelectorCanvas("Sort Mod Data", SCREEN_MANAGER.white, 0, 4, (int)this.popupModsSize.X, bheight, SortType.horizontal);
			typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("renderBrackets", flags).SetValue(popupListingsRoot.elementList.Last<GuiElement>(), false);
			this.scrollModsCanvas = (ScrollCanvas)popupListingsRoot.elementList.Last<GuiElement>().AddScrollCanvas("enabled scroll", SCREEN_MANAGER.white, 0, 2, (int)this.popupModsSize.X / 2, bheight - 6, SortType.vertical);
			popupListingsRoot.elementList.Last<GuiElement>().AddSelectorCanvas("Sort Mod Settings", SCREEN_MANAGER.white, 0, 4, (int)this.popupModsSize.X / 2, bheight - 8, SortType.vertical);
			var sortSettingsCanvas = popupListingsRoot.elementList.Last<GuiElement>().elementList.Last<GuiElement>();
			typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("renderBrackets", flags).SetValue(sortSettingsCanvas, false);
			sortSettingsCanvas.addLabel("", SCREEN_MANAGER.FF20, 8, 0, (int)this.popupModsSize.X / 2 - 16, 40, color);
			this.modTitleLabel = (Label)sortSettingsCanvas.elementList.Last<GuiElement>();
			sortSettingsCanvas.AddSelectorCanvas("Sort Data Labels", SCREEN_MANAGER.transparentBlack, 0, 0, (int)this.popupModsSize.X / 2, 80, SortType.vertical);
			var sortDataLabels = sortSettingsCanvas.elementList.Last<GuiElement>();
			typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("renderBrackets", flags).SetValue(sortDataLabels, false);
			sortDataLabels.addLabel("Author:", SCREEN_MANAGER.FF12, 8, 0, sortDataLabels.width -8, 20, color);
			this.modAuthorLabel = (Label)sortDataLabels.elementList.Last<GuiElement>();
			sortDataLabels.addLabel("Version:", SCREEN_MANAGER.FF12, 8, 0, sortDataLabels.width -8, 20, color);
			this.modVersionLabel = (Label)sortDataLabels.elementList.Last<GuiElement>();
			sortDataLabels.addLabel("Target game version:", SCREEN_MANAGER.FF12, 8, 0, sortDataLabels.width -8, 20, color);
			this.gameVersionLabel = (Label)sortDataLabels.elementList.Last<GuiElement>();
			sortDataLabels.addLabel("Source:", SCREEN_MANAGER.FF12, 8, 0, sortDataLabels.width - 8, 20, color);
			this.modSourceLabel = (Label)sortDataLabels.elementList.Last<GuiElement>();
			sortDataLabels.baseColor = new Color(0, 0, 0, 0); //making labels background transparent

			sortSettingsCanvas.AddScrollCanvas("description scroll", SCREEN_MANAGER.white, 8, 12, (int)this.popupModsSize.X / 2 - 16 , 426, SortType.vertical);
			this.descriptionTextScroll = sortSettingsCanvas.elementList.Last<GuiElement>();
			

			var bheight2 = 40;
			var bwidth = (int)this.popupModsSize.X / 4-8;
			this.selectModSettingsCanvas = sortSettingsCanvas.AddSelectorCanvas("Mod state select", SCREEN_MANAGER.white, 0, 10, (int)this.popupModsSize.X / 2 - 4, 48, SortType.horizontal);
			this.settings_Mod_Enabled = selectModSettingsCanvas.AddCheckBoxAdv("Enabled", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent((sender) => updateSettings(this.selectedEntry, true)), SCREEN_MANAGER.FF20, Color.LightGreen);
			this.settings_Mod_Disabled = selectModSettingsCanvas.AddCheckBoxAdv("Disabled", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent((sender) => updateSettings(this.selectedEntry, false)), SCREEN_MANAGER.FF20, CONFIG.textColorRed);

			
			var offsetcenter = (int)this.popupModsSize.X / 2 - (113 + 158 + 113) / 2;
			var guiElement2 = this.popupListingsRoot.AddCanvas("Control", SCREEN_MANAGER.white, 0, 0, (int)this.popupModsSize.X, 48, SortType.horizontal);
			guiElement2.AddButton("Apply", SCREEN_MANAGER.white, offsetcenter + 4, 4, 113, 40, new BasicButton.ClickEvent(this.applySettings), SCREEN_MANAGER.FF20, color);
			guiElement2.AddButton("Apply & Close", SCREEN_MANAGER.white, 4, 4, 158, 40, new BasicButton.ClickEvent(this.actionApplyCloseSettings), SCREEN_MANAGER.FF20, color);
			guiElement2.AddButton("Close", SCREEN_MANAGER.white, 4, 4, 113, 40, new BasicButton.ClickEvent(this.actionCloseSettings), SCREEN_MANAGER.FF20, CONFIG.textColorRed);
			GuiElement guiElement4 = (BasicButton)guiElement2.elementList.Last<GuiElement>();
			Color baseColor2 = new Color(120, 0, 0, 120);
			guiElement4.baseColor = baseColor2;
			this.popupCanvas.Last<GuiElement>().setVisibilityFadeSelf(false);
		}

		public void OpenMods()
		{
			this.loadSettings();
			this.active = true;
			this.popupCanvas.Last<GuiElement>().setVisibilityFadeSelf(true);
		}

		public void placeholderF(object sender)
		{
		}

		public void actionCloseSettings(object sender)
		{
			this.popupCanvas.Last<GuiElement>().setVisibilityFadeSelf(false);
			this.active = false;
			this.closeEvent(null);
		}

		public void actionApplyCloseSettings(object sender)
		{
			this.popupCanvas.Last<GuiElement>().setVisibilityFadeSelf(false);
			this.applySettings(null);
			this.active = false;
			this.closeEvent(null);
		}

		public void applySettings(object sender)
		{
			if (this._tempActiveMods.IsValueCreated)
			{
				WTFModLoader._modManager.ActiveMods.Clear();
				WTFModLoader._modManager.ActiveMods.AddRange(this._tempActiveMods.Value);				
			}
			if(this._tempInactiveMods.IsValueCreated)
            {
				WTFModLoader._modManager.InactiveMods.Value.Clear();
				WTFModLoader._modManager.InactiveMods.Value.AddRange(this._tempInactiveMods.Value);
			}
			ModDbManager.writeModCfgData();
			ModDbManager.writeCfgData();
			SCREEN_MANAGER.alerts.Enqueue("Mod settings saved and will be applied on the next game start.");
		}

		public void loadSettings()
		{
			this.scrollModsCanvas.elementList.Clear();

			UpdateModInfo(null);
			WTFModLoader._modManager.ActiveMods.Sort((x, y) => x.ModMetadata.Name.CompareTo(y.ModMetadata.Name));
			foreach (var modentry in WTFModLoader._modManager.ActiveMods)
			{
				string status = "active";				
				this.scrollModsCanvas.AddSaveEntry(shortenEntryTitle(modentry), status, SCREEN_MANAGER.white, 0, 4, (int)this.popupModsSize.X / 2 - 14, 52, SortType.vertical, new SaveEntry.ClickJournalEvent((entry) => UpdateModInfo(modentry)), null);
				//this.scrollModsCanvas.elementList.Last().elementList[0].baseColor = Color.LightGreen;
				this.scrollModsCanvas.elementList.Last().elementList[1].baseColor = Color.LightGreen;
			}

			if (WTFModLoader._modManager.InactiveMods.IsValueCreated)
			{
				WTFModLoader._modManager.InactiveMods.Value.Sort((x, y) => x.ModMetadata.Name.CompareTo(y.ModMetadata.Name));
				foreach (var modentry in WTFModLoader._modManager.InactiveMods.Value)
				{
					string status = "disabled";
					this.scrollModsCanvas.AddSaveEntry(shortenEntryTitle(modentry), status, SCREEN_MANAGER.white, 0, 4, (int)this.popupModsSize.X / 2 - 14, 52, SortType.vertical, new SaveEntry.ClickJournalEvent((entry) => UpdateModInfo(modentry)), null);
					this.scrollModsCanvas.elementList.Last().elementList[1].baseColor = CONFIG.textColorMedium;
				}
			}

			if (WTFModLoader._modManager.IssuedMods.IsValueCreated)
			{
				foreach (var modentry in WTFModLoader._modManager.IssuedMods.Value)
				{
					
					string issue = "unknown error";
					if (modentry.Issue != "")
					{
						issue = "loading error: " + modentry.Issue;
					}
					this.scrollModsCanvas.AddSaveEntry(shortenEntryTitle(modentry), issue, SCREEN_MANAGER.white, 0, 4, (int)this.popupModsSize.X / 2 - 14, 52, SortType.vertical, new SaveEntry.ClickJournalEvent((entry) => UpdateModInfo(modentry)), null);
					//this.scrollModsCanvas.elementList.Last().elementList.ForEach((label) => label.baseColor = CONFIG.textColorRed);
					//this.scrollModsCanvas.elementList.Last().elementList[0].baseColor = CONFIG.textColorRed;
					this.scrollModsCanvas.elementList.Last().elementList[1].baseColor = CONFIG.textColorRed;
				}
			}
			this._tempActiveMods.Value.Clear();
			this._tempActiveMods.Value.AddRange(WTFModLoader._modManager.ActiveMods);
			this._tempActiveMods.Value.Sort((x, y) => x.ModMetadata.Name.CompareTo(y.ModMetadata.Name));
			if (WTFModLoader._modManager.InactiveMods.IsValueCreated)
			{
				this._tempInactiveMods.Value.Clear();
				this._tempInactiveMods.Value.AddRange(WTFModLoader._modManager.InactiveMods.Value);
				this._tempInactiveMods.Value.Sort((x, y) => x.ModMetadata.Name.CompareTo(y.ModMetadata.Name));
			}

		}

		private string shortenEntryTitle(ModEntry modentry)
        {
			var titletext = modentry.ModMetadata.wrappedName;
			while (SCREEN_MANAGER.FF16.MeasureString(titletext).X > (int)this.popupModsSize.X / 2 - 22)
			{
				titletext = titletext.Substring(0, titletext.Length - 4);
				titletext = titletext + "..";
			}
			return titletext;
		}

		public void updateSettings(ModEntry selected, bool enable)
		{
			//move active/inactive mods
			if (selected != null)
			{
				if (!enable && this._tempActiveMods.Value.Contains(selected))
				{
					this._tempActiveMods.Value.Remove(selected);
					this._tempInactiveMods.Value.Add(selected);
					this._tempActiveMods.Value.Sort((x, y) => x.ModMetadata.Name.CompareTo(y.ModMetadata.Name));
					this._tempInactiveMods.Value.Sort((x, y) => x.ModMetadata.Name.CompareTo(y.ModMetadata.Name));
				}
				else if (enable && this._tempInactiveMods.Value.Contains(selected))
				{
					this._tempInactiveMods.Value.Remove(selected);
					this._tempActiveMods.Value.Add(selected);
					this._tempActiveMods.Value.Sort((x, y) => x.ModMetadata.Name.CompareTo(y.ModMetadata.Name));
					this._tempInactiveMods.Value.Sort((x, y) => x.ModMetadata.Name.CompareTo(y.ModMetadata.Name));
				}
				this.scrollModsCanvas.elementList.Clear();
				UpdateModInfo(selected);
				foreach (var modentry in this._tempActiveMods.Value)
				{
					string status = "active";
					this.scrollModsCanvas.AddSaveEntry(shortenEntryTitle(modentry), status, SCREEN_MANAGER.white, 0, 4, (int)this.popupModsSize.X / 2 - 14, 52, SortType.vertical, new SaveEntry.ClickJournalEvent((entry) => UpdateModInfo(modentry)), null);
					//this.scrollModsCanvas.elementList.Last().elementList[0].baseColor = Color.LightGreen;
					this.scrollModsCanvas.elementList.Last().elementList[1].baseColor = Color.LightGreen;
					if (modentry.ModMetadata == selected.ModMetadata)
					{
						this.scrollModsCanvas.elementList.Last().hasFocus = true;
					}
				}
				if (this._tempInactiveMods.IsValueCreated)
				{
					foreach (var modentry in this._tempInactiveMods.Value)
					{
						string status = "disabled";
						this.scrollModsCanvas.AddSaveEntry(shortenEntryTitle(modentry), status, SCREEN_MANAGER.white, 0, 4, (int)this.popupModsSize.X / 2 - 14, 52, SortType.vertical, new SaveEntry.ClickJournalEvent((entry) => UpdateModInfo(modentry)), null);
						this.scrollModsCanvas.elementList.Last().elementList[1].baseColor = CONFIG.textColorMedium;
						if (modentry.ModMetadata == selected.ModMetadata)
						{
							this.scrollModsCanvas.elementList.Last().hasFocus = true;
						}

					}
				}
				if (WTFModLoader._modManager.IssuedMods.IsValueCreated)
				{
					foreach (var modentry in WTFModLoader._modManager.IssuedMods.Value)
					{
						string issue = "unknown error";
						if (modentry.Issue != "")
						{
							issue = "loading error: " + modentry.Issue;
						}
						this.scrollModsCanvas.AddSaveEntry(shortenEntryTitle(modentry), issue, SCREEN_MANAGER.white, 0, 4, (int)this.popupModsSize.X / 2 - 14, 52, SortType.vertical, new SaveEntry.ClickJournalEvent((entry) => UpdateModInfo(modentry)), null);
						//this.scrollModsCanvas.elementList.Last().elementList.ForEach((label) => label.baseColor = CONFIG.textColorRed);
						//this.scrollModsCanvas.elementList.Last().elementList[0].baseColor = CONFIG.textColorRed;
						this.scrollModsCanvas.elementList.Last().elementList[1].baseColor = CONFIG.textColorRed;
						if (modentry.ModMetadata == selected.ModMetadata)
						{
							this.scrollModsCanvas.elementList.Last().hasFocus = true;
						}

					}
				}
			}

		}
		public void InputChar(char character)
		{
			foreach (GuiElement guiElement in this.inputFieldList)
			{
				InputField inputField = (InputField)guiElement;
				if (Game1.instance.IsActive && inputField.hasFocus)
				{
					inputField.InputCharacter(character);
				}
			}
		}

		public void Update(float elapsed, Rectangle mousePos, MouseAction clickState)
		{
			foreach (GuiElement guiElement in this.popupCanvas)
			{
				if (Game1.instance.IsActive)
				{
					guiElement.mouseCheck(mousePos, clickState);
				}
				guiElement.update(elapsed, mousePos, clickState);
			}
		}

		public void Draw(SpriteBatch batch)
		{
			foreach (GuiElement guiElement in this.popupCanvas)
			{
				((Canvas)guiElement).Draw(batch);
			}
		}

		public void UpdateModInfo(ModEntry selected)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			this.selectedEntry = selected;
			if(selected == null)
			{
				this.descriptionTextScroll.elementList.Clear();
				typeof(Canvas).Assembly.GetType("CoOpSpRpG.ScrollCanvas", throwOnError: true).GetField("maxValue", flags).SetValue(this.descriptionTextScroll, 0f);
				typeof(Canvas).Assembly.GetType("CoOpSpRpG.ScrollCanvas", throwOnError: true).GetField("sliderValue", flags).SetValue(this.descriptionTextScroll, 0f);
				modTitleLabel.setText("");
				modAuthorLabel.setText("Author:");
				modSourceLabel.setText("Source:");
				modVersionLabel.setText("Version:");
				gameVersionLabel.setText("Target game version:");
				settings_Mod_Enabled.state = false;
				settings_Mod_Disabled.state = false;
				typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("renderBrackets", flags).SetValue(this.selectModSettingsCanvas, false);
				typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("selectionColor", flags).SetValue(this.selectModSettingsCanvas, Color.Transparent);
				foreach (var element in this.selectModSettingsCanvas.elementList)
                {
					(element as CheckBoxAdv).highAlpha = (element as CheckBoxAdv).lowAlpha;
				}			
			}
			else
            {
				if(selected.Issue != null)
				{
					typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("renderBrackets", flags).SetValue(this.selectModSettingsCanvas, false);
					typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("selectionColor", flags).SetValue(this.selectModSettingsCanvas, Color.Transparent);
					foreach (var element in this.selectModSettingsCanvas.elementList)
					{
						(element as CheckBoxAdv).highAlpha = (element as CheckBoxAdv).lowAlpha;
					}
				}
				else
				{ 
					typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("renderBrackets", flags).SetValue(this.selectModSettingsCanvas, true);
					typeof(Canvas).Assembly.GetType("CoOpSpRpG.SelectorCanvas", throwOnError: true).GetField("selectionColor", flags).SetValue(this.selectModSettingsCanvas, new Color(197, 250, 255, 42));
					foreach (var element in this.selectModSettingsCanvas.elementList)
					{
						(element as CheckBoxAdv).highAlpha = 75;
					}
				}

				UpdateDescriptionText(selected.ModMetadata.Description, SCREEN_MANAGER.FF12);
				var titletext = selected.ModMetadata.wrappedName;
				while(SCREEN_MANAGER.FF20.MeasureString(titletext).X > modTitleLabel.width)
				{
					titletext = titletext.Substring(0, titletext.Length - 4);
					titletext = titletext + "..";
				}
				modTitleLabel.setText(titletext);
				string source = "n/a";
				if (selected.Source.Contains(WTFModLoader.ModsDirectory))
				{
					source = "Local Mod";
				}
				else if (selected.Source.Contains(WTFModLoader.SteamModsDirectory))
				{
					source = "Steam Mod";
				}

				modSourceLabel.setText("Source: " + source);

				if (selected.ModMetadata.Version != "0.0")
				{
					modVersionLabel.setText("Version: " + selected.ModMetadata.Version);
				}
				else
				{
					modVersionLabel.setText("Version: " + "n/a");
				}
				
				if (selected.ModMetadata.Gameversion != "" && selected.ModMetadata.Gameversion != null)
				{
					gameVersionLabel.setText("Target game version: " + selected.ModMetadata.Gameversion);
				}
				else
				{
					gameVersionLabel.setText("Target game version: " + "n/a");
				}

				if (selected.ModMetadata.Author != "" && selected.ModMetadata.Author != null)
				{
					modAuthorLabel.setText("Author: " + selected.ModMetadata.Author);
				}
				else
				{
					modAuthorLabel.setText("Author: " + "n/a");
				}

				if(this._tempActiveMods.Value.Contains(selected))
                {
					settings_Mod_Enabled.state = true;
					settings_Mod_Disabled.state = false;
				}
				else
                {
					settings_Mod_Enabled.state = false;
					settings_Mod_Disabled.state = true;
				}

			}
		}

		public void UpdateDescriptionText(string text, SpriteFont font)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			var wrapedText = ToolBox.WrapDynamicText(font, text, descriptionTextScroll.width, 10f, 1f);
			this.descriptionTextScroll.elementList.Clear();
			for (int i = 0; i < wrapedText.textLines.Count; i++)
			{
				this.descriptionTextScroll.AddTextBox(wrapedText.textLines[i], font, 0, 0, descriptionTextScroll.width, (int)font.MeasureString(wrapedText.textLines[i]).Y + 1, new Color(177, 197, 200), VerticalAlignment.top, 18, 1f, 10f, 1f);
			}
			var headPosition = (int)typeof(Canvas).Assembly.GetType("CoOpSpRpG.ScrollCanvas", throwOnError: true).GetField("headPosition", flags).GetValue(this.descriptionTextScroll);
			typeof(Canvas).Assembly.GetType("CoOpSpRpG.ScrollCanvas", throwOnError: true).GetField("maxValue", flags).SetValue(this.descriptionTextScroll, (float)(headPosition - this.descriptionTextScroll.region.Height));
			typeof(Canvas).Assembly.GetType("CoOpSpRpG.ScrollCanvas", throwOnError: true).GetField("sliderValue", flags).SetValue(this.descriptionTextScroll, 0f);
		}

		private Lazy<List<ModEntry>> _tempInactiveMods = new Lazy<List<ModEntry>>();

		private Lazy<List<ModEntry>> _tempActiveMods = new Lazy<List<ModEntry>>();

		private GuiElement descriptionTextScroll;

		private Label modTitleLabel;

		private Label modAuthorLabel;

		private Label modVersionLabel;

		private Label modSourceLabel;

		private Label gameVersionLabel;

		private ScrollCanvas scrollModsCanvas;

		public WidgetMods.CloseEvent closeEvent;

		private List<GuiElement> popupCanvas;

		public bool active;

		private Vector2 popupModsSize = new Vector2(1200f, 720f);

		private List<GuiElement> inputFieldList;

		private GuiElement popupListingsRoot;

		private int screenHeight;

		private int screenWidth;

        private GuiElement selectModSettingsCanvas;

        private GuiElement settings_Mod_Enabled;

		private GuiElement settings_Mod_Disabled;

        private ModEntry selectedEntry;

        public delegate void CloseEvent(GuiElement sender);
	}
}

