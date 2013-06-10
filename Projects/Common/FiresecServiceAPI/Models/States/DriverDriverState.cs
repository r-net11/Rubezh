using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceDriverState
	{
		[DataMember]
		public DriverState DriverState { get; set; }

		[DataMember]
		public DateTime? Time { get; set; }
	}
}