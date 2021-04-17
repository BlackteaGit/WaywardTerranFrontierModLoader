using CoOpSpRpG;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using WaywardExtensions;
using WTFModLoader.Config;
using WTFModLoader.Infrastructure;
using WTFModLoader.Manager;

namespace WTFModLoader
{
	public static class WTFModLoader
	{
		private static ModManager _modManager;

		public static string CurrentBuildVersion { get; private set; }
		public static string ModsDirectory { get; private set; }
		public static string SteamModsDirectory { get; private set; }
		public static void Initialize()
		{
			CurrentBuildVersion = "0.3";
			if(ModsDirectory == null || SteamModsDirectory == null)
			{
				LegacyLoad();
			}
			else
			{
				string manifestDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				
				String manifestDirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(manifestDirectory, System.IO.Path.Combine(@"0Harmony.dll")));
				String rootdirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"0Harmony.dll")));

				if (System.IO.File.Exists(manifestDirectoryFile) && System.IO.File.Exists(rootdirectoryFile))
				{
					String rootdirectoryBackupFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"0Harmony.dll.old")));
					File.Copy(rootdirectoryFile, rootdirectoryBackupFile, true);
					File.Delete(rootdirectoryFile);
				}

			}
			EnsureFolderSetup();
			Logger.InitializeLogging(Path.Combine(ModsDirectory, "WTFModLoader.log"));		
			SimpleInjector.Container container = CompositionRoot.GetContainer();
			_modManager = new ModManager(ModsDirectory, SteamModsDirectory, new JsonConfigProvider(), new FileSystemModLoader(), container);
			_modManager.Initialize();
		}

		public class MyLoader : ExtensionLoader
		{
			public void load(Dictionary<Color, BackdropExt> backdrops, Dictionary<string, TextureBatch> sectorTextures, Dictionary<Color, TerrainGenerator> sectorGenerators, Dictionary<Color, LightSettings> lightSettings, Dictionary<Color, LightShaftSettings> lightShaftSettings, Dictionary<Color, string[]> audioSettings, List<Color> preloadRequired, Dictionary<Color, IconBatch> mapIcons, Dictionary<Color, string> iconTechniques, List<string> mapDataIncludes, Dictionary<string, POEIcon> interestIcons, Dictionary<Color, BackdropInfo> backdropInfo)
			{			
			
			string manifestDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
					?? throw new InvalidOperationException("Could not determine operating directory. Is your folder structure correct? " +
					"Try verifying game files in Steam, if you're using it.");
				SteamModsDirectory = Path.GetFullPath(Path.Combine(manifestDirectory, Path.Combine(@"..\..\..\workshop\content\392080")));
				ModsDirectory = Path.GetFullPath(Path.Combine(manifestDirectory, Path.Combine(@"..\Mods")));
				Initialize();
			}
		}

		[HarmonyPatch(typeof(BACKDROP), "Load", new Type[] { typeof(GraphicsDevice), typeof(IServiceProvider) })] // suppressing loading of the WTFModLoader.dll from Extensions directory if it is already loaded by Injector (legacy load)
		public class RootMenuRev2_resize
		{
			[HarmonyPrefix]
			private static bool Prefix(GraphicsDevice device, IServiceProvider services, ref ContentManager ___content, ref RenderTarget2D ___backdropTarget, ref RenderTarget2D ___occlusionTarget)
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
				string path = Directory.GetCurrentDirectory() + "\\Extensions\\";
				if (Directory.Exists(path))
				{
					string[] files = Directory.GetFiles(path, "*.dll");
					List<Assembly> list = new List<Assembly>(files.Length);
					foreach (string text in files)
					{
						if(!text.Contains("WTFModLoader.dll") && !text.Contains("0Harmony.dll") && !text.Contains("Newtonsoft.Json.dll"))
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
		}
			

		private static void LegacyLoad()
		{
			string manifestDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
					?? throw new InvalidOperationException("Could not determine operating directory. Is your folder structure correct? " +
					"Try verifying game files in Steam, if you're using it.");

			String rootdirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"0Harmony.dll")));
			String rootdirectoryChangedFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"0Harmony.dll.old")));
			if (System.IO.File.Exists(rootdirectoryChangedFile) && !System.IO.File.Exists(rootdirectoryFile))
			{
				File.Copy(rootdirectoryChangedFile, rootdirectoryFile, true);
				File.Delete(rootdirectoryChangedFile);
			}
			SteamModsDirectory = Path.GetFullPath(Path.Combine(manifestDirectory, Path.Combine(@"..\..\workshop\content\392080")));
			ModsDirectory = Path.GetFullPath(Path.Combine(manifestDirectory, Path.Combine(@"Mods")));

			Harmony harmony = new Harmony("WTFModLoader");
			harmony.PatchAll();
		}

		private static void EnsureFolderSetup()
		{
			
			if (!Directory.Exists(ModsDirectory))
			{
				Directory.CreateDirectory(ModsDirectory);
			}
		}
	}
}

