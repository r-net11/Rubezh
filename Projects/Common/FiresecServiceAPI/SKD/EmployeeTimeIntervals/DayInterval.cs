using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class DayInterval : OrganisationElementBase
	{
		public DayInterval()
		{
			DayIntervalParts = new List<DayIntervalPart>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public TimeSpan SlideTime { get; set; }

		[DataMember]
		public List<DayIntervalPart> DayIntervalParts { get; set; }
	}
}