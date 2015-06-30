using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;
using System;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateGuardZones()
		{
			ValidateGuardZoneNoEquality();
			ValidateGuardZoneSameDevices();

			foreach (var guardZone in GKManager.GuardZones)
			{
				if (IsManyGK)
					ValidateDifferentGK(guardZone);
				ValidateGuardZoneHasNoDevices(guardZone);
				ValidateGuardZoneCodesCount(guardZone);
				ValidateGuardZoneSameCodeReaderEnterTypes(guardZone);
			}
		}

		void ValidateGuardZoneNoEquality()
		{
			var zoneNos = new HashSet<int>();
			foreach (var guardZone in GKManager.GuardZones)
			{
				if (!zoneNos.Add(guardZone.No))
					Errors.Add(new GuardZoneValidationError(guardZone, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateGuardZoneSameDevices()
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var guardZone in GKManager.GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
					{
						if (!deviceUIDs.Add(guardZoneDevice.Device.UID))
							Errors.Add(new GuardZoneValidationError(guardZone, "Устройство " + guardZoneDevice.Device.PresentationName + " уже участвует в другой охранной зоне", ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}

		void ValidateDifferentGK(GKGuardZone guardZone)
		{
			var devices = new List<GKDevice>();
			foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
			{
				devices.Add(guardZoneDevice.Device);
			}
			if (AreDevicesInSameGK(devices))
				Errors.Add(new GuardZoneValidationError(guardZone, "Зона содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		void ValidateGuardZoneHasNoDevices(GKGuardZone guardZone)
		{
			if (guardZone.GuardZoneDevices.Count == 0)
			{
				Errors.Add(new GuardZoneValidationError(guardZone, "К зоне не подключено ни одного устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateGuardZoneCodesCount(GKGuardZone guardZone)
		{
			foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
			{
				if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
				{
					if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs.Count > 100)
						Errors.Add(new GuardZoneValidationError(guardZone, "Количестао кодов для постановки у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite));
					if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs.Count > 100)
						Errors.Add(new GuardZoneValidationError(guardZone, "Количестао кодов для снятия с охраны у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite));
					if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs.Count > 100)
						Errors.Add(new GuardZoneValidationError(guardZone, "Количестао кодов для изменения состояни у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite));
					if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs.Count > 100)
						Errors.Add(new GuardZoneValidationError(guardZone, "Количестао кодов на вызов тревоги у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateGuardZoneSameCodeReaderEnterTypes(GKGuardZone guardZone)
		{
			foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
			{
				if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
				{
					var enterTypes = new HashSet<GKCodeReaderEnterType>();

					if(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						if(!enterTypes.Add(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType))
							Errors.Add(new GuardZoneValidationError(guardZone, "Дублируется метод ввода у кодонаборника " + guardZoneDevice.Device.PredefinedName, ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}