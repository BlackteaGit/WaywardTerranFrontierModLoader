
using CoOpSpRpG;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WaywardExtensions;
using WTFModLoader.Config;
using WTFModLoader.Infrastructure;
using WTFModLoader.Manager;

namespace WTFModLoader
{
	public static class WTFModLoader
	{
		public static ModManager _modManager;
		public static string CurrentBuildVersion { get; private set; }
		public static string ModsDirectory { get; private set; }
		public static string SteamModsDirectory { get; private set; }
		public static void Initialize()
		{
			
			if(ModsDirectory == null || SteamModsDirectory == null)
			{
				LegacyLoad();
			}
			else
			{
				string manifestDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				if(_modManager != null)
				{
					return;
                }
				CurrentBuildVersion = "0.4";
				String manifestDirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(manifestDirectory, System.IO.Path.Combine(@"0Harmony.dll")));
				String rootdirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"0Harmony.dll")));
				if (System.IO.File.Exists(manifestDirectoryFile) && System.IO.File.Exists(rootdirectoryFile))
				{
					String rootdirectoryBackupFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"0Harmony.dll.old")));
					File.Copy(rootdirectoryFile, rootdirectoryBackupFile, true);
					File.Delete(rootdirectoryFile);
				}

				manifestDirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(manifestDirectory, System.IO.Path.Combine(@"Newtonsoft.Json.dll")));
				rootdirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"Newtonsoft.Json.dll")));
				if (System.IO.File.Exists(manifestDirectoryFile) && System.IO.File.Exists(rootdirectoryFile))
				{
					String rootdirectoryBackupFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"Newtonsoft.Json.dll.old")));
					File.Copy(rootdirectoryFile, rootdirectoryBackupFile, true);
					File.Delete(rootdirectoryFile);
				}

				manifestDirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(manifestDirectory, System.IO.Path.Combine(@"SimpleInjector.dll")));
				rootdirectoryFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"SimpleInjector.dll")));
				if (System.IO.File.Exists(manifestDirectoryFile) && System.IO.File.Exists(rootdirectoryFile))
				{
					String rootdirectoryBackupFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"SimpleInjector.dll.old")));
					File.Copy(rootdirectoryFile, rootdirectoryBackupFile, true);
					File.Delete(rootdirectoryFile);
				}


				
			}
			EnsureFolderSetup();
			ModDbManager.Init();
			ModDbManager.updateCfgDb();
			ModDbManager.loadCfgData();
			HarmonyPatcher.PatchGameRootMenu();
			Logger.InitializeLogging(Path.Combine(ModsDirectory, "WTFModLoader.log"));		
			SimpleInjector.Container container = CompositionRoot.GetContainer();
			container.Options.ResolveUnregisteredConcreteTypes = true;
			_modManager = new ModManager(ModsDirectory, SteamModsDirectory, new JsonConfigProvider(), new FileSystemModLoader(), container);
			_modManager.Initialize();
		}

		public class MyLoader : ExtensionLoader
		{
			public void load(Dictionary<Color, BackdropExt> backdrops, Dictionary<string, TextureBatch> sectorTextures, Dictionary<Color, TerrainGenerator> sectorGenerators, Dictionary<Color, LightSettings> lightSettings, Dictionary<Color, LightShaftSettings> lightShaftSettings, Dictionary<Color, string[]> audioSettings, List<Color> preloadRequired, Dictionary<Color, IconBatch> mapIcons, Dictionary<Color, string> iconTechniques, List<string> mapDataIncludes, Dictionary<string, POEIcon> interestIcons, Dictionary<Color, BackdropInfo> backdropInfo)
			{
				string rootDirectory = System.IO.Directory.GetCurrentDirectory()
					?? throw new InvalidOperationException("Could not determine operating directory. Is your folder structure correct? " +
					"Try verifying game files in Steam, if you're using it.");
				var workshop = Path.GetFullPath(Path.Combine(rootDirectory, Path.Combine(@"..\..\workshop\content\392080")));
				var mods = Path.GetFullPath(Path.Combine(rootDirectory, Path.Combine(@"Mods")));
				SteamModsDirectory = workshop;
				ModsDirectory = mods;
				Initialize();

			}
		}

		private static void LegacyLoad()
		{
			CurrentBuildVersion = "0.4";
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
			HarmonyPatcher.PatchBACKDROP();
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

