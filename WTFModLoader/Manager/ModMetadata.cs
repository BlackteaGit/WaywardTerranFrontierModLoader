using Newtonsoft.Json;
using WTFModLoader.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WTFModLoader.Manager
{
    public class ModMetadata
    {
        private string wrappedName1;

        public ModMetadata(string name) : this(name, string.Empty, "0.0", string.Empty, string.Empty, string.Empty) { }

        public ModMetadata(string name, string version) : this(name, string.Empty, version, string.Empty, string.Empty, string.Empty) { }

        [JsonConstructor]
        public ModMetadata(string name, string author, string version, string description, string gameversion, string loaderversion)
        {
            Name = name;
            Author = author;
            Version = version;
            Description = description;
            Dependencies = new List<ModMetadata>();
            Conflicts = new List<ModMetadata>();
            Gameversion = gameversion;
            Loaderversion = loaderversion;
        }

        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Version { get; private set; }
        public string Description { get; private set; }
        public IList<ModMetadata> Dependencies { get; private set; }
        public IList<ModMetadata> Conflicts { get; private set; }
        public string Gameversion { get; private set; }
        public string Loaderversion { get; private set; }
        public string wrappedName {
            get
            {
                if(wrappedName1 == null)
                {
                    wrappedName1 = Name.Substring(0, Math.Max(Name.IndexOf('.'), Name.IndexOf('.') * -1 * Name.Length)).Replace('_',' ');
                }
                return wrappedName1;
            }
            private set => wrappedName1 = value; }

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

        public bool TryResolveGameVersion()
        {
            bool ResolveGameVersionForMod(ModMetadata mod)
            {
                bool result = false;

                if (mod.Gameversion.Length == 0)
                {
                    return true;
                }

                if (mod.Gameversion == CoOpSpRpG.CONFIG.version)
                {
                    return true;
                }

                try
                {
                    string currentgameversion = CoOpSpRpG.CONFIG.version;
                    string modrequiredgameversion = mod.Gameversion;
                    float formatedCurrentGameversion = 0;
                    float formatedModRequiredGameversion = 0;

                    if (currentgameversion.Length >= 5)
                    {

                        var toformat = currentgameversion.Substring(0, 5);
                        int foundS1 = toformat.IndexOf(".");
                        int foundS2 = toformat.IndexOf(".", foundS1 + 1);

                        if (foundS1 != foundS2 && foundS1 >= 0 && foundS2 >= 0)
                        {
                            toformat = toformat.Remove(foundS2, 1);
                        }
                        else
                        {
                            toformat = toformat.Substring(0, 3);
                        }

                        formatedCurrentGameversion = Convert.ToSingle(toformat, System.Globalization.CultureInfo.InvariantCulture);

                        if (modrequiredgameversion.Length == 3)
                        {
                            toformat = currentgameversion.Substring(0, 3);
                            formatedCurrentGameversion = Convert.ToSingle(toformat, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    if (modrequiredgameversion.Length == 3)
                    {
                        var toformat = modrequiredgameversion.Substring(0, 3);
                        formatedModRequiredGameversion = Convert.ToSingle(toformat, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        if (modrequiredgameversion.Length == 5)
                        {

                            var toformat = modrequiredgameversion.Substring(0, 5);
                            int foundS1 = toformat.IndexOf(".");
                            int foundS2 = toformat.IndexOf(".", foundS1 + 1);

                            if (foundS1 != foundS2 && foundS1 >= 0)
                                toformat = toformat.Remove(foundS2, 1);

                            formatedModRequiredGameversion = Convert.ToSingle(toformat, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    if (formatedModRequiredGameversion != 0 && formatedCurrentGameversion != 0)
                    {
                        if (formatedCurrentGameversion >= formatedModRequiredGameversion)
                            result = true;
                    }
                    else
                    {
                        if (modrequiredgameversion.Length <= 5)
                        {
                            Logger.Log($"Mod `{mod.Name} (v{mod.Version})` has game version metadata in unknown format: `{mod.Gameversion}` and will not be loaded");
                        }

                        result = false;
                    }

                }
                catch (Exception e)
                {
                    Logger.Log($"{e}");
                    result = false;
                }

                return result;
            }

            return ResolveGameVersionForMod(this);
        }

        public bool TryResolveLoaderVersion()
        {
            bool ResolveLoaderVersionForMod(ModMetadata mod)
            {
                bool result = false;

                if (mod.Loaderversion.Length == 0)
                {
                    return true;
                }

                float versioncheck = Convert.ToSingle(mod.Loaderversion, System.Globalization.CultureInfo.InvariantCulture);
                float comparevalue = Convert.ToSingle(WTFModLoader.CurrentBuildVersion, System.Globalization.CultureInfo.InvariantCulture);
                if (versioncheck >= comparevalue)
                    result = true;


                return result;
            }

            return ResolveLoaderVersionForMod(this);
        }

    }
}
