using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.Validation;
using System.Diagnostics;
using GKProcessor;
using System;

namespace GKModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public List<IValidationError> Validate()
		{
			DescriptorsManager.Create();
			Errors = new List<IValidationError>();
			ValidateGKObjectsCount();
			ValidateDevices();
			ValidateZones();
			ValidateDirections();
			ValidatePumpStations();
			ValidateMPTs();
			ValidateDelays();
			ValidateCodes();
			ValidateGuardZones();
			ValidateDoors();
			ValidateSKDZones();
			ValidatePlans();
			ValidateLicense();
			if (Errors.All(x => x.ErrorLevel != ValidationErrorLevel.CannotWrite))
				ValidateDescriptors();
			return Errors;
		}

		/// <summary>
		/// Проверка отсутствия ошибок при компиляции дескрипторов
		/// </summary>
		void ValidateDescriptors()
		{
			foreach(var descriptorError in DescriptorsManager.Check())
			{
				AddError(descriptorError.BaseDescriptor.GKBase.GkDatabaseParent, "Ошибка дескриптора " + descriptorError.BaseDescriptor.GKBase.PresentationName + ": " + descriptorError.Error, ValidationErrorLevel.CannotWrite);
			}
		}

		/// <summary>
		/// Проверка на уникальность идентификатора и номера
		/// Проверка на непустое название объекта
		/// </summary>
		/// <param name="gkBases"></param>
		void ValidateCommon(IEnumerable<GKBase> gkBases)
		{
			var uids = new HashSet<Guid>();
			var nos = new HashSet<int>();

			foreach (var gkBase in gkBases)
			{
				if (!uids.Add(gkBase.UID))
					AddError(gkBase, "Дублируется идентификатор", ValidationErrorLevel.CannotSave);

				if (!nos.Add(gkBase.No))
					AddError(gkBase, "Дублируется номер", ValidationErrorLevel.CannotWrite);

				if (string.IsNullOrWhiteSpace(gkBase.Name))
					AddError(gkBase, "Пустое название", ValidationErrorLevel.CannotWrite);

				ValidateObjectOnlyOnOneGK(gkBase);
			}
		}

		/// <summary>
		/// Объект должен зависеть от объектов, присутствующих на одном и только на одном ГК
		/// </summary>
		/// <param name="code"></param>
		bool ValidateObjectOnlyOnOneGK(GKBase gkBase)
		{
			if (gkBase is GKDevice && (gkBase as GKDevice).DriverType == GKDriverType.System)
				return true;
			if (gkBase.GkParents.Count == 0)
			{
				AddError(gkBase, "Не содержится ни в одном ГК", ValidationErrorLevel.CannotWrite);
				return false;
			}

			if (gkBase.GkParents.Count > 1)
			{
				AddError(gkBase, "Содержится в нескольких ГК", ValidationErrorLevel.CannotWrite);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Добавление ошибки
		/// </summary>
		/// <param name="gkBase"></param>
		/// <param name="error"></param>
		/// <param name="level"></param>
		void AddError(GKBase gkBase, string error, ValidationErrorLevel level)
		{
			if(gkBase is GKDevice)
			{
				Errors.Add(new DeviceValidationError(gkBase as GKDevice, error, level));
			}
			if (gkBase is GKZone)
			{
				Errors.Add(new ZoneValidationError(gkBase as GKZone, error, level));
			}
			if (gkBase is GKGuardZone)
			{
				Errors.Add(new GuardZoneValidationError(gkBase as GKGuardZone, error, level));
			}
			if (gkBase is GKDirection)
			{
				Errors.Add(new DirectionValidationError(gkBase as GKDirection, error, level));
			}
			if (gkBase is GKDelay)
			{
				Errors.Add(new DelayValidationError(gkBase as GKDelay, error, level));
			}
			if (gkBase is GKPumpStation)
			{
				Errors.Add(new PumpStationValidationError(gkBase as GKPumpStation, error, level));
			}
			if (gkBase is GKMPT)
			{
				Errors.Add(new MPTValidationError(gkBase as GKMPT, error, level));
			}
			if (gkBase is GKDoor)
			{
				Errors.Add(new DoorValidationError(gkBase as GKDoor, error, level));
			}
			if (gkBase is GKSKDZone)
			{
				Errors.Add(new SKDZoneValidationError(gkBase as GKSKDZone, error, level));
			}
			if (gkBase is GKCode)
			{
				Errors.Add(new CodeValidationError(gkBase as GKCode, error, level));
			}
		}
	}
}