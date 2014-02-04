using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDControllerProperty
	{
		public SKDControllerProperty()
		{

		}

		[DataMember]
		public bool IsDUControl { get; set; }

		[DataMember]
		public bool IsEmployeeDU { get; set; }

		[DataMember]
		public bool IsEmployeeDUTime { get; set; }

		[DataMember]
		public bool IsEmployeeDUZone { get; set; }

		[DataMember]
		public bool IsGuestDU { get; set; }

		[DataMember]
		public bool IsGuestDUTime { get; set; }

		[DataMember]
		public bool IsGuestDUZone { get; set; }
	}
}