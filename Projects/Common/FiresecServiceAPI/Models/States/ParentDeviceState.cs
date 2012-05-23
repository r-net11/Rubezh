using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ParentDeviceState
	{
		public Device ParentDevice { get; set; }
		public DriverState DriverState { get; set; }
		public bool IsDeleting { get; set; }

		[DataMember]
		public Guid ParentDeviceId { get; set; }

		[DataMember]
		public string Code { get; set; }
	}
}