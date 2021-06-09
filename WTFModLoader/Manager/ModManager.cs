using WTFModLoader.Config;
using WTFModLoader.Exceptions;
using WTFModLoader.Mods;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WTFModLoader.Manager
{
	public class ModManager
	{
		private Container _container;
		private IFileConfigProvider _metadataProvider;
		private FileSystemModLoader _modLoader;

		public string ModsDirectory { get; }
		public string SteamModsDirectory { get; }
		public List<ModEntry> ActiveMods { get; private set; }
		public Lazy<List<ModEntry>> IssuedMods { get; private set; }
		public Lazy<List<ModEntry>> InactiveMods { get; private set; }
		public Lazy<List<Tuple<Assembly, string>>> conflictingAssemblies { get; private set; }

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
			IssuedMods = new Lazy<List<ModEntry>>();
			InactiveMods = new Lazy<List<ModEntry>>();
			conflictingAssemblies = new Lazy<List<Tuple<Assembly, string>>>();
			List<Type> mods = _modLoader.LoadModTypesFromDirectory(ModsDirectory, SteamModsDirectory);
			List<ModEntry> modsWithMetadata = LoadMetadataForModTypes(mods);
			SetSource(modsWithMetadata);
			List<ModEntry> modsWithResolvedDependencies = ResolveDependencies(modsWithMetadata);
			List<ModEntry> modsWithResolvedConflicts = ResolveConflicts(modsWithMetadata);
			List<ModEntry> modsWithResolvedGameversion = ResolveGameversion(modsWithMetadata);
			List<ModEntry> modsWithResolvedLoaderversion = ResolveLoaderversion(modsWithMetadata);
			List<ModEntry> enabledMods = ResolveDisabledMods(modsWithResolvedLoaderversion);
			ActiveMods = InstantiateMods(modsWithResolvedDependencies.Intersect(modsWithResolvedConflicts).ToList().Intersect(modsWithResolvedGameversion).ToList().Intersect(modsWithResolvedLoaderversion).ToList().Intersect(enabledMods).ToList());
			AddDefaultModEntries(ActiveMods);
			InitializeMods(ActiveMods);
		}

		private void SetSource(List<ModEntry> mods)
		{
			foreach (var mod in mods)
			{
				mod.Source = mod.ModType.Assembly.Location;
			}
		}

		private void AddDefaultModEntries(List<ModEntry> mods)
		{
			//mods.Add(new ModEntry(new ModsLoadedInfo(), null, new ModMetadata("Mods Loaded Info", "1.0")));  //Loading asseembly included mod for dispalying WTFModloader debug info into the game menu.
		}

		private List<ModEntry> ResolveDisabledMods(List<ModEntry> modsWithMetadata)
		{
			List<ModEntry> Resolved = new List<ModEntry>();
			List<ModEntry> successfullyResolved = new List<ModEntry>();
			var disabledResolutionList = modsWithMetadata;
			foreach (ModEntry entry in disabledResolutionList)
			{

				
				if (ModDbManager.isInactive(entry))
				{
					var loaded = InactiveMods.Value.Find(e => e.ModType.FullName == entry.ModType.FullName);
					if (loaded == null)
					{
						InactiveMods.Value.Add(entry);
					}
					else
					{
						if (conflictingAssemblies.IsValueCreated)
						{
							foreach (var assemblyconflict in conflictingAssemblies.Value)
							{
								if (assemblyconflict.Item1 == loaded.ModType.Assembly)
								{
									if (loaded.ModType.Assembly.Location.Contains(WTFModLoader.ModsDirectory) && assemblyconflict.Item2.Contains(WTFModLoader.SteamModsDirectory))
									{
										var mod = entry.ModType;
										try
										{
											string metadataPath = Path.Combine(
												Path.GetDirectoryName(assemblyconflict.Item2),
												$"{Path.GetFileNameWithoutExtension(assemblyconflict.Item2)}.json");
											ModMetadata metadata = _metadataProvider.Read<ModMetadata>(metadataPath);
											if (metadata is null)
											{
												metadata = new ModMetadata(mod.FullName, "0.0");
											}
											entry.ModMetadata = metadata;
										}
										catch (FileNotFoundException)
										{
											ModMetadata metadata = new ModMetadata(mod.FullName, "0.0");
											entry.ModMetadata = metadata;
										}
										entry.Source = assemblyconflict.Item2;
										entry.Issue = $"unable to load steam mod, a local version of this mod is already loaded";
										IssuedMods.Value.Add(entry);
										break;
									}
									else if (loaded.ModType.Assembly.Location.Contains(WTFModLoader.SteamModsDirectory) && assemblyconflict.Item2.Contains(WTFModLoader.ModsDirectory))
									{
										var mod = entry.ModType;
										try
										{
											string metadataPath = Path.Combine(
												Path.GetDirectoryName(assemblyconflict.Item2),
												$"{Path.GetFileNameWithoutExtension(assemblyconflict.Item2)}.json");
											ModMetadata metadata = _metadataProvider.Read<ModMetadata>(metadataPath);
											if (metadata is null)
											{
												metadata = new ModMetadata(mod.FullName, "0.0");
											}
											entry.ModMetadata = metadata;
										}
										catch (FileNotFoundException)
										{
											ModMetadata metadata = new ModMetadata(mod.FullName, "0.0");
											entry.ModMetadata = metadata;
										}
										entry.Source = assemblyconflict.Item2;
										entry.Issue = $"unable to load local mod version, unsub this mod from Steam to resolve";
										IssuedMods.Value.Add(entry);
										break;
									}
									if (!IssuedMods.Value.Contains(entry))
									{
										var mod = entry.ModType;
										try
										{
											string metadataPath = Path.Combine(
												Path.GetDirectoryName(assemblyconflict.Item2),
												$"{Path.GetFileNameWithoutExtension(assemblyconflict.Item2)}.json");
											ModMetadata metadata = _metadataProvider.Read<ModMetadata>(metadataPath);
											if (metadata is null)
											{
												metadata = new ModMetadata(mod.FullName, "0.0");
											}
											entry.ModMetadata = metadata;
										}
										catch (FileNotFoundException)
										{
											ModMetadata metadata = new ModMetadata(mod.FullName, "0.0");
											entry.ModMetadata = metadata;
										}
										entry.Source = assemblyconflict.Item2;
										entry.Issue = $"unable to load mod, a different version of this mod is already loaded";
										IssuedMods.Value.Add(entry);
										break;
									}
								}
							}

						}
						if (!IssuedMods.Value.Contains(entry))
						{
							entry.Issue = $"unable to load mod, a different version of this mod is already loaded";
							IssuedMods.Value.Add(entry);
						}
					}
				}
				else
				{
					Resolved.Add(entry);
				}
			}

			foreach (ModEntry entry in Resolved)
			{
				var active = successfullyResolved.Find(e => e.ModType.FullName == entry.ModType.FullName);
				if (active == null)
				{
					successfullyResolved.Add(entry);
				}
				else
				{
					if(conflictingAssemblies.IsValueCreated)
					{ 
						foreach(var assemblyconflict in conflictingAssemblies.Value)
						{
							if(assemblyconflict.Item1 == active.ModType.Assembly)
							{
								if (active.ModType.Assembly.Location.Contains(WTFModLoader.ModsDirectory) && assemblyconflict.Item2.Contains(WTFModLoader.SteamModsDirectory))
								{
									var mod = entry.ModType;
									try
									{					
										string metadataPath = Path.Combine(
											Path.GetDirectoryName(assemblyconflict.Item2),
											$"{Path.GetFileNameWithoutExtension(assemblyconflict.Item2)}.json");
										ModMetadata metadata = _metadataProvider.Read<ModMetadata>(metadataPath);
										if (metadata is null)
										{
											metadata = new ModMetadata(mod.FullName, "0.0");
										}
										entry.ModMetadata = metadata;
									}
									catch (FileNotFoundException)
									{
										ModMetadata metadata = new ModMetadata(mod.FullName, "0.0");
										entry.ModMetadata = metadata;
									}
									entry.Source = assemblyconflict.Item2;
									entry.Issue = $"unable to load steam mod, a local version of this mod is already loaded";
									IssuedMods.Value.Add(entry);
									break;
								}
								else if (active.ModType.Assembly.Location.Contains(WTFModLoader.SteamModsDirectory) && assemblyconflict.Item2.Contains(WTFModLoader.ModsDirectory))
								{
									var mod = entry.ModType;
									try
									{
										string metadataPath = Path.Combine(
											Path.GetDirectoryName(assemblyconflict.Item2),
											$"{Path.GetFileNameWithoutExtension(assemblyconflict.Item2)}.json");
										ModMetadata metadata = _metadataProvider.Read<ModMetadata>(metadataPath);
										if (metadata is null)
										{
											metadata = new ModMetadata(mod.FullName, "0.0");
										}
										entry.ModMetadata = metadata;
									}
									catch (FileNotFoundException)
									{
										ModMetadata metadata = new ModMetadata(mod.FullName, "0.0");
										entry.ModMetadata = metadata;
									}
									entry.Source = assemblyconflict.Item2;
									entry.Issue = $"unable to load local mod version, unsub this mod from Steam to resolve";
									IssuedMods.Value.Add(entry);
									break;
								}
								if (!IssuedMods.Value.Contains(entry))
								{
									var mod = entry.ModType;
									try
									{
										string metadataPath = Path.Combine(
											Path.GetDirectoryName(assemblyconflict.Item2),
											$"{Path.GetFileNameWithoutExtension(assemblyconflict.Item2)}.json");
										ModMetadata metadata = _metadataProvider.Read<ModMetadata>(metadataPath);
										if (metadata is null)
										{
											metadata = new ModMetadata(mod.FullName, "0.0");
										}
										entry.ModMetadata = metadata;
									}
									catch (FileNotFoundException)
									{
										ModMetadata metadata = new ModMetadata(mod.FullName, "0.0");
										entry.ModMetadata = metadata;
									}
									entry.Source = assemblyconflict.Item2;
									entry.Issue = $"unable to load mod, a different version of this mod is already loaded";
									IssuedMods.Value.Add(entry);
									break;
								}
							}
						}

					}
					if (!IssuedMods.Value.Contains(entry))
					{
						entry.Issue = $"unable to load, a different version of this mod is already loaded";
						IssuedMods.Value.Add(entry);
					}
				}
			}
			return successfullyResolved;
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
						metadata = new ModMetadata(mod.FullName, "0.0");
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
					entry.Issue = "dependency not found";
					IssuedMods.Value.Add(entry);
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
					entry.Issue = "conflicts with other mod(s)";
					IssuedMods.Value.Add(entry);
					Logger.Log($"Mod `{entry.ModMetadata.Name} (v{entry.ModMetadata.Version})` failed to resolve conflicts. (conflicting mod(s) definition was found in metadata files)");
				}
			}
			return successfullyResolved;
		}
		private List<ModEntry> ResolveGameversion(List<ModEntry> modsWithMetadata)
		{
			List<ModEntry> successfullyResolved = new List<ModEntry>();
			var conflictResolutionList = modsWithMetadata;
			foreach (ModEntry entry in conflictResolutionList)
			{
				if (entry.ModMetadata.Gameversion is null || entry.ModMetadata.Gameversion.Length == 0)
				{
					successfullyResolved.Add(entry);
					continue;
				}
				List<ModMetadata> currentlyResolvedMods = successfullyResolved.Select(x => x.ModMetadata).ToList();
				currentlyResolvedMods.Add(entry.ModMetadata);
				bool resolved = entry.ModMetadata.TryResolveGameVersion();
				if (resolved)
				{
					successfullyResolved.Add(entry);
				}
				else
				{
					entry.Issue = "incompatible with current game version";
					IssuedMods.Value.Add(entry);
					Logger.Log($"Mod `{entry.ModMetadata.Name} (v{entry.ModMetadata.Version})` failed game version compatibility check. (mod is not compatible with current game version)");
				}
			}
			return successfullyResolved;
		}
		private List<ModEntry> ResolveLoaderversion(List<ModEntry> modsWithMetadata)
		{
			List<ModEntry> successfullyResolved = new List<ModEntry>();
			var conflictResolutionList = modsWithMetadata;
			foreach (ModEntry entry in conflictResolutionList)
			{
				if (entry.ModMetadata.Loaderversion is null || entry.ModMetadata.Loaderversion.Length == 0)
				{
					successfullyResolved.Add(entry);
					continue;
				}
				List<ModMetadata> currentlyResolvedMods = successfullyResolved.Select(x => x.ModMetadata).ToList();
				currentlyResolvedMods.Add(entry.ModMetadata);
				bool resolved = entry.ModMetadata.TryResolveLoaderVersion();
				if (resolved)
				{
					successfullyResolved.Add(entry);
				}
				else
				{
					entry.Issue = "incompatible with current loader version";
					IssuedMods.Value.Add(entry);
					Logger.Log($"Mod `{entry.ModMetadata.Name} (v{entry.ModMetadata.Version})` failed loader version compatibility check. (mod is not compatible with current version of WTFML)");
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
					instantiatedEntries.Last<ModEntry>().Source = entry.Source;
				}
				catch (Exception e)
				{
					Logger.Log($"Mod `{entry.ModMetadata.Name} (v{entry.ModMetadata.Version})` failed to initialize.");
					Logger.Log($"{e}");
					entry.Issue = "failed to initialize";
					IssuedMods.Value.Add(entry);
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
