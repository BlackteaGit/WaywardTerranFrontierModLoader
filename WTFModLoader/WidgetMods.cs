using CoOpSpRpG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			this.createElemenets();
		}

		public void Resize()
		{
			this.screenWidth = SCREEN_MANAGER.Device.Viewport.Width;
			this.screenHeight = SCREEN_MANAGER.Device.Viewport.Height;
			if (this.popupListingsRoot.isVisible)
			{
				this.popupListingsRoot.reposition(this.screenWidth / 2 - (int)this.popupSettingSize.X / 2, this.screenHeight / 2 - (int)this.popupSettingSize.Y / 2, false);
				return;
			}
			this.popupListingsRoot.reposition(this.screenWidth / 2 - (int)this.popupSettingSize.X / 2 - this.popupListingsRoot.width, this.screenHeight / 2 - (int)this.popupSettingSize.Y / 2, false);
		}

		private void createElemenets()
		{
			Color color = new Color(196, 250, 255, 210);
			Color baseColor = new Color(0, 0, 0, 198);
			int bheight = 360;
			this.popupCanvas.Add(new Canvas("SettingsUnderlay", SCREEN_MANAGER.white, this.screenWidth / 2 - (int)this.popupSettingSize.X / 2, this.screenHeight / 2 - (int)this.popupSettingSize.Y / 2 + 400, 0, 0, (int)this.popupSettingSize.X, (int)this.popupSettingSize.Y, SortType.vertical, baseColor));
			this.popupListingsRoot = this.popupCanvas.Last<GuiElement>();
			this.popupListingsRoot.addLabel("Mods", SCREEN_MANAGER.FF20, 32, 0, 220, 40, color);

			GuiElement enabledCanvas = this.popupListingsRoot.AddCanvas("Enabled", SCREEN_MANAGER.white, 0, 2, (int)this.popupSettingSize.X, bheight, SortType.vertical);
			enabledCanvas.addLabel("Enabled Mods", SCREEN_MANAGER.FF16, 32, 0, 220, 40, color);
			int bheight2 = 32;
			int bwidth = 128;


			GuiElement guiElement2;// = enabledCanvas.AddSelectorCanvas("Screen Mode options", SCREEN_MANAGER.white, 0, 4, (int)this.popupSettingSize.X, 40, SortType.horizontal);
			/*
			this.settings_FullScreen = guiElement2.AddCheckBoxAdv("Fullscreen", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_Borderless = guiElement2.AddCheckBoxAdv("Borderless", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_Windowed = guiElement2.AddCheckBoxAdv("Windowed", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			enabledCanvas.AddCanvas("Resolution labels", SCREEN_MANAGER.white, 0, 2, (int)this.popupSettingSize.X, 28, SortType.horizontal);
			enabledCanvas.elementList.Last<GuiElement>().addLabel("Full screen resolution", SCREEN_MANAGER.FF12, 32, 0, 170, 24, color);
			enabledCanvas.elementList.Last<GuiElement>().addLabel("Windowed resolution", SCREEN_MANAGER.FF12, 32, 0, 80, 24, color);
			int num = 32;
			guiElement2 = enabledCanvas.AddCanvas("Resolution options", SCREEN_MANAGER.white, 0, 0, (int)this.popupSettingSize.X, num + 6, SortType.horizontal);
			guiElement2.addLabel("x:", SCREEN_MANAGER.FF12, 8, 0, 10, num, color);
			this.inputFieldList.Add(guiElement2.AddInputField("FS width", SCREEN_MANAGER.white, 10, 2, 60, num, new InputField.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color, TextInputType.resolution));
			this.settings_FS_X = guiElement2.elementList.Last<GuiElement>();
			guiElement2.addLabel("y:", SCREEN_MANAGER.FF12, 8, 0, 10, num, color);
			this.inputFieldList.Add(guiElement2.AddInputField("FS width", SCREEN_MANAGER.white, 10, 2, 60, num, new InputField.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color, TextInputType.resolution));
			this.settings_FS_Y = guiElement2.elementList.Last<GuiElement>();
			guiElement2.addLabel("x:", SCREEN_MANAGER.FF12, 40, 0, 10, num, color);
			this.inputFieldList.Add(guiElement2.AddInputField("FS width", SCREEN_MANAGER.white, 10, 2, 60, num, new InputField.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color, TextInputType.resolution));
			this.settings_Win_X = guiElement2.elementList.Last<GuiElement>();
			guiElement2.addLabel("y:", SCREEN_MANAGER.FF12, 8, 0, 10, num, color);
			this.inputFieldList.Add(guiElement2.AddInputField("FS width", SCREEN_MANAGER.white, 10, 2, 60, num, new InputField.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color, TextInputType.resolution));
			this.settings_Win_Y = guiElement2.elementList.Last<GuiElement>();
			enabledCanvas.addLabel("Use alt + enter to switch window mode while ingame", SCREEN_MANAGER.FF12, 14, 2, 220, 20, color * 0.75f);
			bheight2 = 24;
			bwidth = 194;
			enabledCanvas.addLabel("Interior light quality", SCREEN_MANAGER.FF16, 32, 0, 220, 28, color);
			guiElement2 = enabledCanvas.AddSelectorCanvas("Interior quality options", SCREEN_MANAGER.white, 0, 2, (int)this.popupSettingSize.X, 32, SortType.horizontal);
			this.settings_Interior_Full = guiElement2.AddCheckBoxAdv("High", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_Interior_Normal = guiElement2.AddCheckBoxAdv("Normal", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			bwidth = 128;
			enabledCanvas.addLabel("Background quality", SCREEN_MANAGER.FF16, 32, 0, 220, 28, color);
			guiElement2 = enabledCanvas.AddSelectorCanvas("Background quality options", SCREEN_MANAGER.white, 0, 2, (int)this.popupSettingSize.X, 32, SortType.horizontal);
			this.settings_BCG_Full = guiElement2.AddCheckBoxAdv("Full", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_BCG_Limited = guiElement2.AddCheckBoxAdv("Limited", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_BCG_Low = guiElement2.AddCheckBoxAdv("Low", SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			


			bwidth = 128;
			enabledCanvas.addLabel("Postprocessing", SCREEN_MANAGER.FF16, 32, 0, 220, 28, color);
			guiElement2 = enabledCanvas.AddCanvas("Postprocessing options", SCREEN_MANAGER.white, 0, 2, (int)this.popupSettingSize.X, 32, SortType.horizontal);
			this.settings_Post_LightShafts = guiElement2.AddCheckBoxAdv("Lightshaft", SCREEN_MANAGER.white, SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_Post_Bloom = guiElement2.AddCheckBoxAdv("HDR Bloom", SCREEN_MANAGER.white, SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_Post_FXAA = guiElement2.AddCheckBoxAdv("FXAA", SCREEN_MANAGER.white, SCREEN_MANAGER.white, 4, 4, bwidth, bheight2, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			guiElement2 = this.popupListingsRoot.AddCanvas("Labels bot", SCREEN_MANAGER.white, 0, 2, (int)this.popupSettingSize.X, 40, SortType.horizontal);
			guiElement2.addLabel("Gameplay", SCREEN_MANAGER.FF20, 32, 0, 160, 40, color);
			guiElement2.addLabel("Audio", SCREEN_MANAGER.FF20, 32, 0, 100, 40, color);

			GuiElement guiElement3 = this.popupListingsRoot.AddCanvas("Bot horizontal", SCREEN_MANAGER.white, 0, 0, (int)this.popupSettingSize.X, 182, SortType.horizontal);
			guiElement2 = guiElement3.AddCanvas("Gameplay", SCREEN_MANAGER.white, 0, 0, 197, 180, SortType.vertical);
			this.settings_showGUI = guiElement2.AddCheckBox("Show GUI", SCREEN_MANAGER.white, SCREEN_MANAGER.MenuArt[269], 0, 2, 197, 28, new CheckBoxR2.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_dragArrow = guiElement2.AddCheckBox("OneHand Travel", SCREEN_MANAGER.white, SCREEN_MANAGER.MenuArt[269], 0, 2, 197, 28, new CheckBoxR2.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_floatText = guiElement2.AddCheckBox("Float Text", SCREEN_MANAGER.white, SCREEN_MANAGER.MenuArt[269], 0, 2, 197, 28, new CheckBoxR2.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_hwCursor = guiElement2.AddCheckBox("HW cursor", SCREEN_MANAGER.white, SCREEN_MANAGER.MenuArt[269], 0, 2, 197, 28, new CheckBoxR2.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_dockAssist = guiElement2.AddCheckBox("Dock Assist", SCREEN_MANAGER.white, SCREEN_MANAGER.MenuArt[269], 0, 2, 197, 28, new CheckBoxR2.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_turnAssist = guiElement2.AddCheckBox("Turn Assist", SCREEN_MANAGER.white, SCREEN_MANAGER.MenuArt[269], 0, 2, 197, 28, new CheckBoxR2.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);

			guiElement2 = guiElement3.AddCanvas("Audi / Min camera", SCREEN_MANAGER.white, 2, 0, 197, 180, SortType.vertical, new Color(0, 0, 0, 0));
			guiElement2 = guiElement2.AddCanvas("Audio", SCREEN_MANAGER.white, 0, 0, 197, 70, SortType.vertical);
			guiElement2.addLabel("Master Volume", SCREEN_MANAGER.FF12, 32, -4, 160, 18, color);
			guiElement2.addSlider("Master Volume", SCREEN_MANAGER.white, SCREEN_MANAGER.white, 4, 4, 190, 22, 0f, 1f, new Slider.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12);
			this.settings_sfxVolume = guiElement2.elementList.Last<GuiElement>();
			guiElement2.addLabel("Music Volume", SCREEN_MANAGER.FF12, 32, -12, 160, 14, color);
			guiElement2.addSlider("Music Volume", SCREEN_MANAGER.white, SCREEN_MANAGER.white, 4, 6, 190, 22, 0f, 1f, new Slider.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12);
			this.settings_musicVolume = guiElement2.elementList.Last<GuiElement>();
			guiElement2 = guiElement3.elementList.Last<GuiElement>().AddCanvas("Min Cam canv", SCREEN_MANAGER.white, 0, 2, 197, 108, SortType.vertical);
			guiElement2.addLabel("Min Camera distance", SCREEN_MANAGER.FF12, 32, -4, 160, 18, color);
			guiElement2.addSlider("Min Camera distance", SCREEN_MANAGER.white, SCREEN_MANAGER.white, 4, 6, 190, 22, 450f, 1500f, new Slider.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, 980f, 950f);
			this.settings_minCameraDist = guiElement2.elementList.Last<GuiElement>();
			guiElement2.addLabel("Gui Scale", SCREEN_MANAGER.FF12, 32, -8, 160, 18, color);
			guiElement2.addSlider("Gui Scale", SCREEN_MANAGER.white, SCREEN_MANAGER.white, 4, 6, 190, 22, 1f, 3f, new Slider.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, 1f);
			this.settings_guiScale = guiElement2.elementList.Last<GuiElement>();
			this.settings_extraZoom = guiElement2.AddCheckBox("Extra zoom", SCREEN_MANAGER.white, SCREEN_MANAGER.MenuArt[269], 0, 2, 197, 28, new CheckBoxR2.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			guiElement2 = this.popupListingsRoot.AddSelectorCanvas("Camera selection", SCREEN_MANAGER.white, 0, 0, (int)this.popupSettingSize.X, 44, SortType.horizontal);
			guiElement2.addLabel("Camera", SCREEN_MANAGER.FF16, 32, -4, 72, 40, color);
			this.settings_cameraUnlocked = guiElement2.AddCheckBoxAdv("Unlocked", SCREEN_MANAGER.white, 4, 2, 142, 32, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			this.settings_cameraCentered = guiElement2.AddCheckBoxAdv("Centered", SCREEN_MANAGER.white, 4, 2, 142, 32, new CheckBoxAdv.ClickEvent(this.placeholderF), SCREEN_MANAGER.FF12, color);
			*/

			guiElement2 = this.popupListingsRoot.AddCanvas("Control", SCREEN_MANAGER.white, 0, 0, (int)this.popupSettingSize.X, 48, SortType.horizontal);
			guiElement2.AddButton("Apply", SCREEN_MANAGER.white, 4, 4, 113, 40, new BasicButton.ClickEvent(this.applySettings), SCREEN_MANAGER.FF20, color);
			guiElement2.AddButton("Apply & Close", SCREEN_MANAGER.white, 4, 4, 158, 40, new BasicButton.ClickEvent(this.actionApplyCloseSettings), SCREEN_MANAGER.FF20, color);
			guiElement2.AddButton("Close", SCREEN_MANAGER.white, 4, 4, 113, 40, new BasicButton.ClickEvent(this.actionCloseSettings), SCREEN_MANAGER.FF20, CONFIG.textColorRed);
			GuiElement guiElement4 = (BasicButton)guiElement2.elementList.Last<GuiElement>();
			Color baseColor2 = new Color(120, 0, 0, 120);
			guiElement4.baseColor = baseColor2;
			this.popupCanvas.Last<GuiElement>().setVisibilityFadeSelf(false);
		}

		public void OpenSettings()
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
		}

		public void loadSettings()
		{

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

		public WidgetMods.CloseEvent closeEvent;

		private List<GuiElement> popupCanvas;

		private GuiElement menuUnder;

		private Rectangle mousePos;

		public bool active;

		private Vector2 popupAsize = new Vector2(220f, 82f);

		private Vector2 popupSettingSize = new Vector2(400f, 720f);

		private List<GuiElement> inputFieldList;

		private GuiElement popupListingsRoot;

		private GuiElement settings_FullScreen;

		private GuiElement settings_Windowed;

		private GuiElement settings_Borderless;

		private GuiElement settings_FS_X;

		private GuiElement settings_FS_Y;

		private GuiElement settings_Win_X;

		private GuiElement settings_Win_Y;

		private GuiElement settings_BCG_Full;

		private GuiElement settings_BCG_Limited;

		private GuiElement settings_BCG_Low;

		private GuiElement settings_Post_LightShafts;

		private GuiElement settings_Post_Bloom;

		private GuiElement settings_Post_FXAA;

		private GuiElement settings_Autosave;

		private GuiElement settings_showGUI;

		private GuiElement settings_dragArrow;

		private GuiElement settings_floatText;

		private GuiElement settings_hwCursor;

		private GuiElement settings_sfxVolume;

		private GuiElement settings_musicVolume;

		private GuiElement settings_cameraUnlocked;

		private GuiElement settings_cameraCentered;

		private GuiElement settings_dockAssist;

		private GuiElement settings_turnAssist;

		private int screenHeight;

		private int screenWidth;

		private GuiElement settings_Interior_Full;

		private GuiElement settings_Interior_Normal;

		private GuiElement settings_minCameraDist;

		private GuiElement settings_guiScale;

		private GuiElement settings_extraZoom;

		public delegate void CloseEvent(GuiElement sender);
	}
}

