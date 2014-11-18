using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ServerTask
	{
		[DataMember]
		public string DeviceName { get; set; }

		[DataMember]
		public string DeviceAddress { get; set; }

		[DataMember]
		public string CardNumber { get; set; }

		[DataMember]
		public PendingCardAction PendingCardAction { get; set; }
	}
}