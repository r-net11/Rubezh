using System;
using GenerateKeyApplication.Common;
using GenerateKeyApplication.Model.Entity;
using Licencing.Services;
using License.Model.Entities;
using Licensing.Generator;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Security.Cryptography;

namespace Licensing.Services
{
	[Export(typeof(ILicenseGeneratorService))]
	public sealed class LicenseGeneratorService : ILicenseGeneratorService
	{
		private const string CertificatePassword = "F+*A^!k=J3+=ar-7jr4-R";
		private const string CertificateName = "ACTechCert.pfx";
		private const string Key = "Wr6CZRJix4jsxIUAVfyf9UVYUaNhv0IjhaeheDlV5FI=";
		private const string IV = "X8WjoAYe/R4KwMDpEZtqjw==";
		private readonly LicenseGenerator _licenseGenerator;

		[ImportingConstructor]
		public LicenseGeneratorService()
		{
			_licenseGenerator = new LicenseGenerator();
		}

		public void GenerateLicenseFile(ClientLicense license, string pathToFile)
		{
			var serverLicense = LicenseFactory.CreateLicense(license);

			if (!ValidateKeyFormat(serverLicense))
				throw new FormatException("Incorrect license format");

			var resultString = GenerateLicenseString(serverLicense);

			using (var rdProvider = new RijndaelManaged())
			{
				var secretFile = new SecretFile(rdProvider, Key, IV, pathToFile);

				secretFile.SaveSensitiveData(resultString);
			}
		}

		public string GenerateServiceLicense(LicenseEntity license)
		{
			if (license == null || string.IsNullOrEmpty(license.UID))
				throw new ArgumentNullException("license");

			if(!ValidateKeyFormat(license))
				throw new FormatException("Incorrect license format");

			var resultString = GenerateLicenseString(license);

			using (var rdProvider = new RijndaelManaged())
			{
				var secretFile = new SecretFile(rdProvider, Key, IV);

				return secretFile.GetEncryptString(resultString);
			}
		}

		private string GenerateLicenseString(ILicenseEntity license)
		{
			var bytes = Utilites.GetBytesFrom(Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}.{2}", "Licensing.Services", "Resources", CertificateName)));
			return _licenseGenerator.GenerateLicenseBASE64String(license, bytes, CertificatePassword);
		}

		private static bool ValidateKeyFormat(ILicenseEntity license)
		{
			if (license == null || string.IsNullOrWhiteSpace(license.UID)) return false;

			var ids = license.UID.Split('-');

			return (ids.Length == 4);
		}
	}
}
