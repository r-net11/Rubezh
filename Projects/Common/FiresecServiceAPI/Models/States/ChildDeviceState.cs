using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ChildDeviceState
	{
		public Device ChildDevice { get; set; }
		public DriverState DriverState { get; set; }
		public bool IsDeleting { get; set; }

		[DataMember]
		public Guid ChildDeviceId { get; set; }

		[DataMember]
		public string Code { get; set; }
	}
}