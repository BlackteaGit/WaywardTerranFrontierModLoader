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
			if (Directory.Exists(steamdirectory))
			{
				modTypes = FilterTypesInDirectory(steamdirectory, IsModType).ToList();
			}
			modTypes.AddRange(FilterTypesInDirectory(directory, IsModType).ToList());
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
					Assembly loadedFile = Assembly.UnsafeLoadFrom(foundFile);
					modTypes = modTypes.Concat(FilterTypes(loadedFile, predicate)).ToList();
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