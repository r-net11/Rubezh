using FiresecClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateLicense()
		{
			ValidateFire();
			ValidateSecurity();
			ValidateAccess();
		}

		void ValidateFire()
		{
			if (LicenseHelper.Fire)
				return;
			
			foreach (var pumpStation in GKManager.PumpStations)
				Errors.Add(new PumpStationValidationError(pumpStation, "Отсутствует лицензия модуля \"GLOBAL Пожаротушение\"", ValidationErrorLevel.CannotWrite));

			foreach(var mpt in GKManager.MPTs)
				Errors.Add(new MPTValidationError(mpt, "Отсутствует лицензия модуля \"GLOBAL Пожаротушение\"", ValidationErrorLevel.CannotWrite));
		}

		void ValidateSecurity()
		{
			if (LicenseHelper.Security)
				return;

			foreach(var guardZone in GKManager.GuardZones)
				Errors.Add(new GuardZoneValidationError(guardZone, "Отсутствует лицензия модуля \"GLOBAL Охрана\"", ValidationErrorLevel.CannotWrite));
		}

		void ValidateAccess()
		{
			if (LicenseHelper.Access)
				return;

			foreach(var skdZone in GKManager.SKDZones)
				Errors.Add(new SKDZoneValidationError(skdZone, "Отсутствует лицензия модуля \"GLOBAL Доступ\"", ValidationErrorLevel.CannotWrite));

			foreach(var door in GKManager.Doors)
				Errors.Add(new DoorValidationError(door, "Для работы с точками доступа нужна лицензия модуля \"GLOBAL Доступ\"", ValidationErrorLevel.Warning));
		}
	}
}