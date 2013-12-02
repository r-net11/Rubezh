using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;
using System;

namespace GKModule.Validation
{
    public static partial class Validator
    {
        static void ValidateDirections()
        {
            ValidateDirectionNoEquality();
			ValidateNSDifferentDevices();

            foreach (var direction in XManager.Directions)
            {
                if (IsManyGK())
                    ValidateDifferentGK(direction);
                if (ValidateEmptyDirection(direction))
                {
                    if (MustValidate("В направлении отсутствуют входные устройства или зоны"))
                        ValidateDirectionInputCount(direction);
                    if (MustValidate("В направлении отсутствуют выходные устройства"))
                    {
                        if (direction.IsNS)
                        {
                            ValidateDirectionNS(direction);
                        }
                        else
                        {
                            ValidateDirectionOutputCount(direction);
                        }
                    }

                    ValidateEmptyZoneInDirection(direction);
                    ValidateSameInputOutputDevices(direction);
                }
            }
        }

        static void ValidateDirectionNoEquality()
        {
            var directionNos = new HashSet<int>();
            foreach (var direction in XManager.Directions)
            {
                if (!directionNos.Add(direction.No))
                    Errors.Add(new DirectionValidationError(direction, "Дублируется номер", ValidationErrorLevel.CannotWrite));
            }
        }

        static void ValidateDifferentGK(XDirection direction)
        {
            var devices = new List<XDevice>();
            devices.AddRange(direction.InputDevices);
            devices.AddRange(direction.OutputDevices);
            foreach (var zone in direction.InputZones)
            {
                devices.AddRange(zone.Devices);
            }

            if (AreDevicesInSameGK(devices))
                Errors.Add(new DirectionValidationError(direction, "Направление содержит объекты устройства разных ГК", ValidationErrorLevel.CannotWrite));
        }

        static void ValidateDirectionInputCount(XDirection direction)
        {
            if (direction.InputDevices.Count + direction.InputZones.Count == 0)
                Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют входные устройства или зоны", ValidationErrorLevel.CannotWrite));
        }

        static void ValidateDirectionOutputCount(XDirection direction)
        {
            if (direction.OutputDevices.Count == 0)
                Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют выходные устройства", ValidationErrorLevel.CannotWrite));
        }

        static void ValidateDirectionNS(XDirection direction)
        {
			if(direction.Regime != 2)
				Errors.Add(new DirectionValidationError(direction, "Режим НС должен быть выключено", ValidationErrorLevel.CannotWrite));

            var nsDevices = new List<XDevice>();
            foreach (var nsDeviceUID in direction.NSDeviceUIDs)
            {
                var nsDevice = XManager.Devices.FirstOrDefault(x => x.UID == nsDeviceUID);
                if (nsDevice != null)
                {
                    nsDevices.Add(nsDevice);
                }
            }
            var pumpsCount = nsDevices.Count(x => (x.DriverType == XDriverType.Pump && x.IntAddress <= 8) || x.DriverType == XDriverType.RSR2_Bush);
            if (pumpsCount == 0)
            {
                Errors.Add(new DirectionValidationError(direction, "В насосной станции отсутствуют насосы", ValidationErrorLevel.CannotWrite));
            }
            else
            {
                if (direction.NSPumpsCount > pumpsCount)
                    Errors.Add(new DirectionValidationError(direction, "В насосной количество основных насосов меньше реально располагаемых", ValidationErrorLevel.CannotWrite));
            }

            var am1_TCount = nsDevices.Count(x => x.DriverType == XDriverType.AM1_T);
            if (am1_TCount > 1)
                Errors.Add(new DirectionValidationError(direction, "В насосной станции количество подключенных технологических меток больше 1", ValidationErrorLevel.CannotWrite));

			var jnPumpsCount = nsDevices.Count(x => x.DriverType == XDriverType.Pump && x.IntAddress == 12);
			if (jnPumpsCount > 1)
				Errors.Add(new DirectionValidationError(direction, "В насосной станции количество подключенных ЖН больше 1", ValidationErrorLevel.CannotWrite));

			if (jnPumpsCount == 1)
			{
				if(direction.InputZones.Count > 0)
					Errors.Add(new DirectionValidationError(direction, "В насосной станции c ЖН не могут участвовать входные зоны", ValidationErrorLevel.CannotWrite));

				if (!direction.InputDevices.All(x=>x.DriverType == XDriverType.AM_1))
					Errors.Add(new DirectionValidationError(direction, "В насосной станции c ЖН во входных устройствах могут участвовать только АМ1", ValidationErrorLevel.CannotWrite));
			}
        }

        static bool ValidateEmptyDirection(XDirection direction)
        {
            var count = direction.InputZones.Count + direction.InputDevices.Count + direction.OutputDevices.Count;
            if (count == 0)
            {
                Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют входные или выходные объекты", ValidationErrorLevel.CannotWrite));
                return false;
            }
            return true;
        }

        static void ValidateEmptyZoneInDirection(XDirection direction)
        {
            foreach (var zone in direction.InputZones)
            {
                if (zone.Devices.Count == 0)
                {
                    Errors.Add(new DirectionValidationError(direction, "В направление входит пустая зона " + zone.PresentationName, ValidationErrorLevel.CannotWrite));
                }
            }
        }

        static void ValidateSameInputOutputDevices(XDirection direction)
        {
            foreach (var device in direction.OutputDevices)
            {
                if (direction.InputDevices.Any(x => x.UID == device.UID))
                {
                    Errors.Add(new DirectionValidationError(direction, "Устройство " + device.PresentationName + " участвует во входных и выходных зависимостях направления", ValidationErrorLevel.CannotWrite));
                }
            }
        }

		static void ValidateNSDifferentDevices()
		{
			var nsDevices = new HashSet<Guid>();
			foreach (var direction in XManager.Directions)
			{
				if (direction.IsNS)
				{
					foreach (var device in direction.NSDevices)
					{
						if(!nsDevices.Add(device.UID))
							Errors.Add(new DirectionValidationError(direction, "Устройство " + device.PresentationName + " участвует в различных НС", ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}
    }
}