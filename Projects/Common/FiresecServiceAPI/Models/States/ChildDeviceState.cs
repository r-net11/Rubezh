using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ChildDeviceState
	{
		[DataMember]
		public Guid ChildDeviceUID { get; set; }

		[DataMember]
		public StateType StateType { get; set; }

		public Device ChildDevice { get; set; }
		public bool IsDeleting { get; set; }
	}
}