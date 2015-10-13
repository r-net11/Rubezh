using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using System.Diagnostics;
using GKProcessor;

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
			ValidateDescriptors();
			ValidateLicense();
			return Errors;
		}

		/// <summary>
		/// Проверка отсутствия ошибок при компиляции дескрипторов
		/// </summary>
		void ValidateDescriptors()
		{
			foreach(var descriptorError in DescriptorsManager.Check())
			{
				Errors.Add(new DeviceValidationError(descriptorError.BaseDescriptor.GKBase.DataBaseParent, "Ошибка дескриптора " + descriptorError.BaseDescriptor.GKBase.PresentationName + ": " + descriptorError.Error, ValidationErrorLevel.CannotWrite));
			}
		}
	}
}