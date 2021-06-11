using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using CoOpSpRpG;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WaywardExtensions;

namespace WTFModLoader
{
    public static class HarmonyPatcher
    {
        public static Harmony harmony = new Harmony("WTFModLoader");

		//modifying WTF backdrop loading process with harmony for testing purposes
		/*
		public static void PatchBACKDROP()
        {
			var mOriginal = AccessTools.Method(typeof(BACKDROP), "Load", new Type[] { typeof(GraphicsDevice), typeof(IServiceProvider) });
            var mPrefix = AccessTools.Method(typeof(HarmonyPatcher), "BACKDROPLoadPrefix");
            harmony.Patch(mOriginal, new HarmonyMethod(mPrefix));
        }
		*/
		public static void PatchGameRootMenu()
        {
			PatchRootMenuRev2();
			PatchWidgetSettings();
		}

		public static void PatchRootMenuRev2()
		{
			var mOriginal = (MethodBase)(typeof(RootMenuRev2).GetMember(".ctor", AccessTools.all)[0]);
			var mPostfix = AccessTools.Method(typeof(HarmonyPatcher), "RootMenuRev2ConstructorPostfix");
			harmony.Patch(mOriginal, null, new HarmonyMethod(mPostfix));

			mOriginal = AccessTools.Method(typeof(RootMenuRev2), "createElements", new Type[] { });
			mPostfix = AccessTools.Method(typeof(HarmonyPatcher), "RootMenuRev2createElementsPostfix");
			harmony.Patch(mOriginal, null, new HarmonyMethod(mPostfix));

			mOriginal = AccessTools.Method(typeof(RootMenuRev2), "resize", new Type[] { });
			mPostfix = AccessTools.Method(typeof(HarmonyPatcher), "RootMenuRev2resizePostfix");
			harmony.Patch(mOriginal, null, new HarmonyMethod(mPostfix));

			mOriginal = AccessTools.Method(typeof(RootMenuRev2), "Update", new Type[] { typeof(float) });
			var mPrefix = AccessTools.Method(typeof(HarmonyPatcher), "RootMenuRev2UpdatePrefix");
			harmony.Patch(mOriginal, new HarmonyMethod(mPrefix));
		}

