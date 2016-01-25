using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class DayIntervalPart : SKDModelBase
	{
		public DayIntervalPart()
		{
			BeginTime = new TimeSpan(0, 0, 0);
			EndTime = new TimeSpan(0, 0, 0);
		}
		public double BeginTimeTotalSeconds { private get; set; }
		public double EndTimeTotalSeconds { private get; set; }
		[DataMember]
		public Guid DayIntervalUID { get; set; }

		[DataMember]
		public TimeSpan BeginTime
		{
			get { return new TimeSpan(0, 0, 0, (int)BeginTimeTotalSeconds); }
			set { BeginTimeTotalSeconds = value.TotalSeconds; }
		}

		[DataMember]
		public TimeSpan EndTime
		{
			get { return new TimeSpan(0, 0, 0, (int)EndTimeTotalSeconds); }
			set { EndTimeTotalSeconds = value.TotalSeconds; }
		}

		[DataMember]
		public DayIntervalPartTransitionType TransitionType { get; set; }

		[DataMember]
		public int Number { get; set; }
	}
}