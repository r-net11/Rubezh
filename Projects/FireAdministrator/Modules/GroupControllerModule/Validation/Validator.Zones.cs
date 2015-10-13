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
			ValidateZoneNoEquality();

			foreach (var zone in GKManager.Zones)
			{
				if (ValidateCodeStationOnlyOnOneGK(zone))
				{
					//ValidateZoneHasNoDevices(zone);
					if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.HasFlag(ValidationErrorType.ZoneSensorQuantity))
					{
						ValidateZoneDetectorCount(zone);
						ValidateZoneFire1Fire2Count(zone);
					}
				}
			}
		}

		/// <summary>
		/// Валидация уникальности номеров зон
		/// </summary>
		void ValidateZoneNoEquality()
		{
			var nos = new HashSet<int>();
			foreach (var zone in GKManager.Zones)
			{
				if (!nos.Add(zone.No))
					Errors.Add(new ZoneValidationError(zone, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Зона должна зависеть от объектов, присутствующих на одном и только на одном ГК
		/// </summary>
		/// <param name="code"></param>
		bool ValidateCodeStationOnlyOnOneGK(GKZone zone)
		{
			if (zone.GkParents.Count == 0)
			{
				Errors.Add(new ZoneValidationError(zone, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
				return false;
			}

			if (zone.GkParents.Count > 1)
			{
				Errors.Add(new ZoneValidationError(zone, "Зона содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
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
				Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 1", ValidationErrorLevel.CannotWrite));
				return;
			}
			if (fire2Count == 0 && fire1Count < zone.Fire2Count)
			{
				Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.CannotWrite));
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
				Errors.Add(new ZoneValidationError(zone, "Количество датчиков для сработки Пожар 1 должно быть меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}