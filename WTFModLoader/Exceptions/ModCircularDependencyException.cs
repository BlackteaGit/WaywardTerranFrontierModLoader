using WTFModLoader.Manager;
using System;
using System.Runtime.Serialization;

namespace WTFModLoader.Exceptions
{
	internal class ModCircularDependencyException : Exception
	{
		private ModMetadata modMetadata;
		private ModMetadata dependency;

		public ModCircularDependencyException()
		{
		}

		public ModCircularDependencyException(string message) : base(message)
		{
		}

		public ModCircularDependencyException(ModMetadata modMetadata, ModMetadata dependency)
		/*	: base($"Circular dependency found between mods " +
				  $"`{modMetadata.Name}` and " +
				  $"`{dependency.Name}`.")
		*/
		: base($"Circular dependency found between mods " +
			  $"`{modMetadata.Name} (v{modMetadata.Version})` and " +
			  $"`{dependency.Name} (v{dependency.Version})`.")
		
		{
			this.modMetadata = modMetadata;
			this.dependency = dependency;
		}
	}
}