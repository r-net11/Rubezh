using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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