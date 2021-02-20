using System;
using System.IO;
using System.Reflection;
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
			EnsureFolderSetup();
			Logger.InitializeLogging(Path.Combine(ModsDirectory, "WTFModLoader.log"));		
			SimpleInjector.Container container = CompositionRoot.GetContainer();
			_modManager = new ModManager(ModsDirectory, SteamModsDirectory, new JsonConfigProvider(), new FileSystemModLoader(), container);
			_modManager.Initialize();
		}

		private static void EnsureFolderSetup()
		{
			string manifestDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
				?? throw new InvalidOperationException("Could not determine operating directory. Is your folder structure correct? " +
				"Try verifying game files in the Epic Games Launcher, if you're using it.");

			SteamModsDirectory = Path.GetFullPath(Path.Combine(manifestDirectory, Path.Combine(@"..\..\workshop\content\392080")));
			ModsDirectory = Path.GetFullPath(Path.Combine(manifestDirectory, Path.Combine(@"Mods")));

			if (!Directory.Exists(ModsDirectory))
			{
				Directory.CreateDirectory(ModsDirectory);
			}
		}
	}
}

