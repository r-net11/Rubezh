using RubezhClient;
using Infrastructure.Common.Validation;
using RubezhAPI.License;
using RubezhAPI;
using Infrastructure.Common.License;

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
			if (LicenseManager.CurrentLicenseInfo.HasFirefighting)
				return;
			
			foreach (var pumpStation in GKManager.PumpStations)
				AddError(pumpStation, "Отсутствует лицензия модуля \"GLOBAL Пожаротушение\"", ValidationErrorLevel.CannotWrite);

			foreach(var mpt in GKManager.MPTs)
				AddError(mpt, "Отсутствует лицензия модуля \"GLOBAL Пожаротушение\"", ValidationErrorLevel.CannotWrite);
		}

		/// <summary>
		/// Валидация того, что при отсутствие лицензии на охрану конфигурация не должна содержать охранные зоны
		/// </summary>
		void ValidateGuard()
		{
			if (LicenseManager.CurrentLicenseInfo.HasGuard)
				return;

			foreach(var guardZone in GKManager.GuardZones)
				AddError(guardZone, "Отсутствует лицензия модуля \"GLOBAL Охрана\"", ValidationErrorLevel.CannotWrite);
		}

		/// <summary>
		/// Валидация того, что при отсутствие лицензии на СКД конфигурация не должна содержать ТД и зоны СКД
		/// </summary>
		void ValidateSKD()
		{
			if (LicenseManager.CurrentLicenseInfo.HasSKD)
				return;

			foreach(var skdZone in GKManager.SKDZones)
				AddError(skdZone, "Отсутствует лицензия модуля \"GLOBAL Доступ\"", ValidationErrorLevel.CannotWrite);

			foreach(var door in GKManager.Doors)
				AddError(door, "Отсутствует лицензия модуля \"GLOBAL Доступ\"", ValidationErrorLevel.CannotWrite);
		}
	}
}