using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeTimeInterval : OrganizationElementBase
	{
		public EmployeeTimeInterval()
		{
			UID = Guid.NewGuid();
			TimeIntervalParts = new List<EmployeeTimeIntervalPart>();
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
		public DateTime ConstantSlideTime { get; set; }

		[DataMember]
		public List<EmployeeTimeIntervalPart> TimeIntervalParts { get; set; }
	}
}