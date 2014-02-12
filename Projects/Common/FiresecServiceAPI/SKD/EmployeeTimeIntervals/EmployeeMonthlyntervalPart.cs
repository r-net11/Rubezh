using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeMonthlyIntervalPart
	{
		public EmployeeMonthlyIntervalPart()
		{

		}

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public Guid TimeIntervalUID { get; set; }
	}
}