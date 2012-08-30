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
			DirectionStates = new List<XDirectionState>();
		}
		public List<XDeviceState> DeviceStates { get; set; }
		public List<XZoneState> ZoneStates { get; set; }
		public List<XDirectionState> DirectionStates { get; set; }
	}
}