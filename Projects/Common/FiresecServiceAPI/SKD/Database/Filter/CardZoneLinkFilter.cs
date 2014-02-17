using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CardZoneLinkFilter : FilterBase
	{
		[DataMember]
		public List<Guid> CardUids { get; set; }

		[DataMember]
		public List<Guid> ZoneUids { get; set; }

		[DataMember]
		public List<Guid> IntervalUids { get; set; }

		public CardZoneLinkFilter():base()
		{
			CardUids = new List<Guid>();
			ZoneUids = new List<Guid>();
			IntervalUids = new List<Guid>();
		}
	}
}