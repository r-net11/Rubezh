using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDTimeInterval
	{
		public SKDTimeInterval()
		{
			TimeIntervalParts = new List<SKDTimeIntervalPart>();
		}

		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<SKDTimeIntervalPart> TimeIntervalParts { get; set; }
	}
}