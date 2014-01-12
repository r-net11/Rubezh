using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDTimeInterval
	{
		public SKDTimeInterval()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public DateTime StartTime { get; set; }

		[DataMember]
		public DateTime EndTime { get; set; }
	}
}