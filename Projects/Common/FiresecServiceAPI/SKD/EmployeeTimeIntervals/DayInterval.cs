using System;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class DayInterval : SKDIsDeletedModel
	{
		public DayInterval()
		{

		}

		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public Guid NamedIntervalUID { get; set; }
	}
}