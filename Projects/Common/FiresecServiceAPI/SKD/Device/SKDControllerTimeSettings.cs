using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDControllerTimeSettings
	{
		[DataMember]
		public bool IsEnabled { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public int UpdatePeriod { get; set; }

		[DataMember]
		public SKDTimeZoneType TimeZone { get; set; }
	}
}