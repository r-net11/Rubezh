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
			IsManyGK = GKManager.Devices.Count(x => x.DriverType == GKDriverType.GK) > 1;
			GKManager.UpdateConfiguration();
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

		static bool AreDevicesInSameGK(IEnumerable<GKDevice> devices)
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