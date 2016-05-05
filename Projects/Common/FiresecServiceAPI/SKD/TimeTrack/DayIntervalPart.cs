using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class DayIntervalPart : SKDModelBase
	{
		public DayIntervalPart()
		{
			BeginTime = new TimeSpan(0, 0, 0);
			EndTime = new TimeSpan(0, 0, 0);
		}

		[DataMember]
		public Guid DayIntervalUID { get; set; }

		[DataMember]
		public TimeSpan BeginTime { get; set; }

		[DataMember]
		public TimeSpan EndTime { get; set; }

		[DataMember]
		public DayIntervalPartTransitionType TransitionType { get; set; }
	}
}