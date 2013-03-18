using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecAPI.Models.Binary;

namespace ClientFS2.ConfigurationWriter
{
	public class DevicesGroup
	{
		public DevicesGroup(string name)
		{
			Name = name;
			BinaryDevices = new List<BinaryDevice>();
		}

		public string Name { get; set; }
		public List<BinaryDevice> BinaryDevices { get; set; }
		public bool IsRemoteDevicesPointer { get; set; }
	}
}