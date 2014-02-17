using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeWeeklyIntervalPart
	{
		public EmployeeWeeklyIntervalPart()
		{

		}

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public Guid TimeIntervalUID { get; set; }
	}
}