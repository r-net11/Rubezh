using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;
using System.Diagnostics;

namespace GKModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public List<IValidationError> Validate()
		{
			IsManyGK = GKManager.Devices.Where(x => x.DriverType == GKDriverType.GK).Count() > 1;
			GKManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateDevices();
			ValidatePumpStations();
			ValidateMPTs();
			ValidateDoors();
			ValidateSKDZones();
			ValidateDaySchedules();
			ValidateSchedules();
			ValidatePlans();
			return Errors;
		}

		bool IsManyGK { get; set; }

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