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
			//if (AreDevicesInSameGK(guardZone.Devices))
			//    Errors.Add(new GuardZoneValidationError(guardZone, "Зона содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		void ValidateGuardZoneHasNoDevices(XGuardZone guardZone)
		{
			//if (guardZone.Devices.Count == 0)
			//{
			//    Errors.Add(new GuardZoneValidationError(guardZone, "К зоне не подключено ни одного устройства", ValidationErrorLevel.CannotWrite));
			//}
		}
	}
}