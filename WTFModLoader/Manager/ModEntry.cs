using System;

namespace WTFModLoader.Manager
{
	public class ModEntry
	{
		public Type ModType { get; }
		public IWTFMod ModInstance { get; }
		public ModMetadata ModMetadata { get; }

		public ModEntry(Type modType, ModMetadata modMetadata) : this(null, modType, modMetadata) { }

		public ModEntry(IWTFMod modInstance, Type modType, ModMetadata modMetadata)
		{
			ModInstance = modInstance;
			ModType = modType;
			ModMetadata = modMetadata;
		}
	}
}