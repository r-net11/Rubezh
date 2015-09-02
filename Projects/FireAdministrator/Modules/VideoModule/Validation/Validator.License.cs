using FiresecClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common;

namespace VideoModule.Validation
{
	public partial class Validator
	{
		void ValidateLicense()
		{
			if (LicenseHelper.Video)
				return;

			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
				Errors.Add(new VideoValidationError(camera, "Отсутствует лицензия модуля \"GLOBAL Видео\"", ValidationErrorLevel.CannotWrite));
		}
	}
}