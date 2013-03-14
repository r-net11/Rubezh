using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class DevicesGroup
	{
		public DevicesGroup(string name)
		{
			Name = name;
			Devices = new List<Device>();
		}

		public string Name { get; set; }
		public List<Device> Devices { get; set; }
		public bool IsRemoteDevicesPointer { get; set; }
	}
}