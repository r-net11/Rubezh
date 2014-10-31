using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public List<IValidationError> Validate()
		{
			GKManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateDevices();
			ValidateZones();
			ValidateDirections();
			ValidatePumpStations();
			ValidateMPTs();
			ValidateDelays();
			ValidateCodes();
			ValidateGuardZones();
			ValidateDoors();
			ValidateDaySchedules();
			ValidateSchedules();
            ValidatePlans();
			return Errors;
		}

		bool IsManyGK()
		{
			return GKManager.Devices.Where(x=>x.DriverType == GKDriverType.GK).Count() > 1;
		}

		static bool AreDevicesInSameGK(List<GKDevice> devices)
		{
			var gkDevices = new HashSet<GKDevice>();
			foreach (var device in devices)
			{
				if (device.GKParent != null)
					gkDevices.Add(device.GKParent);
			}
			return (gkDevices.Count > 1);
		}
	}
}