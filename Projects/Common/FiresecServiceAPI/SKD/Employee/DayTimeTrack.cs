using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class DayTimeTrack
	{
		public DayTimeTrack()
		{
			PlannedTimeTrackParts = new List<TimeTrackPart>();
			RealTimeTrackParts = new List<TimeTrackPart>();
			CombinedTimeTrackParts = new List<TimeTrackPart>();
		}

		public DayTimeTrack(string error) : this()
		{
			Error = error;
		}

		[DataMember]
		public DateTime Date { get; set; }

		[DataMember]
		public List<TimeTrackPart> PlannedTimeTrackParts { get; set; }

		[DataMember]
		public List<TimeTrackPart> RealTimeTrackParts { get; set; }

		[DataMember]
		public List<TimeTrackPart> CombinedTimeTrackParts { get; set; }

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
			//long totalIntervals = 0;
			//foreach (var timeTrackPart in PlannedTimeTrackParts)
			//{
			//    var deltaTicks = timeTrackPart.EndTime.Ticks - timeTrackPart.StartTime.Ticks;
			//    totalIntervals += deltaTicks;
			//}
			//TimeSpan timeSpan = new TimeSpan(totalIntervals);

			//long totalTime = 0;
			//foreach (var timeTrackPart in RealTimeTrackParts)
			//{
			//    var deltaTrack = timeTrackPart.EndTime.Ticks - timeTrackPart.StartTime.Ticks;
			//    totalTime += deltaTrack;
			//}
			//Total = new TimeSpan(totalTime);


			var timeSpans = new List<TimeSpan>();
			foreach (var timeTrackPart in RealTimeTrackParts)
			{
				timeSpans.Add(timeTrackPart.StartTime);
				timeSpans.Add(timeTrackPart.EndTime);
			}
			foreach (var interval in PlannedTimeTrackParts)
			{
				timeSpans.Add(interval.StartTime);
				timeSpans.Add(interval.EndTime);
			}
			timeSpans.Sort();

			CombinedTimeTrackParts = new List<TimeTrackPart>();
			for (int i = 0; i < timeSpans.Count - 1; i++)
			{
				var startTime = timeSpans[i];
				var endTime = timeSpans[i + 1];

				var timeTrackPart = new TimeTrackPart();
				timeTrackPart.StartTime = startTime;
				timeTrackPart.EndTime = endTime;
				CombinedTimeTrackParts.Add(timeTrackPart);

				var hasRealTimeTrack = RealTimeTrackParts.Any(x => x.StartTime <= startTime && x.EndTime >= endTime);
				var hasPlannedTimeTrack = PlannedTimeTrackParts.Any(x => x.StartTime <= startTime && x.EndTime >= endTime);

				if (hasRealTimeTrack && hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackType = TimeTrackType.AsPlanned;
				}
				if (!hasRealTimeTrack && !hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackType = TimeTrackType.None;
				}
				if (hasRealTimeTrack && !hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackType = TimeTrackType.RealOnly;
				}
				if (!hasRealTimeTrack && hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackType = TimeTrackType.PlanedOnly;
				}
			}

			var totalTimeSpan = new TimeSpan();
			foreach (var timeTrack in CombinedTimeTrackParts)
			{
				if (timeTrack.TimeTrackType == TimeTrackType.AsPlanned || timeTrack.TimeTrackType == TimeTrackType.PlanedOnly)
				{
					totalTimeSpan += timeTrack.EndTime - timeTrack.StartTime;
				}
			}
			Total = totalTimeSpan;
		}
	}

	public enum TimeTrackType
	{
		[Description("Нет")]
		None,

		[Description("Пропуск")]
		PlanedOnly,

		[Description("Работа вне графика")]
		RealOnly,

		[Description("Работа по графику")]
		AsPlanned
	}

	[DataContract]
	public class TimeTrackPart
	{
		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan EndTime { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public TimeTrackType TimeTrackType { get; set; }
	}
}