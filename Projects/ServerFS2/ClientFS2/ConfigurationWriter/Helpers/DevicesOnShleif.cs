using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class DevicesOnShleif
	{
		public DevicesOnShleif()
		{
			Devices = new List<Device>();
		}

		public int ShleifNo { get; set; }
		public List<Device> Devices { get; set; }
	}
}