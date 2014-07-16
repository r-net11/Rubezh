using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateCodes()
		{
			ValidateGuardPasswordEquality();

			foreach (var code in XManager.DeviceConfiguration.Codes)
			{
				if (IsManyGK())
					ValidateCodeDifferentGK(code);

				ValidateEmptyPassword(code);
				ValidateEmptyCode(code);
			}
		}

		void ValidateGuardPasswordEquality()
		{
			var codeNos = new HashSet<string>();
			foreach (var code in XManager.DeviceConfiguration.Codes)
			{
				if (!codeNos.Add(code.Password))
					Errors.Add(new CodeValidationError(code, "Дублируется пароль кода", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateEmptyPassword(XCode code)
		{
			if (string.IsNullOrEmpty(code.Password))
				Errors.Add(new CodeValidationError(code, "Пустой пароль кода", ValidationErrorLevel.CannotWrite));
		}

		void ValidateCodeDifferentGK(XCode code)
		{
			if (IsManyGK())
			{
				var devicesInZones = new List<XDevice>();
				foreach (var guardZone in code.GuardZones)
				{
					foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
					{
						devicesInZones.Add(guardZoneDevice.Device);
					}
				}
				if (AreDevicesInSameGK(devicesInZones))
						Errors.Add(new CodeValidationError(code, "Код содержит зоны разных ГК", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateEmptyCode(XCode code)
		{
			if(code.GuardZoneUIDs.Count == 0)
				Errors.Add(new CodeValidationError(code, "У кода отсутствуют зоны", ValidationErrorLevel.CannotWrite));
		}
	}
}