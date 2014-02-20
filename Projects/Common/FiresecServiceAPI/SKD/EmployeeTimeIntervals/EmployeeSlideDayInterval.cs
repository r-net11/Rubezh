using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeSlideDayInterval : OrganizationElementBase
	{
		public EmployeeSlideDayInterval()
		{
			UID = Guid.NewGuid();
			TimeIntervalUIDs = new List<Guid>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public bool IsDefault { get; set; }

		[DataMember]
		public List<Guid> TimeIntervalUIDs { get; set; }
	}
}