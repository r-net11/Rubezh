using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			XManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateDevices();
			ValidateZones();
			ValidateDirections();
			ValidatePumpStations();
			ValidateMPTs();
			ValidateGuard();
			return Errors;
		}

		bool IsManyGK()
		{
			return XManager.Devices.Where(x=>x.DriverType == XDriverType.GK).Count() > 1;
		}

		static bool AreDevicesInSameGK(List<XDevice> devices)
		{
			var gkDevices = new HashSet<XDevice>();
			foreach (var device in devices)
			{
				if (device.GKParent != null)
					gkDevices.Add(device.GKParent);
			}
			return (gkDevices.Count > 1);
		}
	}
}