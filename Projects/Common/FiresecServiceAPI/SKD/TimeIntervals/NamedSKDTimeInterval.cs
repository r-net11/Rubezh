using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class NamedSKDTimeInterval
	{
		public NamedSKDTimeInterval()
		{
			UID = Guid.NewGuid();
			TimeIntervals = new List<SKDTimeInterval>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<SKDTimeInterval> TimeIntervals { get; set; }
	}
}