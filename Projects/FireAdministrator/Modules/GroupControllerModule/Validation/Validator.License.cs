using RubezhClient;
using Infrastructure.Common.Validation;
using FiresecLicense;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateLicense()
		{
			ValidateFirefighting();
			ValidateGuard();
			ValidateSKD();
		}

		/// <summary>
		/// Валидация того, что при отсутствие лицензии на пожаротушение конфигурация не должна содержать НС и МПТ
		/// </summary>
		void ValidateFirefighting()
		{
			if (FiresecLicenseManager.CurrentLicenseInfo.HasFirefighting)
				return;
			
			foreach (var pumpStation in GKManager.PumpStations)
				Errors.Add(new PumpStationValidationError(pumpStation, "Отсутствует лицензия модуля \"GLOBAL Пожаротушение\"", ValidationErrorLevel.CannotWrite));

			foreach(var mpt in GKManager.MPTs)
				Errors.Add(new MPTValidationError(mpt, "Отсутствует лицензия модуля \"GLOBAL Пожаротушение\"", ValidationErrorLevel.CannotWrite));
		}

		/// <summary>
		/// Валидация того, что при отсутствие лицензии на охрану конфигурация не должна содержать охранные зоны
		/// </summary>
		void ValidateGuard()
		{
			if (FiresecLicenseManager.CurrentLicenseInfo.HasGuard)
				return;

			foreach(var guardZone in GKManager.GuardZones)
				Errors.Add(new GuardZoneValidationError(guardZone, "Отсутствует лицензия модуля \"GLOBAL Охрана\"", ValidationErrorLevel.CannotWrite));
		}

		/// <summary>
		/// Валидация того, что при отсутствие лицензии на СКД конфигурация не должна содержать ТД и зоны СКД
		/// </summary>
		void ValidateSKD()
		{
			if (FiresecLicenseManager.CurrentLicenseInfo.HasSKD)
				return;

			foreach(var skdZone in GKManager.SKDZones)
				Errors.Add(new SKDZoneValidationError(skdZone, "Отсутствует лицензия модуля \"GLOBAL Доступ\"", ValidationErrorLevel.CannotWrite));

			foreach(var door in GKManager.Doors)
				Errors.Add(new DoorValidationError(door, "Для работы с точками доступа нужна лицензия модуля \"GLOBAL Доступ\"", ValidationErrorLevel.CannotWrite));
		}
	}
}