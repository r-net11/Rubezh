using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace DevicesModule.Validation
{
	partial class Validator
	{
		void ValidateGuardUsers()
		{
			var missingPasswordUsers = new List<GuardUser>();

			foreach (var device in _firesecConfiguration.DeviceConfiguration.Devices)
			{
				if ((device.Driver.DriverType == DriverType.USB_Rubezh_2OP) || (device.Driver.DriverType == DriverType.Rubezh_2OP))
				{
					var devicePlusPasswords = new List<string>();

					var deviceZones = new List<Guid>();
					foreach (var childDevice in device.Children)
					{
						if (childDevice.Driver.DriverType == DriverType.AM1_O)
							if (childDevice.ZoneUID != Guid.Empty)
								deviceZones.Add(childDevice.ZoneUID);
					}

					foreach (var guardUser in _firesecConfiguration.DeviceConfiguration.GuardUsers)
					{
						if ((guardUser.DeviceUID == device.UID) || guardUser.ZoneUIDs.Any(x => deviceZones.Contains(x)))
						{
							if (string.IsNullOrEmpty(guardUser.Password))
							{
								if (missingPasswordUsers.Contains(guardUser) == false)
								{
									Errors.Add(new DeviceValidationError(device, "Отсутствует пароль у пользователя прибора " + guardUser.Name, ValidationErrorLevel.CannotWrite));
									missingPasswordUsers.Add(guardUser);
								}
							}
							else
							{
								var devicePlusPassword = device.UID.ToString() + guardUser.Password;
								if (devicePlusPasswords.Contains(devicePlusPassword))
								{
									Errors.Add(new DeviceValidationError(device, "Совпадение пароля у пользователя прибора " + guardUser.Name, ValidationErrorLevel.CannotWrite));
								}
								else
								{
									devicePlusPasswords.Add(devicePlusPassword);
								}
							}

							if (guardUser.Name != null && !Validator.ValidateString(guardUser.Name))
							{
								Errors.Add(new DeviceValidationError(device, "Недопустимые символы в имени пользователя прибора " + guardUser.Name, ValidationErrorLevel.CannotWrite));
							}

							var zones = new List<Zone>();
							foreach (var zoneUID in guardUser.ZoneUIDs)
							{
								var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
								zones.Add(zone);
							}
							if (zones.Count == 0)
							{
								Errors.Add(new DeviceValidationError(device, "Отсутствуют зоны у пользователя пользователя прибора " + guardUser.Name, ValidationErrorLevel.CannotWrite));
							}
						}
					}
				}
			}
		}
	}
}