using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDeviceConfigurationStates
	{
		public XDeviceConfigurationStates()
		{
			DeviceStates = new List<XDeviceState>();
			ZoneStates = new List<XZoneState>();
		}

		[DataMember]
		public List<XDeviceState> DeviceStates { get; set; }

		[DataMember]
		public List<XZoneState> ZoneStates { get; set; }
	}
}