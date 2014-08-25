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
		public DateTime Date { get; set; }

		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public ShortEmployee ShortEmployee { get; set; }

		[DataMember]
		public List<TimeTrackPart> PlannedTimeTrackParts { get; set; }

		[DataMember]
		public List<TimeTrackPart> RealTimeTrackParts { get; set; }

		[DataMember]
		public List<TimeTrackPart> DocumentTrackParts { get; set; }

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

		[DataMember]
		public string Error { get; set; }

		public TimeTrackType TimeTrackType { get; set; }
		public string LetterCode { get; set; }
		public string Tooltip { get; set; }
		public List<TimeTrackTotal> Totals { get; set; }

		public void Calculate()
		{
			CalculateDocuments();

			PlannedTimeTrackParts = NormalizeTimeTrackParts(PlannedTimeTrackParts);
			RealTimeTrackParts = NormalizeTimeTrackParts(RealTimeTrackParts);

			CalculateCombinedTimeTrackParts();
			CalculateTotal();
			CalculateLetterCode();
		}

		void CalculateDocuments()
		{
			DocumentTrackParts = new List<TimeTrackPart>();
			foreach (var document in Documents)
			{
				document.TimeTrackDocumentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == document.DocumentCode);

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

		void CalculateCombinedTimeTrackParts()
		{
			var firstReal = new TimeSpan();
			var lastReal = new TimeSpan();
			var firstPlanned = new TimeSpan();
			var lastPlanned = new TimeSpan();

			if (RealTimeTrackParts.Count > 0)
			{
				firstReal = RealTimeTrackParts.FirstOrDefault().StartTime;
				lastReal = RealTimeTrackParts.LastOrDefault().EndTime;
			}
			if (PlannedTimeTrackParts.Count > 0)
			{
				firstPlanned = PlannedTimeTrackParts.FirstOrDefault().StartTime;
				lastPlanned = PlannedTimeTrackParts.LastOrDefault().EndTime;
			}

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
			combinedTimeSpans.Sort();

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
				var documentTimeTrack = DocumentTrackParts.FirstOrDefault(x => x.StartTime <= startTime && x.EndTime >= endTime);

				if (hasRealTimeTrack && hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackPartType = TimeTrackType.Presence;
				}
				if (!hasRealTimeTrack && !hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackPartType = TimeTrackType.None;
				}
				if (hasRealTimeTrack && !hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackPartType = TimeTrackType.Overtime;
					if (timeTrackPart.StartTime > firstPlanned && timeTrackPart.EndTime < lastPlanned)
						timeTrackPart.TimeTrackPartType = TimeTrackType.PresenceInBrerak;
				}
				if (!hasRealTimeTrack && hasPlannedTimeTrack)
				{
					timeTrackPart.TimeTrackPartType = TimeTrackType.Absence;

					if (RealTimeTrackParts.Any(x => x.StartTime <= startTime && x.EndTime >= endTime) && IsOnlyFirstEnter)
					{
						timeTrackPart.TimeTrackPartType = TimeTrackType.AbsenceInsidePlan;
					}

					if (timeTrackPart.StartTime == firstPlanned && timeTrackPart.EndTime < lastPlanned && !PlannedTimeTrackParts.Any(x => x.EndTime == timeTrackPart.EndTime))
					{
						if (timeTrackPart.Delta > AllowedLate)
						{
							timeTrackPart.TimeTrackPartType = TimeTrackType.Late;
						}
					}

					if (timeTrackPart.EndTime == lastPlanned && timeTrackPart.StartTime > firstPlanned && !PlannedTimeTrackParts.Any(x => x.StartTime == timeTrackPart.StartTime))
					{
						if (timeTrackPart.Delta > AllowedEarlyLeave)
						{
							timeTrackPart.TimeTrackPartType = TimeTrackType.EarlyLeave;
						}
					}
				}
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

		void CalculateTotal()
		{
			Totals = new List<TimeTrackTotal>();
			var totalBalance = new TimeTrackTotal(TimeTrackType.Balance);
			Totals.Add(totalBalance);
			Totals.Add(new TimeTrackTotal(TimeTrackType.Presence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Absence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.AbsenceInsidePlan));
			Totals.Add(new TimeTrackTotal(TimeTrackType.PresenceInBrerak));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Late));
			Totals.Add(new TimeTrackTotal(TimeTrackType.EarlyLeave));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Overtime));
			var totalNight = new TimeTrackTotal(TimeTrackType.Night);
			Totals.Add(totalNight);
			Totals.Add(new TimeTrackTotal(TimeTrackType.DayOff));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Holiday));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentOvertime));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentPresence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentAbsence));

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

				switch(timeTrack.TimeTrackPartType)
				{
					case SKD.TimeTrackType.DocumentOvertime:
					case SKD.TimeTrackType.DocumentPresence:
						totalBalance.TimeSpan += timeTrack.Delta;
						break;

					case SKD.TimeTrackType.Absence:
					case SKD.TimeTrackType.Late:
					case SKD.TimeTrackType.EarlyLeave:
					case SKD.TimeTrackType.DocumentAbsence:
						totalBalance.TimeSpan -= timeTrack.Delta;
						break;
				}
			}

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

			var totalEaveningTimeSpan = new TimeSpan();
			if (NightSettings != null)
			{
				totalEaveningTimeSpan = CalculateEveningTime(NightSettings.EveningStartTime, NightSettings.EveningEndTime);
				totalNight.TimeSpan = CalculateEveningTime(NightSettings.NightStartTime, NightSettings.NightEndTime);
			}

			TimeTrackType = CalculateTimeTrackType();
		}

		TimeTrackType CalculateTimeTrackType()
		{
			if (!string.IsNullOrEmpty(Error))
				return TimeTrackType.None;

			var longestTimeTrackType = TimeTrackType.Presence;
			var longestTimeSpan = new TimeSpan();
			foreach (var total in Totals)
			{
				switch(total.TimeTrackType)
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

		void CalculateLetterCode()
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
					LetterCode = "ПР";
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
	}
}