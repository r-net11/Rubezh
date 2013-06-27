using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2.Helpers
{
	public static class ConfigurationCompareHelper
	{
		public static bool Compare(Device panelDevice, List<Device> remoteDevices)
		{
			var localDevices = panelDevice.GetRealChildren();
			if (localDevices.Count != remoteDevices.Count)
				return false;
			return true;
		}
	}
}