using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Schedule : OrganisationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid ScheduleSchemeUid { get; set; }

		[DataMember]
		public List<Guid> ZoneLinkUids { get; set; }
	}
}