		public static void PatchWidgetSettings()
		{
			var internalclass = typeof(RootMenuRev2).Assembly.GetType("CoOpSpRpG.WidgetSettings");
			var mOriginal = AccessTools.Method(internalclass, "Draw", new Type[] { typeof(SpriteBatch) });
			var mPostfix = AccessTools.Method(typeof(HarmonyPatcher), "WidgetSettingsDrawPostfix");
			harmony.Patch(mOriginal, null, new HarmonyMethod(mPostfix));
		}

		
		//modifying WTF backdrop loading process with harmony for testing purposes
		/*
		private static bool BACKDROPLoadPrefix(GraphicsDevice device, IServiceProvider services, ref ContentManager ___content, ref RenderTarget2D ___backdropTarget, ref RenderTarget2D ___occlusionTarget)
		{
			___content = new ContentManager(services, "Content");
			BACKDROP.defaultLight = default(LightSettings);
			BACKDROP.defaultLight.ambLightColor = new Color(0.2f, 0.2f, 0.2f, 1f);
			BACKDROP.defaultLight.lightIntensity = 1.3f;
			BACKDROP.defaultLight.lightColor = new Color(0.67f, 0.88f, 1f, 1f);
			BACKDROP.defaultLight.fogColor = new Color(0, 0, 0, 0);
			BACKDROP.defaultLight.bloomSettings = new BloomSettings(0.8f, 31f, 2.4f, 1f, 0.9f, 1f, 0.81f);
			BACKDROP.defaultLightShafts = new LightShaftSettings(0.11f, 1.05f, 0.9f, 0.998f, 400f);
			BACKDROP.stationSpawnSectors.Add(new Color(180, 180, 180));
			BACKDROP.stationSpawnSectors.Add(new Color(90, 90, 90));
			BACKDROP.stationSpawnSectors.Add(new Color(255, 200, 70));
			BACKDROP.stationSpawnSectors.Add(new Color(128, 100, 35));

			//BACKDROP.makeTarget(device);
			if (___backdropTarget != null)
			{
				___backdropTarget.Dispose();
			}
			if (___occlusionTarget != null)
			{
				___occlusionTarget.Dispose();
			}
			PresentationParameters presentationParameters = device.PresentationParameters;
			SurfaceFormat preferredFormat = SurfaceFormat.HalfVector4;
			___backdropTarget = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, preferredFormat, DepthFormat.Depth24, presentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
			___occlusionTarget = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Alpha8, DepthFormat.None, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);


			Dictionary<Color, BackdropExt> dictionary = new Dictionary<Color, BackdropExt>();
			Dictionary<Color, TerrainGenerator> dictionary2 = new Dictionary<Color, TerrainGenerator>();
			Dictionary<Color, LightSettings> dictionary3 = new Dictionary<Color, LightSettings>();
			Dictionary<Color, LightShaftSettings> dictionary4 = new Dictionary<Color, LightShaftSettings>();
			Dictionary<Color, string[]> dictionary5 = new Dictionary<Color, string[]>();
			Dictionary<Color, IconBatch> dictionary6 = new Dictionary<Color, IconBatch>();
			Dictionary<Color, string> dictionary7 = new Dictionary<Color, string>();
			if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Extensions\\"))
			{
				string[] libraries = DATA_FOLDER.getLibraries();
				List<Assembly> list = new List<Assembly>(libraries.Length);
				foreach (string text in libraries)
				{
					if (!text.Contains("WTFModLoader.dll") && !text.Contains("0Harmony.dll") && !text.Contains("Newtonsoft.Json.dll") && !text.Contains("SimpleInjector.dll"))
					{
						try
						{
							Assembly item = Assembly.UnsafeLoadFrom(text);
							list.Add(item);
						}
						catch
						{
							SCREEN_MANAGER.debug1 = "failed to load an extension:" + text;
						}
					}
				}
				Type typeFromHandle = typeof(ExtensionLoader);
				List<Type> list2 = new List<Type>();
				foreach (Assembly assembly in list)
				{
					if (assembly != null)
					{
						try
						{
							foreach (Type type in assembly.GetTypes())
							{
								if (!type.IsInterface && !type.IsAbstract && type.GetInterface(typeFromHandle.FullName) != null)
								{
									list2.Add(type);
								}
							}
						}
						catch
						{
							SCREEN_MANAGER.debug1 += "Skipped loading extension with invalid format/n";
						}
					}
				}
				List<ExtensionLoader> list3 = new List<ExtensionLoader>(list2.Count);
				foreach (Type type2 in list2)
				{
					ExtensionLoader item2 = (ExtensionLoader)Activator.CreateInstance(type2);
					list3.Add(item2);
				}
				BACKDROP.preloadRequired = new List<Color>();
				BACKDROP.sectorArt = new Dictionary<string, TextureBatch>();
				foreach (ExtensionLoader extensionLoader in list3)
				{
					if (CONFIG.debugExtensions)
					{
						extensionLoader.load(dictionary, BACKDROP.sectorArt, dictionary2, dictionary3, dictionary4, dictionary5, BACKDROP.preloadRequired, dictionary6, dictionary7, BACKDROP.mapDataFiles, BACKDROP.interestIcons, BACKDROP.mapInfo);
					}
					else
					{
						try
						{
							extensionLoader.load(dictionary, BACKDROP.sectorArt, dictionary2, dictionary3, dictionary4, dictionary5, BACKDROP.preloadRequired, dictionary6, dictionary7, BACKDROP.mapDataFiles, BACKDROP.interestIcons, BACKDROP.mapInfo);
						}
						catch (Exception ex)
						{
							SCREEN_MANAGER.debug1 = SCREEN_MANAGER.debug1 + "\nFailed to load an extension:\n" + ex.Message;
						}
					}
				}
				//BACKDROP.getBackdopData();
				BACKDROP.mapData = new Dictionary<Rectangle, string>();
				if (BACKDROP.mapDataFiles != null && BACKDROP.mapDataFiles.Count > 0)
				{
					string[] array = BACKDROP.mapDataFiles.ToArray();
					for (int i = 0; i < array.Length; i++)
					{
						try
						{
							if (File.Exists(array[i]))
							{
								string[] array2 = Path.GetFileNameWithoutExtension(array[i]).Split(new char[]
								{
								'_'
								});
								int x = int.Parse(array2[0]);
								int y = int.Parse(array2[1]);
								using (FileStream fileStream = new FileStream(array[i], FileMode.Open, FileAccess.Read))
								{
									BitmapFrame bitmapFrame = new PngBitmapDecoder(fileStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
									int pixelWidth = bitmapFrame.PixelWidth;
									int pixelHeight = bitmapFrame.PixelHeight;
									Rectangle key = new Rectangle(x, y, pixelWidth, pixelHeight);
									BACKDROP.mapData[key] = array[i];
								}
							}
						}
						catch
						{
						}
					}
				}

			}
			foreach (Color color in dictionary.Keys)
			{
				UniversalBackdrop universalBackdrop = new UniversalBackdrop(dictionary[color]);
				universalBackdrop.onFirstLoad(color, device, new ModDataTextureFinder());
				try
				{
					BACKDROP.addBackdrop(color, universalBackdrop);
				}
				catch (Exception ex2)
				{
					SCREEN_MANAGER.debug1 = SCREEN_MANAGER.debug1 + ex2.Message + "\n";
				}
			}
			foreach (Color color2 in dictionary2.Keys)
			{
				TerrainGenerator mod = dictionary2[color2];
				try
				{
					BACKDROP.addGenerator(color2, mod);
				}
				catch (Exception ex3)
				{
					SCREEN_MANAGER.debug1 = SCREEN_MANAGER.debug1 + ex3.Message + "\n";
				}
			}
			foreach (Color color3 in dictionary3.Keys)
			{
				LightSettings mod2 = dictionary3[color3];
				try
				{
					BACKDROP.addLightSettings(color3, mod2);
				}
				catch (Exception ex4)
				{
					SCREEN_MANAGER.debug1 = SCREEN_MANAGER.debug1 + ex4.Message + "\n";
				}
			}
			foreach (Color color4 in dictionary4.Keys)
			{
				LightShaftSettings mod3 = dictionary4[color4];
				try
				{
					BACKDROP.addLightShaftSettings(color4, mod3);
				}
				catch (Exception ex5)
				{
					SCREEN_MANAGER.debug1 = SCREEN_MANAGER.debug1 + ex5.Message + "\n";
				}
			}
			foreach (Color color5 in dictionary5.Keys)
			{
				string[] mod4 = dictionary5[color5];
				try
				{
					BACKDROP.addAudioSettings(color5, mod4);
				}
				catch (Exception ex6)
				{
					SCREEN_MANAGER.debug1 = SCREEN_MANAGER.debug1 + ex6.Message + "\n";
				}
			}
			foreach (Color color6 in dictionary6.Keys)
			{
				IconBatch mod5 = dictionary6[color6];
				try
				{
					BACKDROP.addIcon(color6, mod5);
				}
				catch (Exception ex7)
				{
					SCREEN_MANAGER.debug1 = SCREEN_MANAGER.debug1 + ex7.Message + "\n";
				}
			}
			foreach (Color color7 in dictionary7.Keys)
			{
				string mod6 = dictionary7[color7];
				try
				{
					BACKDROP.addIconTechnique(color7, mod6);
				}
				catch (Exception ex8)
				{
					SCREEN_MANAGER.debug1 = SCREEN_MANAGER.debug1 + ex8.Message + "\n";
				}
			}
			foreach (string key in BACKDROP.sectorArt.Keys)
			{
				BACKDROP.sectorArt[key].onFirstLoad(new ModDataTextureFinder());
			}
			return false;
		}
		*/

