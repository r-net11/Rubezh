using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace VideoModule.Validation
{
	public partial class Validator
	{
		private void ValidateAddress()
		{
			var addressList = new List<string>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				if (addressList.Contains(camera.Address))
					Errors.Add(new VideoValidationError(camera, "Камера с таким адресом уже существует " + camera.Address, ValidationErrorLevel.CannotSave));
				addressList.Add(camera.Address);
			}
		}
	}
}
