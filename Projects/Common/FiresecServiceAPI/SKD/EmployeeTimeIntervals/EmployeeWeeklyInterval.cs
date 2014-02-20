using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeWeeklyInterval : OrganizationElementBase
	{
		public EmployeeWeeklyInterval()
		{
			UID = Guid.NewGuid();
			WeeklyIntervalParts = new List<EmployeeWeeklyIntervalPart>();
			for (int i = 1; i <= 8; i++)
			{
				WeeklyIntervalParts.Add(new EmployeeWeeklyIntervalPart() { No = i });
			}
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool IsDefault { get; set; }

		[DataMember]
		public List<EmployeeWeeklyIntervalPart> WeeklyIntervalParts { get; set; }
	}
}