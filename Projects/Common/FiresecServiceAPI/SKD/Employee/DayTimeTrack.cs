using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class DayTimeTrack
	{
		public DayTimeTrack()
		{
			TimeTrackParts = new List<DayTimeTrackPart>();
			Intervals = new List<Interval>();
		}

		public DayTimeTrack(string error) : this()
		{
			Error = error;
		}

		[DataMember]
		public DateTime Date { get; set; }

		[DataMember]
		public List<DayTimeTrackPart> TimeTrackParts { get; set; }

		[DataMember]
		public List<Interval> Intervals { get; set; }

		[DataMember]
		public TimeSpan Total { get; set; }

		[DataMember]
		public TimeSpan TotalMiss { get; set; }

		[DataMember]
		public TimeSpan TotalInSchedule { get; set; }

		[DataMember]
		public TimeSpan TotalOutSchedule { get; set; }

		[DataMember]
		public string Error { get; set; }

		public void Calculate()
		{
			long totalIntervals = 0;
			foreach (var interval in Intervals)
			{
				var deltaTicks = interval.EndDate.Value.TimeOfDay.Ticks - interval.BeginDate.Value.TimeOfDay.Ticks;
				totalIntervals += deltaTicks;
			}
			TimeSpan timeSpan = new TimeSpan(totalIntervals);

			long totalTime = 0;
			foreach (var timeTrackPart in TimeTrackParts)
			{
				var deltaTrack = timeTrackPart.EndTime.TimeOfDay.Ticks - timeTrackPart.StartTime.TimeOfDay.Ticks;
				totalTime += deltaTrack;
			}
			Total = new TimeSpan(totalTime);
		}
	}

	[DataContract]
	public class DayTimeTrackPart
	{
		[DataMember]
		public DateTime StartTime { get; set; }

		[DataMember]
		public DateTime EndTime { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }
	}
}