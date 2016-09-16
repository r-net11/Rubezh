using GenerateKeyApplication.Model.Entity;
using GenerateKeyApplication.Model.Enums;
using License.Model.Entities;
using System;

namespace Licensing.Services
{
	public static class LicenseFactory
	{
		private const int UsersForDemoLicense = 5;
		public static ILicenseEntity CreateLicense(ClientLicense clientLicense)
		{
			if (clientLicense == null)
				throw new ArgumentNullException("clientLicense");

			switch (clientLicense.LicenseType)
			{
				case LicenseType.Product:
					return new LicenseEntity
					{
						UID = clientLicense.UID,
						IsEnabledAutomation = clientLicense.IsEnabledAutomation,
						IsEnabledPhotoVerification = clientLicense.IsEnabledPhotoVerification,
						IsEnabledRVI = clientLicense.IsEnabledRVI,
						IsEnabledURV = clientLicense.IsEnabledURV,
						IsUnlimitedUsers = clientLicense.IsUnlimitedUsers,
						OperatorConnectionsNumber = clientLicense.OperatorConnectionsNumber.GetValueOrDefault(),
						TotalUsers = clientLicense.TotalUsers
					};

				case LicenseType.Demo:
					return GetDemoLicense(clientLicense.UID);
			}

			return null;
		}

		public static LicenseEntity GetDemoLicense(string userKey)
		{
			return new LicenseEntity
			{
				UID = userKey,
				IsEnabledAutomation = true,
				IsEnabledPhotoVerification = true,
				IsEnabledRVI = true,
				IsEnabledURV = true,
				IsUnlimitedUsers = false,
				OperatorConnectionsNumber = default(int),
				TotalUsers = UsersForDemoLicense
			};
		}
	}
}
