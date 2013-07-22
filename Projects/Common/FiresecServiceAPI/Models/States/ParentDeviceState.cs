using System.Runtime.Serialization;
using System;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ParentDeviceState
	{
		[DataMember]
		public Guid ParentDeviceUID { get; set; }

		[DataMember]
		public DriverState DriverState { get; set; }

		public Device ParentDevice { get; set; }
		public bool IsDeleting { get; set; }
	}
}