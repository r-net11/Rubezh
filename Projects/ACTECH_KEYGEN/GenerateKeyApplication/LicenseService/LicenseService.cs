using LicenseService.Entities;
using Licensing.Services;

namespace LicenseService
{
	public class LicenseService : ILicenseService
	{
		public string GetDemoLicense(string userKey)
		{
			var licenseService = new LicenseGeneratorService();
			var result = licenseService.GenerateServiceLicense(LicenseFactory.GetDemoLicense(userKey));

			return result;
		}

		public string GetProductLicense(ServiceLicenseData license)
		{
			var licenseService = new LicenseGeneratorService();
			return licenseService.GenerateServiceLicense(license.ToDTO());
		}

		public string TestConnection()
		{
			return "Connection successful";
		}
	}
}
