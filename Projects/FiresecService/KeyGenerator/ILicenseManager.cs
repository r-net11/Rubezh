
using FiresecAPI.Enums;

namespace KeyGenerator
{
	public interface ILicenseManager
	{
		Entities.LicenseEntity CurrentLicense { get; }
		bool VerifyProductKey(string key);
		bool IsValidExistingKey();
		void SaveToFile(string productKey, string userKey);
		string GetUserKey();
		bool CanConnect();
		bool CanLoadModule(ModuleType type);
		bool CanAddCard(int currentCount);
	}
}
