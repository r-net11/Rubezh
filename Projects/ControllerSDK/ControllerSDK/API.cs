using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControllerSDK
{
	public class DeviceSoftwareInfo
	{
		public DateTime SoftwareBuildDate { get; set; }
		public string DeviceType { get; set; }
		public string SoftwareVersion { get; set; }
	}

	public class DeviceNetInfo
	{
		public string IP { get; set; }
		public string SubnetMask { get; set; }
		public string DefaultGateway { get; set; }
		public Int32 MTU;
	}

	public class DeviceJournalItem
	{
		public DateTime DateTime { get; set; }
		public string OperatorName { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}