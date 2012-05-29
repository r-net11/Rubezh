using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceDriverState
	{
		public DriverState DriverState { get; set; }

		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public DateTime? Time { get; set; }
	}
}