using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateGuard()
		{
			ValidateGuardPasswordEquality();

			foreach (var guardUser in XManager.DeviceConfiguration.GuardUsers)
			{
				if (IsManyGK())
					ValidateGuardUserDifferentGK(guardUser);

				if (MustValidate("Пустой пароль охранного пользователя"))
					ValidateEmptyPassword(guardUser);
				if (MustValidate("У охранного пользователя отсутствуют зоны"))
					ValidateEmptyGuardUser(guardUser);
			}
		}

		void ValidateGuardPasswordEquality()
		{
			var guardUserNos = new HashSet<string>();
			foreach (var guardUser in XManager.DeviceConfiguration.GuardUsers)
			{
				if (!guardUserNos.Add(guardUser.Password))
					Errors.Add(new GuardUserValidationError(guardUser, "Дублируется пароль охранного пользователя", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateEmptyPassword(XGuardUser guardUser)
		{
			if (string.IsNullOrEmpty(guardUser.Password))
				Errors.Add(new GuardUserValidationError(guardUser, "Пустой пароль охранного пользователя", ValidationErrorLevel.CannotWrite));
		}

		void ValidateGuardUserDifferentGK(XGuardUser guardUser)
		{
			if (IsManyGK())
			{
				var devicesInZones = new List<XDevice>();
				foreach (var zone in guardUser.Zones)
				{
					devicesInZones.AddRange(zone.Devices);
				}
				if (AreDevicesInSameGK(devicesInZones))
						Errors.Add(new GuardUserValidationError(guardUser, "Охранный пользователь содержит зоны разных ГК", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateEmptyGuardUser(XGuardUser guardUser)
		{
			if(guardUser.ZoneUIDs.Count == 0)
				Errors.Add(new GuardUserValidationError(guardUser, "У охранного пользователя отсутствуют зоны", ValidationErrorLevel.CannotWrite));
		}
	}
}