using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CardZoneFilter : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> CardUIDs { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> IntervalUIDs { get; set; }

		public CardZoneFilter():base()
		{
			CardUIDs = new List<Guid>();
			ZoneUIDs = new List<Guid>();
			IntervalUIDs = new List<Guid>();
		}
	}
}