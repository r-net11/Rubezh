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
		public Guid EmployeeUID { get; set; }

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
		public TimeSpan AllowedLate { get; set; }

		[DataMember]
		public TimeSpan AllowedEarlyLeave { get; set; }

		[DataMember]
		public bool IsHoliday { get; set; }

		[DataMember]
		public int HolidayReduction { get; set; }

		[DataMember]
		public TimeTrackType TimeTrackType { get; set; }

		[DataMember]
		public TimeTrackDocument TimeTrackDocument { get; set; }


		[DataMember]
		public TimeSpan FirstReal { get; set; }

		[DataMember]
		public TimeSpan LastReal { get; set; }

		[DataMember]
		public TimeSpan FirstPlanned { get; set; }

		[DataMember]
		public TimeSpan LastPlanned { get; set; }



		[DataMember]
		public TimeSpan Total { get; set; }

		[DataMember]
		public TimeSpan TotalMissed { get; set; }

		[DataMember]
		public TimeSpan TotalInSchedule { get; set; }

		[DataMember]
		public TimeSpan TotalOutSchedule { get; set; }

		[DataMember]
		public TimeSpan TotalLate { get; set; }

		[DataMember]
		public TimeSpan TotalEarlyLeave { get; set; }

		[DataMember]
		public TimeSpan TotalPlanned { get; set; }

		[DataMember]
		public string Error { get; set; }

		public void Calculate()
		{
			if (RealTimeTrackParts.Count > 0)
			{
				FirstReal = RealTimeTrackParts.FirstOrDefault().StartTime;
				LastReal = RealTimeTrackParts.LastOrDefault().EndTime;
			}
			if (PlannedTimeTrackParts.Count > 0)
			{
				FirstPlanned = PlannedTimeTrackParts.FirstOrDefault().StartTime;
				LastPlanned = PlannedTimeTrackParts.LastOrDefault().EndTime;
			}

			var combinedTimeSpans = new List<TimeSpan>();
			foreach (var timeTrackPart in RealTimeTrackParts)
			{
				combinedTimeSpans.Add(timeTrackPart.StartTime);
				combinedTimeSpans.Add(timeTrackPart.EndTime);
			}
			foreach (var interval in PlannedTimeTrackParts)
			{
				combinedTimeSpans.Add(interval.StartTime);
				combinedTimeSpans.Add(interval.EndTime);
			}
			combinedTimeSpans.Sort();

			TotalPlanned = new TimeSpan();
			foreach (var timeTrackPart in RealTimeTrackParts)
			{
				TotalPlanned += timeTrackPart.Delta;
			}

			CombinedTimeTrackParts = new List<TimeTrackPart>();
			for (int i = 0; i < combinedTimeSpans.Count - 1; i++)
			{
				var startTime = combinedTimeSpans[i];
				var endTime = combinedTimeSpans[i + 1];

				var timeTrackPart = new TimeTrackPart();
				timeTrackPart.StartTime = startTime;
				timeTrackPart.EndTime = endTime;
				CombinedTimeTrackParts.Add(timeTrackPart);

				var hasRealTimeTrack = RealTimeTrackParts.Any(x => x.StartTime <= startTime && x.EndTime >= endTime);
				var hasPlannedTimeTrack = PlannedTimeTrackParts.Any(x => x.StartTime <= startTime && x.EndTime >= endTime);

				if (hasRealTimeTrack && hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackPartType = TimeTrackPartType.AsPlanned;
				}
				if (!hasRealTimeTrack && !hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackPartType = TimeTrackPartType.None;
				}
				if (hasRealTimeTrack && !hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackPartType = TimeTrackPartType.RealOnly;
					if (timeTrackPart.StartTime > FirstPlanned && timeTrackPart.EndTime < LastPlanned)
						timeTrackPart.TimeTrackPartType = TimeTrackPartType.InBrerak;
				}
				if (!hasRealTimeTrack && hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackPartType = TimeTrackPartType.PlanedOnly;

					if (RealTimeTrackParts.Any(x => x.StartTime < startTime) && IsOnlyFirstEnter)
					{
						timeTrackPart.TimeTrackPartType = TimeTrackPartType.MissedButInsidePlan;
					}

					if (timeTrackPart.StartTime == FirstPlanned && timeTrackPart.EndTime < LastPlanned)
					{
						timeTrackPart.TimeTrackPartType = TimeTrackPartType.Late;
					}

					if (timeTrackPart.EndTime == LastPlanned && timeTrackPart.StartTime > FirstPlanned)
					{
						timeTrackPart.TimeTrackPartType = TimeTrackPartType.EarlyLeave;
					}
				}
			}

			Total = new TimeSpan();
			TotalInSchedule = new TimeSpan();
			TotalMissed = new TimeSpan();
			TotalLate = new TimeSpan();
			TotalEarlyLeave = new TimeSpan();
			TotalOutSchedule = new TimeSpan();

			foreach (var timeTrack in CombinedTimeTrackParts)
			{
				if (timeTrack.TimeTrackPartType == TimeTrackPartType.AsPlanned || timeTrack.TimeTrackPartType == TimeTrackPartType.RealOnly || timeTrack.TimeTrackPartType == TimeTrackPartType.MissedButInsidePlan)
				{
					Total += timeTrack.Delta;
				}
				if (timeTrack.TimeTrackPartType == TimeTrackPartType.AsPlanned || timeTrack.TimeTrackPartType == TimeTrackPartType.MissedButInsidePlan)
				{
					TotalInSchedule += timeTrack.Delta;
				}
				if (timeTrack.TimeTrackPartType == TimeTrackPartType.PlanedOnly)
				{
					TotalMissed = timeTrack.Delta;
				}
				if (timeTrack.TimeTrackPartType == TimeTrackPartType.Late)
				{
					TotalLate = timeTrack.Delta;
				}
				if (timeTrack.TimeTrackPartType == TimeTrackPartType.EarlyLeave)
				{
					TotalEarlyLeave = timeTrack.Delta;
				}
				if (timeTrack.TimeTrackPartType == TimeTrackPartType.RealOnly)
				{
					TotalOutSchedule += timeTrack.Delta;
				}
			}

			if (IsHoliday)
			{
				TotalInSchedule = new TimeSpan();
				TotalMissed = new TimeSpan();
				TotalLate = new TimeSpan();
				TotalEarlyLeave = new TimeSpan();
				TotalOutSchedule = new TimeSpan();
			}

			if (!string.IsNullOrEmpty(Error))
			{
				TimeTrackType = TimeTrackType.None;
				return;
			}
			if (TimeTrackDocument.DocumentCode != 0)
			{
				TimeTrackType = TimeTrackType.Document;
				Total = TotalPlanned;
				TotalInSchedule = TotalPlanned;
				TotalMissed = new TimeSpan();
				TotalLate = new TimeSpan();
				TotalEarlyLeave = new TimeSpan();
				TotalOutSchedule = new TimeSpan();
				return;
			}
			if (IsHoliday)
			{
				TimeTrackType = TimeTrackType.Holiday;
				return;
			}
			if (PlannedTimeTrackParts.Count == 0)
			{
				TimeTrackType = TimeTrackType.DayOff;
				return;
			}
			if (Total.TotalSeconds == 0)
			{
				TimeTrackType = TimeTrackType.Missed;
				return;
			}
			if (TotalLate.TotalSeconds > 0)
			{
				TimeTrackType = TimeTrackType.Late;
				return;
			}
			if (TotalEarlyLeave.TotalSeconds > 0)
			{
				TimeTrackType = TimeTrackType.EarlyLeave;
				return;
			}
			if (TotalOutSchedule.TotalSeconds > 0)
			{
				TimeTrackType = TimeTrackType.OutShedule;
				return;
			}
			TimeTrackType = TimeTrackType.AsPlanned;
		}
	}
}