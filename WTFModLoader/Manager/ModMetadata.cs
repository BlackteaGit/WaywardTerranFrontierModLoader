using Newtonsoft.Json;
using WTFModLoader.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WTFModLoader.Manager
{
	public class ModMetadata
	{
		public ModMetadata(string name) : this(name, "0.0", string.Empty) { }

		public ModMetadata(string name, string version) : this(name, version, string.Empty) { }

		[JsonConstructor]
		public ModMetadata(string name, string version, string description)
		{
			Name = name;
			Version = version;
			Description = description;
			Dependencies = new List<ModMetadata>();
			Conflicts = new List<ModMetadata>();
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public string Version { get; private set; }
		public IList<ModMetadata> Dependencies { get; private set; }
		public IList<ModMetadata> Conflicts { get; private set; }

		public bool TryResolveDependencies(IList<ModMetadata> modList)
		{
			List<ModMetadata> resolvedMods = new List<ModMetadata>();
			List<ModMetadata> unresolvedMods = new List<ModMetadata>();

			Func<ModMetadata, bool> AreMetadataEqualTo(ModMetadata lhs)
			{
				return (rhs) => string.Equals(lhs.Name, rhs.Name) && lhs.Version == rhs.Version;
			} 


			bool ResolveDependenciesForMod(ModMetadata mod)
			{
				bool result = false;

				if (!modList.Any(AreMetadataEqualTo(mod)))
				{
					return false;
				}

				if (mod.Dependencies.Count == 0)
				{
					return true;
				}

				unresolvedMods.Add(mod);
				foreach (ModMetadata dependency in mod.Dependencies)
				{
					if (!resolvedMods.Any(AreMetadataEqualTo(dependency)))
					{
						if (unresolvedMods.Any(AreMetadataEqualTo(dependency)))
						{
							throw new ModCircularDependencyException(this, dependency);
						}
						result = ResolveDependenciesForMod(dependency);
					}
					else
					{
						result = true;
					}
				}
				resolvedMods.Add(mod);
				unresolvedMods.Remove(mod);
				return result;
			}

			return ResolveDependenciesForMod(this);
		}

		public bool TryResolveConflicts(IList<ModMetadata> modList)
		{
			List<ModMetadata> resolvedMods = new List<ModMetadata>();
			List<ModMetadata> unresolvedMods = new List<ModMetadata>();

			Func<ModMetadata, bool> AreMetadataEqualTo(ModMetadata lhs)
			{
				return (rhs) => string.Equals(lhs.Name, rhs.Name) && lhs.Version == rhs.Version;
			}


			bool ResolveConflictsForMod(ModMetadata mod)
			{
				bool result = false;

				if (modList.Any(AreMetadataEqualTo(mod)))
				{
					return false;
				}

				if (mod.Conflicts.Count == 0)
				{
					return true;
				}

				unresolvedMods.Add(mod);
				foreach (ModMetadata conflict in mod.Conflicts)
				{
					if (resolvedMods.Any(AreMetadataEqualTo(conflict)))
					{
						if (!unresolvedMods.Any(AreMetadataEqualTo(conflict)))
						{
							throw new ModCircularDependencyException(this, conflict);
						}
						result = ResolveConflictsForMod(conflict);
					}
					else
					{
						result = true;
					}
				}
				resolvedMods.Add(mod);
				unresolvedMods.Remove(mod);
				return result;
			}

			return ResolveConflictsForMod(this);
		}

	}
}
