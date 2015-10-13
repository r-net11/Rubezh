using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using System;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateGuardZones()
		{
			ValidateGuardZoneNoEquality();
			ValidateGuardZoneSameDevices();
			ValidateCodeForZones();

			foreach (var guardZone in GKManager.GuardZones)
			{
				if (ValidateGuardZoneOnlyOnOneGK(guardZone))
				{
					ValidateGuardZoneHasNoDevices(guardZone);
					ValidateGuardZoneCodesCount(guardZone);
					ValidateGuardZoneSameCodeReaderEnterTypes(guardZone);
				}
			}
		}

		/// <summary>
		/// Валидация уникальности номеров охранных зон
		/// </summary>
		void ValidateGuardZoneNoEquality()
		{
			var nos = new HashSet<int>();
			foreach (var guardZone in GKManager.GuardZones)
			{
				if (!nos.Add(guardZone.No))
					Errors.Add(new GuardZoneValidationError(guardZone, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Валидация того, что кодонаборники и контроллеры Wiegand не могут учавствовать в разных охранных зонах
		/// </summary>
		void ValidateGuardZoneSameDevices()
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var guardZone in GKManager.GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
					{
						if (!deviceUIDs.Add(guardZoneDevice.Device.UID))
							Errors.Add(new GuardZoneValidationError(guardZone, "Устройство " + guardZoneDevice.Device.PresentationName + " уже участвует в другой охранной зоне", ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}

		/// <summary>
		/// Валидация того, что один и тот же код может не может выполнять одну и ту же функцию в охранных зонах
		/// </summary>
		void ValidateCodeForZones()
		{
			var guardDevices = new HashSet<Tuple<Guid, GKCodeReaderEnterType>>();
			foreach (var guardZone in GKManager.DeviceConfiguration.GuardZones)
			{
				foreach (var guard in guardZone.GuardZoneDevices.Where(x => x.Device.DriverType == GKDriverType.RSR2_CardReader || x.Device.DriverType == GKDriverType.RSR2_CodeReader))
				{
					ValidationGuardSettings(guard.CodeReaderSettings.AlarmSettings, guardDevices, guardZone, guard.Device.PresentationName);
					ValidationGuardSettings(guard.CodeReaderSettings.ChangeGuardSettings, guardDevices, guardZone, guard.Device.PresentationName);
					ValidationGuardSettings(guard.CodeReaderSettings.ResetGuardSettings, guardDevices, guardZone, guard.Device.PresentationName);
					ValidationGuardSettings(guard.CodeReaderSettings.SetGuardSettings, guardDevices, guardZone, guard.Device.PresentationName);
				}
			}
		}

		void ValidationGuardSettings(GKCodeReaderSettingsPart codeReaderSettingsPart, HashSet<Tuple<Guid, GKCodeReaderEnterType>> hashSet, GKGuardZone guardZone, string name)
		{
			if (codeReaderSettingsPart.CodeReaderEnterType != GKCodeReaderEnterType.None)
			{
				foreach (var codeUID in codeReaderSettingsPart.CodeUIDs)
				{
					if (!hashSet.Add(new Tuple<Guid, GKCodeReaderEnterType>(codeUID, codeReaderSettingsPart.CodeReaderEnterType)))
					{
						var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
						if (code != null)
							Errors.Add(new GuardZoneValidationError(guardZone, "Дублируется комманда " + codeReaderSettingsPart.CodeReaderEnterType.ToDescription() + " в устройстве " + name + " для кода " + code.PresentationName, ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}

		/// <summary>
		/// Охранная зона зависеть от объектов, присутствующие на одном и только на одном ГК
		/// </summary>
		/// <param name="code"></param>
		bool ValidateGuardZoneOnlyOnOneGK(GKGuardZone guardZone)
		{
			if (guardZone.GkParents.Count == 0)
			{
				Errors.Add(new GuardZoneValidationError(guardZone, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
				return false;
			}

			if (guardZone.GkParents.Count > 1)
			{
				Errors.Add(new GuardZoneValidationError(guardZone, "Охранная зона содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}

		/// <summary>
		/// Валидация того, что охранная зона содержит подключенные устройства
		/// </summary>
		/// <param name="guardZone"></param>
		void ValidateGuardZoneHasNoDevices(GKGuardZone guardZone)
		{
			if (guardZone.GuardZoneDevices.Count == 0)
			{
				Errors.Add(new GuardZoneValidationError(guardZone, "К зоне не подключено ни одного устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Валидация того, что охранная зоне не должна содержать более 100 кодов для каждого действия
		/// </summary>
		/// <param name="guardZone"></param>
		void ValidateGuardZoneCodesCount(GKGuardZone guardZone)
		{
			foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
			{
				if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
				{
					if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs.Count > 100)
						Errors.Add(new GuardZoneValidationError(guardZone, "Количество кодов для постановки у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite));
					if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs.Count > 100)
						Errors.Add(new GuardZoneValidationError(guardZone, "Количество кодов для снятия с охраны у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite));
					if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs.Count > 100)
						Errors.Add(new GuardZoneValidationError(guardZone, "Количество кодов для изменения состояни у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite));
					if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs.Count > 100)
						Errors.Add(new GuardZoneValidationError(guardZone, "Количество кодов на вызов тревоги у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		/// <summary>
		/// Для каждого устройства охранной зоны не должно быть дублирующихся методов ввода
		/// </summary>
		/// <param name="guardZone"></param>
		void ValidateGuardZoneSameCodeReaderEnterTypes(GKGuardZone guardZone)
		{
			foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
			{
				if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
				{
					var enterTypes = new HashSet<GKCodeReaderEnterType>();

					if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						if (!enterTypes.Add(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType))
							Errors.Add(new GuardZoneValidationError(guardZone, "Дублируется метод ввода у кодонаборника " + guardZoneDevice.Device.PredefinedName, ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}