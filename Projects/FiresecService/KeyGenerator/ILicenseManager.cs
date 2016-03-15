
using FiresecAPI.Enums;

namespace KeyGenerator
{
	public interface ILicenseManager
	{
		Entities.LicenseEntity CurrentLicense { get; }
		bool IsValidExistingKey();
		string GetUserKey();
		bool CanConnect();
		bool CanLoadModule(ModuleType type);
		bool CanAddCard(int currentCount);
		bool LoadLicenseFromFile(string pathToLicense);
	}
}
