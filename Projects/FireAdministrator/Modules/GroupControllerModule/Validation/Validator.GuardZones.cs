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
			ValidateCommon(GKManager.GuardZones);
			//ValidateGuardZoneSameDevices();
			ValidateCodeForZones();

			foreach (var guardZone in GKManager.GuardZones)
			{
				ValidateGuardZoneHasNoDevices(guardZone);
				ValidateGuardZoneCodesCount(guardZone);
				ValidateGuardZoneSameCodeReaderEnterTypes(guardZone);
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
					if (guardZoneDevice.Device.Driver.IsCardReaderOrCodeReader)
					{
						if (!deviceUIDs.Add(guardZoneDevice.Device.UID))
							AddError(guardZone, "Устройство " + guardZoneDevice.Device.PresentationName + " уже участвует в другой охранной зоне", ValidationErrorLevel.CannotWrite);
					}
				}
			}
		}

		/// <summary>
		/// Валидация того, что один и тот же код может не может выполнять одну и ту же функцию в охранных зонах
		/// </summary>
		void ValidateCodeForZones()
		{
			foreach (var guardZone in GKManager.DeviceConfiguration.GuardZones)
			{
				var guardDevices = new HashSet<Tuple<Guid, GKCodeReaderEnterType>>();
				foreach (var device in guardZone.GuardZoneDevices.Where(x => x.Device.Driver.IsCardReaderOrCodeReader))
				{
					ValidationGuardSettings(device.CodeReaderSettings.AlarmSettings, guardDevices, guardZone, device.Device.PresentationName);
					ValidationGuardSettings(device.CodeReaderSettings.ChangeGuardSettings, guardDevices, guardZone, device.Device.PresentationName);
					ValidationGuardSettings(device.CodeReaderSettings.ResetGuardSettings, guardDevices, guardZone, device.Device.PresentationName);
					ValidationGuardSettings(device.CodeReaderSettings.SetGuardSettings, guardDevices, guardZone, device.Device.PresentationName);
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
							AddError(guardZone, "Дублируется комманда " + codeReaderSettingsPart.CodeReaderEnterType.ToDescription() + " в устройстве " + name + " для кода " + code.PresentationName, ValidationErrorLevel.CannotWrite);
					}
				}
			}
		}

		/// <summary>
		/// Валидация того, что охранная зона содержит подключенные устройства
		/// </summary>
		/// <param name="guardZone"></param>
		void ValidateGuardZoneHasNoDevices(GKGuardZone guardZone)
		{
			if (guardZone.GuardZoneDevices.Count == 0)
			{
				AddError(guardZone, "К зоне не подключено ни одного устройства", ValidationErrorLevel.CannotWrite);
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
				if (guardZoneDevice.Device.Driver.IsCardReaderOrCodeReader)
				{
					if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs.Count > 100)
						AddError(guardZone, "Количество кодов для постановки у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite);
					if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs.Count > 100)
						AddError(guardZone, "Количество кодов для снятия с охраны у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite);
					if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs.Count > 100)
						AddError(guardZone, "Количество кодов для изменения состояни у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite);
					if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs.Count > 100)
						AddError(guardZone, "Количество кодов на вызов тревоги у кодонаборника " + guardZoneDevice.Device.PredefinedName + " не должно превышать 100", ValidationErrorLevel.CannotWrite);
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
				if (guardZoneDevice.Device.Driver.IsCardReaderOrCodeReader)
				{
					var enterTypes = new HashSet<GKCodeReaderEnterType>();

					if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						if (!enterTypes.Add(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType))
							AddError(guardZone, "Дублируется метод ввода у кодонаборника " + guardZoneDevice.Device.PredefinedName, ValidationErrorLevel.CannotWrite);
				}
			}
		}
	}
}