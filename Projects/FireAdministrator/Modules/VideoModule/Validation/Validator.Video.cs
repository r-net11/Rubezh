using System.Collections.Generic;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Validation;

namespace VideoModule.Validation
{
	public partial class Validator
	{
		/// <summary>
		/// Проверяет разрешение на присутствие камер в конфигурации на основе данных лицензии
		/// </summary>
		private void ValidateCamerasAvailabilityAgainstLicenseData()
		{
			if (!ServiceFactory.ConfigurationElementsAvailabilityService.IsCamerasAvailable)
				foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
				{
					Errors.Add(new VideoValidationError(camera, "Камера не может быть загружена по причине лицензионных ограничений", ValidationErrorLevel.CannotSave));
				}
		}
	}
}