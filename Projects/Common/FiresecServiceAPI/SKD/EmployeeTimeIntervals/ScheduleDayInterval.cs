using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ScheduleDayInterval : SKDIsDeletedModel
	{
		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public Guid DayIntervalUID { get; set; }

		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }
	}
}