using WTFModLoader.Manager;

namespace WTFModLoader
{
	public interface IWTFMod
	{
		ModLoadPriority Priority { get; }

		void Initialize();
	}
}