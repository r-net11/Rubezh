using System;

namespace ChinaSKDDriverAPI
{
	public class DeviceNetInfo
	{
		public string IP { get; set; }
		public string SubnetMask { get; set; }
		public string DefaultGateway { get; set; }
		public Int32 MTU;
	}
}