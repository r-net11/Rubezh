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
			DocumentTrackParts = new List<TimeTrackPart>();
			CombinedTimeTrackParts = new List<TimeTrackPart>();
			Documents = new List<TimeTrackDocument>();
			Totals = new List<TimeTrackTotal>();
		}

		public DayTimeTrack(string error)
			: this()
		{
			Error = error;
		}

		[DataMember]
		public string Error { get; set; }

		[DataMember]
		public DateTime Date { get; set; }

		/// <summary>
		/// Временные интервалы, установленные графиком
		/// </summary>
		[DataMember]
		public List<TimeTrackPart> PlannedTimeTrackParts { get; set; }

		/// <summary>
		/// Проходы сотрудника
		/// </summary>
		[DataMember]
		public List<TimeTrackPart> RealTimeTrackParts { get; set; }

		[DataMember]
		public List<TimeTrackPart> DocumentTrackParts { get; set; }

		/// <summary>
		/// Включает в себя интервалы графика работ, интервалы проходов сотрудника
		/// и интервалы, закрытые документами
		/// </summary>
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
		public List<TimeTrackDocument> Documents { get; set; }

		[DataMember]
		public NightSettings NightSettings { get; set; }

		public TimeTrackType TimeTrackType { get; set; }

		public string LetterCode { get; set; }

		public string Tooltip { get; set; }

		public List<TimeTrackTotal> Totals { get; set; }

		private bool IsCrossNight { get { return false; } }

		public void Calculate()
		{
			CalculateDocuments();

			PlannedTimeTrackParts = NormalizeTimeTrackParts(PlannedTimeTrackParts);
			RealTimeTrackParts = NormalizeTimeTrackParts(RealTimeTrackParts);

			CalculateCombinedTimeTrackParts();
			CalculateTotal();
			CalculateLetterCode();
		}

		private void CalculateDocuments()
		{
			DocumentTrackParts = new List<TimeTrackPart>();
			foreach (var document in Documents)
			{
				TimeTrackPart timeTrackPart = null;
				if (document.StartDateTime.Date < Date && document.EndDateTime.Date > Date)
				{
					timeTrackPart = new TimeTrackPart()
					{
						StartTime = TimeSpan.Zero,
						EndTime = new TimeSpan(23, 59, 59)
					};
				}
				if (document.StartDateTime.Date == Date && document.EndDateTime.Date > Date)
				{
					timeTrackPart = new TimeTrackPart()
					{
						StartTime = document.StartDateTime.TimeOfDay,
						EndTime = new TimeSpan(23, 59, 59)
					};
				}
				if (document.StartDateTime.Date == Date && document.EndDateTime.Date == Date)
				{
					timeTrackPart = new TimeTrackPart()
					{
						StartTime = document.StartDateTime.TimeOfDay,
						EndTime = document.EndDateTime.TimeOfDay
					};
				}
				if (document.StartDateTime.Date < Date && document.EndDateTime.Date == Date)
				{
					timeTrackPart = new TimeTrackPart()
					{
						StartTime = TimeSpan.Zero,
						EndTime = document.EndDateTime.TimeOfDay
					};
				}
				if (timeTrackPart != null)
				{
					timeTrackPart.MinTimeTrackDocumentType = document.TimeTrackDocumentType;
					DocumentTrackParts.Add(timeTrackPart);
				}
			}
			DocumentTrackParts = DocumentTrackParts.OrderBy(x => x.StartTime.Ticks).ToList();

			var timeSpans = new List<TimeSpan>();
			foreach (var trackPart in DocumentTrackParts)
			{
				timeSpans.Add(trackPart.StartTime);
				timeSpans.Add(trackPart.EndTime);
			}
			timeSpans = timeSpans.OrderBy(x => x.TotalSeconds).ToList();

			var result = new List<TimeTrackPart>();
			for (int i = 0; i < timeSpans.Count - 1; i++)
			{
				var startTimeSpan = timeSpans[i];
				var endTimeSpan = timeSpans[i + 1];
				var timeTrackParts = DocumentTrackParts.Where(x => x.StartTime <= startTimeSpan && x.EndTime > startTimeSpan);

				if (timeTrackParts.Count() > 0)
				{
					var newTimeTrackPart = new TimeTrackPart()
					{
						StartTime = startTimeSpan,
						EndTime = endTimeSpan,
					};
					foreach (var timeTrackPart in timeTrackParts)
					{
						if (newTimeTrackPart.MinTimeTrackDocumentType == null)
							newTimeTrackPart.MinTimeTrackDocumentType = timeTrackPart.MinTimeTrackDocumentType;
						else if (timeTrackPart.MinTimeTrackDocumentType.DocumentType < newTimeTrackPart.MinTimeTrackDocumentType.DocumentType)
							newTimeTrackPart.MinTimeTrackDocumentType = timeTrackPart.MinTimeTrackDocumentType;
						newTimeTrackPart.TimeTrackDocumentTypes.Add(timeTrackPart.MinTimeTrackDocumentType);
					}

					result.Add(newTimeTrackPart);
				}
			}
			DocumentTrackParts = result;
		}

		private List<TimeSpan> GetCombinedTimeSpans(List<TimeTrackPart> timeTrackParts)
		{
			var timeSpans = new List<TimeSpan>();

			foreach (var timeTrack in timeTrackParts)
			{
				timeSpans.Add(timeTrack.StartTime);
				timeSpans.Add(timeTrack.EndTime);
			}

			return timeSpans;
		}

		private void CalculateCombinedTimeTrackParts()
		{
			var plannedScheduleTime = new ScheduleInterval();
		//	var firstPlanned = new TimeSpan();
		//	var lastPlanned = new TimeSpan();

			if (PlannedTimeTrackParts.Count > 0)
			{
				plannedScheduleTime.StartTime = PlannedTimeTrackParts.FirstOrDefault().StartTime;
				plannedScheduleTime.EndTime = PlannedTimeTrackParts.LastOrDefault().EndTime;
			}

			var combinedTimeSpans = GetCombinedTimeSpans();
			combinedTimeSpans.Sort();

			CombinedTimeTrackParts = new List<TimeTrackPart>();
			for (int i = 0; i < combinedTimeSpans.Count - 1; i++)
			{
				ScheduleInterval combinedInterval = new ScheduleInterval(combinedTimeSpans[i], combinedTimeSpans[i + 1]);
			//	var startTime = combinedTimeSpans[i];
			//	var endTime = combinedTimeSpans[i + 1];

				var timeTrackPart = new TimeTrackPart { StartTime = combinedInterval.StartTime, EndTime = combinedInterval.EndTime };
				CombinedTimeTrackParts.Add(timeTrackPart);

				var hasRealTimeTrack = RealTimeTrackParts.Any(x => x.StartTime <= combinedInterval.StartTime && x.EndTime >= combinedInterval.EndTime);
				var hasPlannedTimeTrack = PlannedTimeTrackParts.Any(x => x.StartTime <= combinedInterval.StartTime && x.EndTime >= combinedInterval.EndTime);
				var documentTimeTrack = DocumentTrackParts.FirstOrDefault(x => x.StartTime <= combinedInterval.StartTime && x.EndTime >= combinedInterval.EndTime);

				timeTrackPart.TimeTrackPartType = GetTimeTrackType();

				//Если на временной интервал есть документ
				if (documentTimeTrack != null)
				{
					var documentType = documentTimeTrack.MinTimeTrackDocumentType.DocumentType;
					timeTrackPart.MinTimeTrackDocumentType = documentTimeTrack.MinTimeTrackDocumentType;
					if (documentType == DocumentType.Overtime)
					{
						timeTrackPart.TimeTrackPartType = TimeTrackType.DocumentOvertime;
					}
					if (documentType == DocumentType.Presence)
					{
						if (hasPlannedTimeTrack)
							timeTrackPart.TimeTrackPartType = TimeTrackType.DocumentPresence;
						else
							timeTrackPart.TimeTrackPartType = TimeTrackType.None;
					}
					if (documentType == DocumentType.Absence)
					{
						if (hasRealTimeTrack || hasPlannedTimeTrack)
							timeTrackPart.TimeTrackPartType = TimeTrackType.DocumentAbsence;
						else
							timeTrackPart.TimeTrackPartType = TimeTrackType.None;
					}
				}
			}
		}

		private TimeTrackType GetTimeTrackType(TimeTrackPart timeTrackPart, bool hasRealTimeTrack, bool hasPlannedTimeTrack, ScheduleInterval schedulePlannedInterval, ScheduleInterval combinedInterval)
		{
			//Если есть интервал прохода сотрудника, который попадает в гафик работ, то "Явка"
			if (hasRealTimeTrack && hasPlannedTimeTrack)
			{
				//timeTrackPart.TimeTrackPartType = TimeTrackType.Presence;
				return TimeTrackType.Presence;
			}

			//Если нет интервала проода сотрудника и нет графика, то "Нет данных"
			if (!hasRealTimeTrack && !hasPlannedTimeTrack)
			{
				//timeTrackPart.TimeTrackPartType = TimeTrackType.None;
				return TimeTrackType.None;
			}

			//Если есть интервал прохода сотрудника, который не попадает в интервал графика работа, то "Сверхурочно"
			//или, если он попадает в график работ, то "Явка в перерыве"
			if (hasRealTimeTrack) //&& !hasPlannedTimeTrack)
			{
				//timeTrackPart.TimeTrackPartType = TimeTrackType.Overtime;
				var type = TimeTrackType.Overtime;
				if (timeTrackPart.StartTime > schedulePlannedInterval.StartTime &&
				    timeTrackPart.EndTime < schedulePlannedInterval.EndTime)
					type = TimeTrackType.PresenceInBrerak;
				//timeTrackPart.TimeTrackPartType = TimeTrackType.PresenceInBrerak;
				return type;
			}

			//Если нет интервала прохода сотрудника, но есть интервал рабочего графика
			//if (!hasRealTimeTrack && hasPlannedTimeTrack)
			//{
				timeTrackPart.TimeTrackPartType = TimeTrackType.Absence; //Отсутствие

				//Если учитывается первый вход-последний выход и проход в рамках графика, то "Отсутствие в рамках графика"
				if (RealTimeTrackParts.Any(x => x.StartTime <= combinedInterval.StartTime && x.EndTime >= combinedInterval.EndTime) && IsOnlyFirstEnter)
				{
				//	timeTrackPart.TimeTrackPartType = TimeTrackType.AbsenceInsidePlan;
					return TimeTrackType.AbsenceInsidePlan;
				}


				if (PlannedTimeTrackParts.Any(x => x.StartTime == timeTrackPart.StartTime) &&
					PlannedTimeTrackParts.All(x => x.EndTime != timeTrackPart.EndTime) &&
					RealTimeTrackParts.Any(x => x.StartTime == timeTrackPart.EndTime))
				{
					var firstPlannedTimeTrack = PlannedTimeTrackParts.FirstOrDefault(x => x.StartTime == timeTrackPart.StartTime);
					if (firstPlannedTimeTrack.StartsInPreviousDay)
					{
						timeTrackPart.TimeTrackPartType = TimeTrackType.Absence;
					}
					else
					{
						if (timeTrackPart.Delta > AllowedLate)
						{
							timeTrackPart.TimeTrackPartType = TimeTrackType.Late;
						}
					}
				}

				if (PlannedTimeTrackParts.All(x => x.StartTime != timeTrackPart.StartTime) &&
					PlannedTimeTrackParts.Any(x => x.EndTime == timeTrackPart.EndTime) &&
					RealTimeTrackParts.Any(x => x.EndTime == timeTrackPart.StartTime))
				{
					var lastPlannedTimeTrack = PlannedTimeTrackParts.FirstOrDefault(x => x.EndTime == timeTrackPart.EndTime);
					if (lastPlannedTimeTrack.EndsInNextDay)
					{
						timeTrackPart.TimeTrackPartType = TimeTrackType.Absence;
					}
					else
					{
						if (timeTrackPart.Delta > AllowedEarlyLeave)
						{
							timeTrackPart.TimeTrackPartType = TimeTrackType.EarlyLeave;
						}
					}
				}
			//}
		}

		private List<TimeSpan> GetCombinedTimeSpans()
		{
			var combinedTimeSpans = new List<TimeSpan>();
			foreach (var trackPart in RealTimeTrackParts)
			{
				combinedTimeSpans.Add(trackPart.StartTime);
				combinedTimeSpans.Add(trackPart.EndTime);
			}
			foreach (var trackPart in PlannedTimeTrackParts)
			{
				combinedTimeSpans.Add(trackPart.StartTime);
				combinedTimeSpans.Add(trackPart.EndTime);
			}
			foreach (var trackPart in DocumentTrackParts)
			{
				combinedTimeSpans.Add(trackPart.StartTime);
				combinedTimeSpans.Add(trackPart.EndTime);
			}
			return combinedTimeSpans;
		}

		private void CalculateTotal()
		{
			Totals = new List<TimeTrackTotal>();

			var totalBalance = new TimeTrackTotal(TimeTrackType.Balance);
			var totalNight = new TimeTrackTotal(TimeTrackType.Night);
			Totals.Add(totalNight);
			Totals.Add(totalBalance);

			Totals.Add(new TimeTrackTotal(TimeTrackType.Presence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Absence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.AbsenceInsidePlan));
			Totals.Add(new TimeTrackTotal(TimeTrackType.PresenceInBrerak));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Late));
			Totals.Add(new TimeTrackTotal(TimeTrackType.EarlyLeave));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Overtime));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentOvertime));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentPresence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentAbsence));

			if (SlideTime.TotalSeconds > 0)
			{
				if (PlannedTimeTrackParts.Count > 0)
					totalBalance.TimeSpan = -TimeSpan.FromSeconds(SlideTime.TotalSeconds);
				else
					totalBalance.TimeSpan = new TimeSpan();
				foreach (var realTimeTrackPart in RealTimeTrackParts)
				{
					totalBalance.TimeSpan += realTimeTrackPart.Delta;
				}
			}

			foreach (var timeTrack in CombinedTimeTrackParts)
			{
				var timeTrackTotal = Totals.FirstOrDefault(x => x.TimeTrackType == timeTrack.TimeTrackPartType);
				if (timeTrackTotal != null)
				{
					if (IsHoliday)
					{
						switch (timeTrack.TimeTrackPartType)
						{
							case SKD.TimeTrackType.Absence:
							case SKD.TimeTrackType.Late:
							case SKD.TimeTrackType.EarlyLeave:
							case SKD.TimeTrackType.DocumentAbsence:
								continue;
						}
					}

					timeTrackTotal.TimeSpan += timeTrack.Delta;
				}

				switch (timeTrack.TimeTrackPartType)
				{
					case SKD.TimeTrackType.DocumentOvertime:
					case SKD.TimeTrackType.DocumentPresence:
						totalBalance.TimeSpan += timeTrack.Delta;
						break;

					case SKD.TimeTrackType.Absence:
					case SKD.TimeTrackType.Late:
					case SKD.TimeTrackType.EarlyLeave:
						if (SlideTime.TotalSeconds == 0)
						{
							totalBalance.TimeSpan -= timeTrack.Delta;
						}
						break;

					case SKD.TimeTrackType.DocumentAbsence:
						totalBalance.TimeSpan -= timeTrack.Delta;
						break;
				}
			}

			if (NightSettings != null && NightSettings.NightEndTime != NightSettings.NightStartTime)
			{
				if (NightSettings.NightEndTime > NightSettings.NightStartTime)
				{
					totalNight.TimeSpan = CalculateEveningTime(NightSettings.NightStartTime, NightSettings.NightEndTime);
				}
				else
				{
					totalNight.TimeSpan = CalculateEveningTime(NightSettings.NightStartTime, new TimeSpan(23, 59, 59)) + CalculateEveningTime(new TimeSpan(0, 0, 0), NightSettings.NightEndTime);
				}
			}

			TimeTrackType = CalculateTimeTrackType();
		}

		private TimeTrackType CalculateTimeTrackType()
		{
			if (!string.IsNullOrEmpty(Error))
				return TimeTrackType.None;

			var longestTimeTrackType = TimeTrackType.Presence;
			var longestTimeSpan = new TimeSpan();
			foreach (var total in Totals)
			{
				switch (total.TimeTrackType)
				{
					case TimeTrackType.Absence:
					case TimeTrackType.Late:
					case TimeTrackType.EarlyLeave:
					case TimeTrackType.Overtime:
					case TimeTrackType.Night:
					case TimeTrackType.DocumentOvertime:
					case TimeTrackType.DocumentPresence:
					case TimeTrackType.DocumentAbsence:
						if (total.TimeSpan > longestTimeSpan)
						{
							longestTimeTrackType = total.TimeTrackType;
							longestTimeSpan = total.TimeSpan;
						}
						break;
				}
			}
			if (longestTimeTrackType == TimeTrackType.Presence)
			{
				if (IsHoliday)
					return TimeTrackType.Holiday;

				if (PlannedTimeTrackParts.Count == 0)
					return TimeTrackType.DayOff;
			}
			return longestTimeTrackType;
		}

		private TimeSpan CalculateEveningTime(TimeSpan start, TimeSpan end)
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

		public static List<TimeTrackPart> NormalizeTimeTrackParts(List<TimeTrackPart> timeTrackParts)
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

			for (int i = 0; i < timeSpans.Count - 1; i++)
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
						TimeTrackPartType = timeTrackPart.TimeTrackPartType,
						ZoneUID = timeTrackPart.ZoneUID,
						StartsInPreviousDay = timeTrackPart.StartsInPreviousDay,
						EndsInNextDay = timeTrackPart.EndsInNextDay,
						DayName = timeTrackPart.DayName,
						PassJournalUID = timeTrackPart.PassJournalUID
					};
					result.Add(newTimeTrackPart);
				}
			}

			for (int i = result.Count - 1; i > 0; i--)
			{
				if (result[i].StartTime == result[i - 1].EndTime && result[i].ZoneUID == result[i - 1].ZoneUID)
				{
					result[i].StartTime = result[i - 1].StartTime;
					result.RemoveAt(i - 1);
				}
			}
			return result;
		}

		private void CalculateLetterCode()
		{
			Tooltip = TimeTrackType.ToDescription();

			switch (TimeTrackType)
			{
				case TimeTrackType.None:
					LetterCode = "";
					break;

				case TimeTrackType.Presence:
					LetterCode = "Я";
					break;

				case TimeTrackType.Absence:
					LetterCode = "НН";
					break;

				case TimeTrackType.Late:
					LetterCode = "ОП";
					break;

				case TimeTrackType.EarlyLeave:
					LetterCode = "УР";
					break;

				case TimeTrackType.Overtime:
					LetterCode = "С";
					break;

				case TimeTrackType.Night:
					LetterCode = "Н";
					break;

				case TimeTrackType.DayOff:
					LetterCode = "В";
					break;

				case TimeTrackType.Holiday:
					LetterCode = "В";
					break;

				case TimeTrackType.DocumentOvertime:
				case TimeTrackType.DocumentPresence:
				case TimeTrackType.DocumentAbsence:
					var tmieTrackPart = CombinedTimeTrackParts.FirstOrDefault(x => x.TimeTrackPartType == TimeTrackType);
					if (tmieTrackPart != null && tmieTrackPart.MinTimeTrackDocumentType != null)
					{
						LetterCode = tmieTrackPart.MinTimeTrackDocumentType.ShortName;
						Tooltip = tmieTrackPart.MinTimeTrackDocumentType.Name;
					}
					break;

				default:
					LetterCode = "";
					Tooltip = "";
					break;
			}
		}

		private class ScheduleInterval
		{
			public TimeSpan StartTime { get; set; }
			public TimeSpan EndTime { get; set; }

			public ScheduleInterval(TimeSpan startTime, TimeSpan endTime)
			{
				StartTime = startTime;
				EndTime = endTime;
			}

			public ScheduleInterval()
			{
			}
		}
	}
}