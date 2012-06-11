using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceConfigurationStates
	{
		public DeviceConfigurationStates()
		{
			DeviceStates = new List<DeviceState>();
			ZoneStates = new List<ZoneState>();
		}

		[DataMember]
		public List<DeviceState> DeviceStates { get; set; }

		[DataMember]
		public List<ZoneState> ZoneStates { get; set; }
	}
}