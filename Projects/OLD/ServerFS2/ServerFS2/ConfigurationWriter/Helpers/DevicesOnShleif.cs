using System.Collections.Generic;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
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