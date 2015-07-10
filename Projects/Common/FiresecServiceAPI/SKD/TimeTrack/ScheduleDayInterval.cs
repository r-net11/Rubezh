using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ScheduleDayInterval : SKDModelBase
	{
		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public DayInterval DayInterval { get; set; }

		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }
	}
}