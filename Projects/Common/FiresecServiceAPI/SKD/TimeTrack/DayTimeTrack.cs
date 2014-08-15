using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

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

		public DayTimeTrack(string error)
			: this()
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
		public TimeSpan SlideTime { get; set; }

		[DataMember]
		public bool IsHoliday { get; set; }

		[DataMember]
		public int HolidayReduction { get; set; }

		[DataMember]
		public TimeTrackType TimeTrackType { get; set; }

		[DataMember]
		public TimeTrackDocument TimeTrackDocument { get; set; }

		[DataMember]
		public HolidaySettings HolidaySettings { get; set; }


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
		public TimeSpan TotalEavening { get; set; }

		[DataMember]
		public TimeSpan TotalNight { get; set; }

		[DataMember]
		public string Error { get; set; }

		public void Calculate()
		{
			PlannedTimeTrackParts = NormalizeTimeTrackParts(PlannedTimeTrackParts);
			RealTimeTrackParts = NormalizeTimeTrackParts(RealTimeTrackParts);

			CalculateCombinedTimeTrackParts();
			CalculateTotal();

			TotalEavening = new TimeSpan();
			TotalNight = new TimeSpan();
			if (HolidaySettings != null)
			{
				TotalEavening = CalculateEveningTime(HolidaySettings.EveningStartTime, HolidaySettings.EveningEndTime);
				TotalNight = CalculateEveningTime(HolidaySettings.NightStartTime, HolidaySettings.NightEndTime);
			}
		}

		void CalculateCombinedTimeTrackParts()
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
						if (timeTrackPart.Delta > AllowedLate)
						{
							timeTrackPart.TimeTrackPartType = TimeTrackPartType.Late;
						}
					}

					if (timeTrackPart.EndTime == LastPlanned && timeTrackPart.StartTime > FirstPlanned)
					{
						if (timeTrackPart.Delta > AllowedEarlyLeave)
						{
							timeTrackPart.TimeTrackPartType = TimeTrackPartType.EarlyLeave;
						}
					}
				}
			}
		}

		void CalculateTotal()
		{
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

			if (SlideTime.TotalSeconds > 0)
			{
				if (Total > SlideTime)
				{
					TotalInSchedule = SlideTime;
					TotalOutSchedule = Total - SlideTime;
				}
				else
				{
					TotalInSchedule = Total;
					TotalOutSchedule = new TimeSpan();
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

			if (TimeTrackDocument.DocumentCode != 0)
			{
				Total = TotalPlanned;
				TotalInSchedule = TotalPlanned;
				TotalMissed = new TimeSpan();
				TotalLate = new TimeSpan();
				TotalEarlyLeave = new TimeSpan();
				TotalOutSchedule = new TimeSpan();
			}
			TimeTrackType = CalculateTimeTrackType();
		}

		TimeTrackType CalculateTimeTrackType()
		{
			if (!string.IsNullOrEmpty(Error))
			{
				return TimeTrackType.None;
			}
			if (TimeTrackDocument.DocumentCode != 0)
			{
				return TimeTrackType.Document;
			}
			if (IsHoliday)
			{
				return TimeTrackType.Holiday;
			}
			if (PlannedTimeTrackParts.Count == 0)
			{
				return TimeTrackType.DayOff;
			}
			if (Total.TotalSeconds == 0)
			{
				return TimeTrackType.Missed;
			}
			if (TotalLate.TotalSeconds > 0)
			{
				return TimeTrackType.Late;
			}
			if (TotalEarlyLeave.TotalSeconds > 0)
			{
				return TimeTrackType.EarlyLeave;
			}
			if (TotalOutSchedule.TotalSeconds > 0)
			{
				return TimeTrackType.OutShedule;
			}
			return TimeTrackType.AsPlanned;
		}

		TimeSpan CalculateEveningTime(TimeSpan start, TimeSpan end)
		{
			var result = new TimeSpan();
			if (end > TimeSpan.Zero)
			{
				foreach (var trackPart in RealTimeTrackParts)
				{
					if (trackPart.StartTime <= start && trackPart.EndTime >= end)
					{
						result += end - start;
					}
					else
					{
						if ((trackPart.StartTime >= start && trackPart.StartTime <= end) ||
							(trackPart.EndTime >= start && trackPart.EndTime <= end))
						{
							var minStartTime = trackPart.StartTime < start ? start : trackPart.StartTime;
							var minEndTime = trackPart.EndTime > end ? end : trackPart.EndTime;
							result += minEndTime - minStartTime;
						}
					}
				}
			}
			return result;
		}

		List<TimeTrackPart> NormalizeTimeTrackParts(List<TimeTrackPart> timeTrackParts)
		{
			if (timeTrackParts.Count == 0)
				return new List<TimeTrackPart>();

			var result = new List<TimeTrackPart>();

			var timeSpans = new List<TimeSpan>();
			foreach (var timeTrackPart in timeTrackParts)
			{
				timeSpans.Add(timeTrackPart.StartTime);
				timeSpans.Add(timeTrackPart.EndTime);
			}
			timeSpans = timeSpans.OrderBy(x => x.TotalSeconds).ToList();

			for (int i = 0; i < timeSpans.Count - 1; i ++)
			{
				var startTimeSpan = timeSpans[i];
				var endTimeSpan = timeSpans[i + 1];
				var timeTrackPart = timeTrackParts.FirstOrDefault(x => x.StartTime <= startTimeSpan && x.EndTime > startTimeSpan);

				if (timeTrackPart != null)
				{
					var newTimeTrackPart = new TimeTrackPart()
					{
						StartTime = startTimeSpan,
						EndTime = endTimeSpan,
						ZoneUID = timeTrackPart.ZoneUID,
						TimeTrackPartType = timeTrackPart.TimeTrackPartType
					};
					result.Add(newTimeTrackPart);
				}
			}

			for (int i = result.Count - 1; i > 0; i--)
			{
				if (result[i].StartTime == result[i - 1].EndTime)
				{
					result[i].StartTime = result[i - 1].StartTime;
					result.RemoveAt(i - 1);
				}
			}
			return result;
		}
	}
}