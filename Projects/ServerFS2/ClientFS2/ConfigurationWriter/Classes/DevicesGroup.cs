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
		public DevicesGroup(string name, int length = 0)
		{
			Name = name;
			Length = length;
			BinaryDevices = new List<BinaryDevice>();
		}

		public string Name { get; set; }
		public List<BinaryDevice> BinaryDevices { get; set; }
		public bool IsRemoteDevicesPointer { get; set; }
		public int Length { get; set; }
	}
}