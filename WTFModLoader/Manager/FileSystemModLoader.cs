using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WTFModLoader.Manager
{
	public class FileSystemModLoader
	{
		
		public List<Type> LoadModTypesFromDirectory(string directory, string steamdirectory)
		{
			List<Type> modTypes = new List<Type>();
			modTypes = FilterTypesInDirectory(directory, IsModType).ToList();
			if (Directory.Exists(steamdirectory))
			{
				modTypes.AddRange(FilterTypesInDirectory(steamdirectory, IsModType).ToList());
			}
			return modTypes;
		}


		private static IEnumerable<Type> FilterTypesInDirectory(string directory, Func<Type, bool> predicate)
		{
			List<Type> modTypes = new List<Type>();
			var fileSearch = Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories);
			foreach (string foundFile in fileSearch)
			{
				try
				{		
					if (!foundFile.Contains("WTFModLoader.dll") && !foundFile.Contains("0Harmony.dll"))
					{
						Assembly loadedFile = null;
						if (WTFModLoader.legacyLoad)
						{
							loadedFile = Assembly.LoadFile(foundFile);
						}
						else
						{ 
							loadedFile = Assembly.UnsafeLoadFrom(foundFile);
						}
						if (!foundFile.Contains(loadedFile.Location))
						{ 
							WTFModLoader._modManager.conflictingAssemblies.Value.Add(new Tuple<Assembly, string>(loadedFile, foundFile));
						}
						modTypes = modTypes.Concat(FilterTypes(loadedFile, predicate)).ToList();				
					}
				}
				catch (Exception e)
				{
					Logger.Log($"Failed to load mod file `{foundFile}`.");
					Logger.Log(e.ToString());
					continue;
				}
			}
			return modTypes;
		}
		private static IEnumerable<Type> FilterTypes(Assembly asm, Func<Type, bool> predicate)
		{
			return asm.GetTypes().Where(predicate);
		}

		private static bool IsModType(Type type)
		{
			return typeof(IWTFMod).IsAssignableFrom(type);
		}

	}
}