using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	public class XDeviceConfigurationStates
	{
		public XDeviceConfigurationStates()
		{
			DeviceStates = new List<XDeviceState>();
			ZoneStates = new List<XZoneState>();
		}
		public List<XDeviceState> DeviceStates { get; set; }
		public List<XZoneState> ZoneStates { get; set; }
	}
}