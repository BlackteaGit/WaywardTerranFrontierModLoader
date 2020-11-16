using WTFModLoader.Config;
using WTFModLoader.Exceptions;
using WTFModLoader.Mods;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WTFModLoader.Manager
{
	public class ModManager
	{
		private Container _container;
		private IFileConfigProvider _metadataProvider;
		private FileSystemModLoader _modLoader;

		public string ModsDirectory { get; }
		public string SteamModsDirectory { get; }
		public List<ModEntry> Mods { get; private set; }

		public ModManager(string modsDirectory, string steamModsDirectory, IFileConfigProvider metadataProvider, FileSystemModLoader modLoader, Container container)
		{
			SteamModsDirectory = steamModsDirectory;
			ModsDirectory = modsDirectory;
			_metadataProvider = metadataProvider;
			_modLoader = modLoader;
			_container = container;
		} 

		public void Initialize()
		{
			List<Type> mods = _modLoader.LoadModTypesFromDirectory(ModsDirectory, SteamModsDirectory);
			List<ModEntry> modsWithMetadata = LoadMetadataForModTypes(mods);
			List<ModEntry> modsWithResolvedDependencies = ResolveDependencies(modsWithMetadata);
			List<ModEntry> modsWithResolvedConflicts = ResolveConflicts(modsWithMetadata);
			Mods = InstantiateMods(modsWithResolvedDependencies.Intersect(modsWithResolvedConflicts).ToList());
			AddDefaultModEntries(Mods);
			InitializeMods(Mods);
		}

		private void AddDefaultModEntries(List<ModEntry> mods)
		{
			//mods.Add(new ModEntry(new ModsLoadedInfo(), null, new ModMetadata("Mods Loaded Info", "1.0", null)));  //Loading asseembly included mod for dispalying WTFModloader debug info into the game menu.
		}

		private List<ModEntry> LoadMetadataForModTypes(List<Type> mods)
		{
			var result = new List<ModEntry>();
			foreach (Type mod in mods)
			{
				try
				{
					string metadataPath = Path.Combine(
						Path.GetDirectoryName(mod.Assembly.Location), 
						$"{Path.GetFileNameWithoutExtension(mod.Assembly.Location)}.json");
					ModMetadata metadata = _metadataProvider.Read<ModMetadata>(metadataPath);
					if (metadata is null)
					{
						metadata = new ModMetadata(mod.FullName , "0.0");
					}
					result.Add(new ModEntry(mod, metadata));
				}
				catch (FileNotFoundException)
				{
					ModMetadata metadata = new ModMetadata(mod.FullName, "0.0");
					result.Add(new ModEntry(mod, metadata));
				}
			}
			return result;
		}

		private List<ModEntry> ResolveDependencies(List<ModEntry> modsWithMetadata)
		{
			List<ModEntry> successfullyResolved = new List<ModEntry>();
			var dependencyResolutionList = modsWithMetadata.OrderBy(entry => entry.ModMetadata?.Dependencies?.Count);
			foreach (ModEntry entry in dependencyResolutionList)
			{
				if (entry.ModMetadata.Dependencies is null || entry.ModMetadata.Dependencies.Count == 0)
				{
					successfullyResolved.Add(entry);
					continue;
				}
				List<ModMetadata> currentlyResolvedMods= successfullyResolved.Select(x => x.ModMetadata).ToList();
				currentlyResolvedMods.Add(entry.ModMetadata);
				bool resolved = entry.ModMetadata.TryResolveDependencies(currentlyResolvedMods);
				if (resolved)
				{
					successfullyResolved.Add(entry);
				}
				else
				{
					Logger.Log($"Mod `{entry.ModMetadata.Name} (v{entry.ModMetadata.Version})` failed to resolve dependencies. (required mod(s) definition not found in metadata files)");
				}
			}
			return successfullyResolved;
		}

		private List<ModEntry> ResolveConflicts(List<ModEntry> modsWithMetadata)
		{
			List<ModEntry> successfullyResolved = new List<ModEntry>();
			var conflictResolutionList = modsWithMetadata.OrderBy(entry => entry.ModMetadata?.Conflicts?.Count);
			foreach (ModEntry entry in conflictResolutionList)
			{
				if (entry.ModMetadata.Conflicts is null || entry.ModMetadata.Conflicts.Count == 0)
				{
					successfullyResolved.Add(entry);
					continue;
				}
				List<ModMetadata> currentlyResolvedMods = successfullyResolved.Select(x => x.ModMetadata).ToList();
				currentlyResolvedMods.Add(entry.ModMetadata);
				bool resolved = entry.ModMetadata.TryResolveConflicts(currentlyResolvedMods);
				if (resolved)
				{
					successfullyResolved.Add(entry);
				}
				else
				{
					Logger.Log($"Mod `{entry.ModMetadata.Name} (v{entry.ModMetadata.Version})` failed to resolve conflicts. (conflicting mod(s) definition was found in metadata files)");
				}
			}
			return successfullyResolved;
		}

		private List<ModEntry> InstantiateMods(List<ModEntry> modEntries)
		{
			var instantiatedEntries = new List<ModEntry>();
			foreach (ModEntry entry in modEntries)
			{
				try
				{
					IWTFMod modInstance = _container.GetInstance(entry.ModType) as IWTFMod;
					if (modInstance == null)
					{
						throw new ModLoadFailureException($"Mod `{entry.ModType.FullName}` failed to initialize for unknown reason.");
					}

					instantiatedEntries.Add(new ModEntry(modInstance, entry.ModType, entry.ModMetadata));		
				}
				catch (Exception e)
				{
					Logger.Log($"Mod `{entry.ModMetadata.Name} (v{entry.ModMetadata.Version})` failed to initialize.");
					Logger.Log($"{e}");
					continue;
				}
			}
			return instantiatedEntries;
		}


		private void InitializeMods(List<ModEntry> mods)
		{
			ModLoadPriority[] priorityOrder = new[] { ModLoadPriority.High, ModLoadPriority.Normal, ModLoadPriority.Low };
			foreach (ModLoadPriority priority in priorityOrder)
			{
				Logger.Log($"Loading `{priority}` priority mods.");
				IEnumerable<ModEntry> prioritizedMods = mods.Where(mod => mod.ModInstance.Priority == priority);
				foreach (ModEntry mod in prioritizedMods)
				{
					mod.ModInstance.Initialize();
					Logger.Log($"Successfully initialized mod `{mod.ModMetadata.Name} (v{mod.ModMetadata.Version})`.");
				}
			}
		}
	}
}
