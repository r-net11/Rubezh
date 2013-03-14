using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class DevicesGroup
	{
		public DevicesGroup(DriverType driverType, string name)
		{
			DriverType = driverType;
			Name = name;
			Devices = new List<Device>();
		}

		public DriverType DriverType { get; set; }
		public string Name { get; set; }
		public List<Device> Devices { get; set; }
		public bool IsRemoteDevicesPointer { get; set; }

		public int Offset { get; set; }
		public int Lenght { get; set; }
		public int Count { get; set; }
	}
}