﻿using System.Collections.Generic;
using RubezhClient;
using Infrastructure.Common.Validation;

namespace VideoModule.Validation
{
	public partial class Validator
	{
		private void ValidateAddress()
		{
			var addressList = new List<string>();
			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
			{
				//if (addressList.Contains(camera.Ip))
				//	Errors.Add(new VideoValidationError(camera, "Камера с таким адресом уже существует " + camera.Ip, ValidationErrorLevel.CannotSave));
				//addressList.Add(camera.Ip);
			}
		}
	}
}