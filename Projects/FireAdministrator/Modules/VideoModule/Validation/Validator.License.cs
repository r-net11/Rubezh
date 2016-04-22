using RubezhClient;
using Infrastructure.Common.Validation;
using RubezhAPI.License;
using Infrastructure.Common.License;

namespace VideoModule.Validation
{
	public partial class Validator
	{
		void ValidateLicense()
		{
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
				return;

			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
				Errors.Add(new VideoValidationError(camera, "Отсутствует лицензия модуля \"GLOBAL Видео\"", ValidationErrorLevel.Warning));
		}
	}
}