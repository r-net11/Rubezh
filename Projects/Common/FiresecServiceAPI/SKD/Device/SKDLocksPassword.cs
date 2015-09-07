using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FiresecAPI.SKD.Device
{
	[DataContract]
	public class SKDLocksPassword
	{
		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public bool IsAppliedToLock1 { get; set; }

		[DataMember]
		public bool IsAppliedToLock2 { get; set; }

		[DataMember]
		public bool IsAppliedToLock3 { get; set; }

		[DataMember]
		public bool IsAppliedToLock4 { get; set; }
	}
}
