using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateGuardZones()
		{
			ValidateGuardZoneNoEquality();
			ValidateDevicesinGuardZone();

			foreach (var guardZone in XManager.GuardZones)
			{
				if (IsManyGK())
					ValidateDifferentGK(guardZone);
				ValidateGuardZoneHasNoDevices(guardZone);
			}
		}

		void ValidateGuardZoneNoEquality()
		{
			var zoneNos = new HashSet<int>();
			foreach (var guardZone in XManager.GuardZones)
			{
				if (!zoneNos.Add(guardZone.No))
					Errors.Add(new GuardZoneValidationError(guardZone, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDifferentGK(XGuardZone guardZone)
		{
			var devices = new List<XDevice>();
			foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
			{
				devices.Add(guardZoneDevice.Device);
			}
			if (AreDevicesInSameGK(devices))
				Errors.Add(new GuardZoneValidationError(guardZone, "Зона содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		void ValidateGuardZoneHasNoDevices(XGuardZone guardZone)
		{
			if (guardZone.GuardZoneDevices.Count == 0)
			{
				Errors.Add(new GuardZoneValidationError(guardZone, "К зоне не подключено ни одного устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDevicesinGuardZone()
		{
			var devices = new HashSet<XDevice>();
			foreach (var guardZone in XManager.GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					if (!devices.Add(guardZoneDevice.Device))
					{
						Errors.Add(new GuardZoneValidationError(guardZone, "Устройство " + guardZoneDevice.Device.PresentationName + " уже подключено к другой охранной зоне", ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}
	}
}