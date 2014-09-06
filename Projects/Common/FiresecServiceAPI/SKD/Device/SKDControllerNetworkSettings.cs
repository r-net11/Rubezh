using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDControllerNetworkSettings
	{
		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public string Mask { get; set; }

		[DataMember]
		public string DefaultGateway { get; set; }

		[DataMember]
		public int MTU { get; set; }
	}
}