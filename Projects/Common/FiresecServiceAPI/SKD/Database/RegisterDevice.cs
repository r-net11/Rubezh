using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class RegisterDevice
	{
		public RegisterDevice()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public bool CanControl { get; set; }
	}
}