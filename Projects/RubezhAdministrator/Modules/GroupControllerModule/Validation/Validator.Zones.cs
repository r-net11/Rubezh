using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateZones()
		{
			ValidateCommon(GKManager.Zones);

			foreach (var zone in GKManager.Zones)
			{
				if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.ZoneSensorQuantity))
					ValidateZoneDetectorCount(zone);
					ValidateZoneFire1Fire2Count(zone);
			}
		}

		/// <summary>
		/// Валидация того, что количество подключенных к зоне датчиков для перехода в соответствующее состояние меньше заданного в настройке зоны
		/// </summary>
		/// <param name="zone"></param>
		void ValidateZoneDetectorCount(GKZone zone)
		{
			var fire1Count = zone.Devices.Count(x => x.Driver.AvailableStateBits.Contains(GKStateBit.Fire1));
			var fire2Count = zone.Devices.Count(x => x.Driver.AvailableStateBits.Contains(GKStateBit.Fire2));
			if (fire2Count == 0 && fire1Count < zone.Fire1Count)
			{
				AddError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 1", ValidationErrorLevel.CannotWrite);
				return;
			}
			if (fire2Count == 0 && fire1Count < zone.Fire2Count)
			{
				AddError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.Warning);
			}
		}

		/// <summary>
		/// Валидация того, что настройка количество датчиков для перехода в Пожар-1 больше, чем для перехода в Пожар-2
		/// </summary>
		/// <param name="zone"></param>
		void ValidateZoneFire1Fire2Count(GKZone zone)
		{
			if (zone.Fire1Count >= zone.Fire2Count)
			{
				AddError(zone, "Количество датчиков для сработки Пожар 1 должно быть меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.CannotWrite);
			}
		}
	}
}