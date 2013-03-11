using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
    public static partial class Validator
	{
		static List<IValidationError> Errors { get; set; }

		public static IEnumerable<IValidationError> Validate()
		{
			XManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateDevices();
			ValidateZones();
			ValidateDirections();
			return Errors;
		}

        static bool IsManyGK()
        {
            return XManager.DeviceConfiguration.Devices.Where(x=>x.Driver.DriverType == XDriverType.GK).Count() > 1;
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