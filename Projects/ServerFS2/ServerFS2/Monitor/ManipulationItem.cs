using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2.Monitor
{
	public class ManipulationItem
	{
		Device Device {get; set;}
		List<byte> Bytes { get; set; }

		public ManipulationItem(Device device, List<byte> bytes)
		{
			Device = device;
			Bytes = bytes;
		}

		public void Manipulate()
		{ 
			;
		}
	}
}
