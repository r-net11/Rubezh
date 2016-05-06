using StrazhAPI.Enums;

namespace Infrastructure.Client.Licensing
{
	public interface ILicenseService
	{
		bool CheckLicenseExising();
		bool CanConnect();
		bool CanLoadModule(ModuleType type);
	}
}
