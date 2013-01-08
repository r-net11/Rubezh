using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
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
                                    _errors.Add(new DeviceValidationError(device, "Отсутствует информация у пользователя(Отсутствует пароль у пользователя\"" + guardUser.Name + "\")", ValidationErrorLevel.CannotWrite));
                                    missingPasswordUsers.Add(guardUser);
                                }
                            }
                            else
                            {
                                var devicePlusPassword = device.UID.ToString() + guardUser.Password;
                                if (devicePlusPasswords.Contains(devicePlusPassword))
                                {
                                    _errors.Add(new DeviceValidationError(device, "Нарушена уникальность пользователей прибора(Совпадение пароля у пользователя\"" + guardUser.Name + "\")", ValidationErrorLevel.CannotWrite));
                                }
                                else
                                {
                                    devicePlusPasswords.Add(devicePlusPassword);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}