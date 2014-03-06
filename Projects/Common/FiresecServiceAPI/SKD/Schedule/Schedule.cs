using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Schedule : OrganizationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid ScheduleSchemeUid { get; set; }

		[DataMember]
		public List<Guid> ZoneLinkUids { get; set; }
	}
}