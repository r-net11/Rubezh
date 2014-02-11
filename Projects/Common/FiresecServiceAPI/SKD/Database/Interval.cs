using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Interval
	{
		public Interval()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public DateTime? BeginDate { get; set; }

		[DataMember]
		public DateTime? EndDate { get; set; }

		[DataMember]
		public Transition Transition { get; set; }
	}

	[DataContract]
	public enum Transition
	{
		Day,
		Night,
		DayNight
	}
}