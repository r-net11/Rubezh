using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
    public static partial class Validator
    {
        static void ValidateDirections()
        {
            foreach (var direction in XManager.DeviceConfiguration.Directions)
            {
                if (IsManyGK())
                    ValidateDifferentGK(direction);
                ValidateDirectionInputCount(direction);
                ValidateDirectionOutputCount(direction);
            }
        }

        static void ValidateDifferentGK(XDirection direction)
        {
            if (AreDevicesInSameGK(direction.InputDevices))
                Errors.Add(new DirectionValidationError(direction, "Направление содержит входные устройства разных ГК", ValidationErrorLevel.CannotWrite));

            if (AreDevicesInSameGK(direction.OutputDevices))
                Errors.Add(new DirectionValidationError(direction, "Направление содержит выходные устройств разных ГК", ValidationErrorLevel.CannotWrite));

            var devicesInZones = new List<XDevice>();
            foreach (var zone in direction.InputZones)
            {
                devicesInZones.AddRange(zone.Devices);
            }

            if (AreDevicesInSameGK(devicesInZones))
                Errors.Add(new DirectionValidationError(direction, "Направление содержит выходные зоны разных ГК", ValidationErrorLevel.CannotWrite));
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
    }
}