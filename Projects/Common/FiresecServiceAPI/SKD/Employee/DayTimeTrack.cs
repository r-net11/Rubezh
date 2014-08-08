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
		public bool IsIgnoreHoliday { get; set; }

		[DataMember]
		public bool IsOnlyFirstEnter { get; set; }

		[DataMember]
		public bool IsControl { get; set; }

		[DataMember]
		public bool IsHoliday { get; set; }

		[DataMember]
		public int HolidayReduction { get; set; }




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

					if (RealTimeTrackParts.Any(x => x.StartTime < startTime) && IsOnlyFirstEnter)
					{
						timeTrackPart.TimeTrackType = TimeTrackType.MissedButInsidePlan;
					}
				}
			}

			Total = new TimeSpan();
			TotalInSchedule = new TimeSpan();
			TotalMiss = new TimeSpan();
			TotalOutSchedule = new TimeSpan();
			foreach (var timeTrack in CombinedTimeTrackParts)
			{
				if (timeTrack.TimeTrackType == TimeTrackType.AsPlanned || timeTrack.TimeTrackType == TimeTrackType.RealOnly || timeTrack.TimeTrackType == TimeTrackType.MissedButInsidePlan)
				{
					Total += timeTrack.Delta;
				}
				if (timeTrack.TimeTrackType == TimeTrackType.AsPlanned || timeTrack.TimeTrackType == TimeTrackType.MissedButInsidePlan)
				{
					TotalInSchedule += timeTrack.Delta;
				}
				if (timeTrack.TimeTrackType == TimeTrackType.PlanedOnly)
				{
					TotalMiss += timeTrack.Delta;
				}
				if (timeTrack.TimeTrackType == TimeTrackType.RealOnly)
				{
					TotalOutSchedule += timeTrack.Delta;
				}
			}
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
		AsPlanned,

		[Description("В рамках графика с уходом")]
		MissedButInsidePlan,
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

		public TimeSpan Delta
		{
			get { return EndTime - StartTime; }
		}
	}
}