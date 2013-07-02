using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2.Helpers
{
	public static class ConfigurationCompareHelper
	{
		public static bool Compare(Device panelDevice, DeviceConfiguration remoteDeviceConfiguration)
		{
			var localDevices = panelDevice.GetRealChildren();
			var remoteDevices = remoteDeviceConfiguration.RootDevice.GetRealChildren();
			if (localDevices.Count != remoteDevices.Count)
				return false;

			foreach (var localDevice in localDevices)
			{
				var remoteDevice = remoteDevices.FirstOrDefault(x => x.IntAddress == x.IntAddress);
				if (remoteDevice == null)
					return false;
				if (localDevice.Zone != null)
				{
					if (remoteDevice.Zone == null || localDevice.Zone.PresentationName != remoteDevice.Zone.PresentationName)
						return false;
				}
				else
				{
					if (remoteDevice.Zone != null)
						return false;
				}
			}

			return true;
		}
	}
}