		private static void RootMenuRev2ConstructorPostfix(RootMenuRev2 __instance, ref int ___coreMenuWidth)
		{
			WTFRootMenu.instance = __instance;
			WTFRootMenu.mods = new WidgetMods(new WidgetMods.CloseEvent(WTFRootMenu.closeSettings));
			___coreMenuWidth += 124;
		}

		private static void RootMenuRev2createElementsPostfix(GuiElement ___subMenuExtra)
		{
			___subMenuExtra.width += 124;
			___subMenuExtra.elementList[0].offsetX += 4;
			___subMenuExtra.AddButton("Mod Manager", SCREEN_MANAGER.white, 0, 0, 120, 40, new BasicButton.ClickEvent(WTFRootMenu.actionMods), SCREEN_MANAGER.FF16, new Color(196, 250, 255, 210));
			___subMenuExtra.elementList.Move(___subMenuExtra.elementList.Count-1, 0);
		}

		private static void RootMenuRev2resizePostfix()
		{
			WTFRootMenu.mods.Resize();
		}
		private static void RootMenuRev2UpdatePrefix(float elapsed, Rectangle ___mousePos, MouseState ___lastMouse)
		{
			MouseAction mouseAction = MouseAction.none;
			var currentMouse = Mouse.GetState();
			if (currentMouse.LeftButton == ButtonState.Released)
			{
				mouseAction = MouseAction.leftRelease;
			}
			if (currentMouse.LeftButton == ButtonState.Released && ___lastMouse.LeftButton == ButtonState.Pressed)
			{
				mouseAction = MouseAction.leftClick;
			}
			if (currentMouse.LeftButton == ButtonState.Pressed && ___lastMouse.LeftButton == ButtonState.Pressed)
			{
				mouseAction = MouseAction.leftDrag;
			}
			if (currentMouse.RightButton == ButtonState.Pressed && ___lastMouse.RightButton == ButtonState.Pressed)
			{
				mouseAction = MouseAction.rightDrag;
			}
			if (currentMouse.RightButton == ButtonState.Released && ___lastMouse.RightButton == ButtonState.Pressed)
			{
				mouseAction = MouseAction.rightClick;
			}
			if (currentMouse.LeftButton == ButtonState.Released && ___lastMouse.LeftButton == ButtonState.Pressed && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
			{
				mouseAction = MouseAction.shiftLeftClick;
			}
			WTFRootMenu.mods.Update(elapsed, ___mousePos, mouseAction);
		}
		private static void WidgetSettingsDrawPostfix(SpriteBatch batch)
		{
			WTFRootMenu.mods.Draw(batch);
		}
	}
}
