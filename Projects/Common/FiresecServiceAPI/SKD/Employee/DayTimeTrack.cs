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
			TimeTrackParts = new List<DayTimeTrackPart>();
			Intervals = new List<Interval>();
			DayTrackDualIntervalParts = new List<DayTrackDualIntervalPart>();
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
		public List<DayTrackDualIntervalPart> DayTrackDualIntervalParts { get; set; }

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


			var timeSpans = new List<TimeSpan>();
			foreach (var timeTrackPart in TimeTrackParts)
			{
				timeSpans.Add(timeTrackPart.StartTime.TimeOfDay);
				timeSpans.Add(timeTrackPart.EndTime.TimeOfDay);
			}
			foreach (var interval in Intervals)
			{
				timeSpans.Add(interval.BeginDate.Value.TimeOfDay);
				timeSpans.Add(interval.EndDate.Value.TimeOfDay);
			}

			timeSpans.Sort();

			DayTrackDualIntervalParts = new List<DayTrackDualIntervalPart>();
			for (int i = 0; i < timeSpans.Count - 1; i++)
			{
				var startTime = timeSpans[i];
				var endTime = timeSpans[i + 1];

				var dayTrackDualIntervalPart = new DayTrackDualIntervalPart();
				dayTrackDualIntervalPart.StartTime = startTime;
				dayTrackDualIntervalPart.EndTime = endTime;
				DayTrackDualIntervalParts.Add(dayTrackDualIntervalPart);

				var hasTimeTrack = TimeTrackParts.Any(x => x.StartTime.TimeOfDay <= startTime && x.EndTime.TimeOfDay >= endTime);
				var hasInterval = Intervals.Any(x => x.BeginDate.Value.TimeOfDay <= startTime && x.EndDate.Value.TimeOfDay >= endTime);

				if (hasTimeTrack && hasInterval)
				{
					dayTrackDualIntervalPart.DayTrackDualIntervalPartType = DayTrackDualIntervalPartType.Both;
				}
				if (!hasTimeTrack && !hasInterval)
				{
					dayTrackDualIntervalPart.DayTrackDualIntervalPartType = DayTrackDualIntervalPartType.None;
				}
				if (hasTimeTrack && !hasInterval)
				{
					dayTrackDualIntervalPart.DayTrackDualIntervalPartType = DayTrackDualIntervalPartType.Real;
				}
				if (!hasTimeTrack && hasInterval)
				{
					dayTrackDualIntervalPart.DayTrackDualIntervalPartType = DayTrackDualIntervalPartType.Planed;
				}
			}

			if (totalTime > 0)
			{
				foreach (var dayTrackDualIntervalPart in DayTrackDualIntervalParts)
				{
					Trace.WriteLine(dayTrackDualIntervalPart.StartTime.ToString() + " - " + dayTrackDualIntervalPart.EndTime.ToString() + " - " + dayTrackDualIntervalPart.DayTrackDualIntervalPartType);
				}
			}
		}
	}

	public class DayTrackDualIntervalPart
	{
		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan EndTime { get; set; }

		[DataMember]
		public DayTrackDualIntervalPartType DayTrackDualIntervalPartType { get; set; }
	}

	public enum DayTrackDualIntervalPartType
	{
		[Description("Нет")]
		None,

		[Description("Пропуск")]
		Planed,

		[Description("Работа вне графика")]
		Real,

		[Description("Работа по графику")]
		Both
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