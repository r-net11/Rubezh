using System.Runtime.Serialization;
using System;

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