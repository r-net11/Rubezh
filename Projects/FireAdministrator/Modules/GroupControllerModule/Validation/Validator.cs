using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
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
			IsManyGK = GKManager.Devices.Count(x => x.DriverType == GKDriverType.GK) > 1;
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
			ValidateTypesCorrectness();
			ValidateDescriptors();
			ValidateLicense();
			return Errors;
		}

		void ValidateTypesCorrectness()
		{
			foreach (var mpt in GKManager.MPTs)
			{
				foreach (var mptDevice in mpt.MPTDevices)
				{
					//mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard
				}
			}
		}

		bool IsManyGK { get; set; }

		void ValidateDescriptors()
		{
			foreach(var descriptorError in DescriptorsManager.Check())
			{
				Errors.Add(new DeviceValidationError(descriptorError.BaseDescriptor.GKBase.DataBaseParent, "Ошибка дескриптора " + descriptorError.BaseDescriptor.GKBase.PresentationName + ": " + descriptorError.Error, ValidationErrorLevel.CannotWrite));
			}
		}
	}
}