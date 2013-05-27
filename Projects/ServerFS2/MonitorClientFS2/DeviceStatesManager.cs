using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerFS2;

namespace MonitorClientFS2
{
	public class DeviceStatesManager
	{
		public void GetStates()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.Driver.IsPanel))
			{
				if (device.IntAddress != 15)
					continue;
			}
		}
	}
}