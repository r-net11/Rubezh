using GenerateKeyApplication.Model.Entity;

namespace Licencing.Services
{
	public interface ILicenseGeneratorService
	{
		void GenerateLicenseFile(ClientLicense license, string pathToFile);
	}
}
