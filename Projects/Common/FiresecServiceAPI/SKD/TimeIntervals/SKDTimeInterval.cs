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
			UID = Guid.NewGuid();
			TimeIntervalParts = new List<SKDTimeIntervalPart>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public bool IsDefault { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<SKDTimeIntervalPart> TimeIntervalParts { get; set; }
	}
}