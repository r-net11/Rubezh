using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeMonthlyInterval
	{
		public EmployeeMonthlyInterval()
		{
			UID = Guid.NewGuid();
			MonthlyIntervalParts = new List<EmployeeMonthlyIntervalPart>();
			for (int i = 1; i <= 31; i++)
			{
				MonthlyIntervalParts.Add(new EmployeeMonthlyIntervalPart() { No = i });
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
		public List<EmployeeMonthlyIntervalPart> MonthlyIntervalParts { get; set; }
	}
}