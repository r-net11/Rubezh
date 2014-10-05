using System.Runtime.Serialization;
using System;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDWeeklyIntervalPart
	{
		[DataMember]
		public int No { get; set; }

		[DataMember]
		public Guid DayIntervalUID { get; set; }
	}
}