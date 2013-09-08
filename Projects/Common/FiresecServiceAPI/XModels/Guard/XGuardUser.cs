using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XGuardUser
	{
		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public string FIO { get; set; }

		[DataMember]
		public string Function { get; set; }

		[DataMember]
		public bool CanSetZone { get; set; }

		[DataMember]
		public bool CanUnSetZone { get; set; }
	}
}