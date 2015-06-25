using System.Runtime.Serialization;
using System;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDoorWeeklyIntervalPart
	{
		[DataMember]
		public SKDDayOfWeek DayOfWeek { get; set; }

		[DataMember]
		public Guid DayIntervalUID { get; set; }
	}
}