using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class DayTimeTrack
	{
		#region Constructors
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
		#endregion

		#region Properties

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
		#endregion

		/// <summary>
		/// Основной метод по вычислению всех интервалов
		/// </summary>
		public void Calculate()
		{
			CalculateDocuments();

			PlannedTimeTrackParts = NormalizeTimeTrackParts(PlannedTimeTrackParts);
			RealTimeTrackParts = NormalizeTimeTrackParts(RealTimeTrackParts);

			//CalculateCombinedTimeTrackParts();
			CombinedTimeTrackParts = CalculateCombinedTimeTrackParts(PlannedTimeTrackParts, RealTimeTrackParts,
																							DocumentTrackParts);
			//CalculateTotal();
			Totals = CalculateTotal();
			TimeTrackType = CalculateTimeTrackType(Totals, PlannedTimeTrackParts, IsHoliday, Error);
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

		/// <summary>
		/// Вычисление всех проходов
		/// </summary>
		private List<TimeTrackPart> CalculateCombinedTimeTrackParts(List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> realTimeTrackParts,																								List<TimeTrackPart> documentTimeTrackParts)
		{
			var plannedScheduleTime = new ScheduleInterval();

			if (plannedTimeTrackParts.Count > 0)
			{
				plannedScheduleTime.StartTime = plannedTimeTrackParts.FirstOrDefault().StartTime;
				plannedScheduleTime.EndTime = plannedTimeTrackParts.LastOrDefault().EndTime;
			}

			var combinedTimeSpans = GetCombinedTimeSpans();
			combinedTimeSpans.Sort();

			var combinedTimeTrackParts = new List<TimeTrackPart>();

			for (var i = 0; i < combinedTimeSpans.Count - 1; i++)
			{
				var combinedInterval = new ScheduleInterval(combinedTimeSpans[i], combinedTimeSpans[i + 1]);

				var timeTrackPart = new TimeTrackPart { StartTime = combinedInterval.StartTime, EndTime = combinedInterval.EndTime };
				combinedTimeTrackParts.Add(timeTrackPart);

				var hasRealTimeTrack = realTimeTrackParts.Any(x => x.StartTime <= combinedInterval.StartTime
																&& x.EndTime >= combinedInterval.EndTime);

				var hasPlannedTimeTrack = plannedTimeTrackParts.Any(x => x.StartTime <= combinedInterval.StartTime
																		&& x.EndTime >= combinedInterval.EndTime);

				var documentTimeTrack = documentTimeTrackParts.FirstOrDefault(x => x.StartTime <= combinedInterval.StartTime
																			&& x.EndTime >= combinedInterval.EndTime);

				timeTrackPart.TimeTrackPartType = GetTimeTrackType(timeTrackPart, hasRealTimeTrack, hasPlannedTimeTrack, plannedScheduleTime, combinedInterval);

				//Если на временной интервал есть документ
				if (documentTimeTrack != null)
				{
					timeTrackPart.TimeTrackPartType = GetDocumentTimeTrackType(documentTimeTrack, timeTrackPart, hasPlannedTimeTrack, hasRealTimeTrack);
				}
			}

			return combinedTimeTrackParts;
		}

		/// <summary>
		/// Получить тип интервала по документу
		/// </summary>
		/// <param name="documentTimeTrack"></param>
		/// <param name="timeTrackPart"></param>
		/// <param name="hasPlannedTimeTrack"></param>
		/// <param name="hasRealTimeTrack"></param>
		/// <returns></returns>
		private static TimeTrackType GetDocumentTimeTrackType(TimeTrackPart documentTimeTrack, TimeTrackPart timeTrackPart,
			bool hasPlannedTimeTrack, bool hasRealTimeTrack)
		{
			var documentType = documentTimeTrack.MinTimeTrackDocumentType.DocumentType;
			timeTrackPart.MinTimeTrackDocumentType = documentTimeTrack.MinTimeTrackDocumentType;

			switch (documentType)
			{
				case DocumentType.Overtime :
					return TimeTrackType.DocumentOvertime;
				case DocumentType.Presence:
					return hasPlannedTimeTrack ? TimeTrackType.DocumentPresence : TimeTrackType.None;
				case DocumentType.Absence:
					return (hasRealTimeTrack || hasPlannedTimeTrack) ? TimeTrackType.DocumentAbsence : TimeTrackType.None;
				default:
					return TimeTrackType.None;
			}
		}

		/// <summary>
		/// Получение типа интервала прохода
		/// </summary>
		/// <param name="timeTrackPart">Время, отслеженное в УРВ</param>
		/// <param name="hasRealTimeTrack">Флаг, показывающий, было ли появление сотрудника в данном интервале времени</param>
		/// <param name="hasPlannedTimeTrack">Флаг, показывающий, запланирован ли на данный интервал времени, график работы сотрудника</param>
		/// <param name="schedulePlannedInterval">Начальное и конечное время графика работы</param>
		/// <param name="combinedInterval">Начальное время и конечное время временного интервала в УРВ</param>
		/// <returns>Возвращает тип интервала прохода для расчета баланса</returns>
		private TimeTrackType GetTimeTrackType(TimeTrackPart timeTrackPart, bool hasRealTimeTrack, bool hasPlannedTimeTrack, ScheduleInterval schedulePlannedInterval, ScheduleInterval combinedInterval)
		{
			//Если есть интервал прохода сотрудника, который попадает в гафик работ, то "Явка"
			if (hasRealTimeTrack && hasPlannedTimeTrack)
			{
				return TimeTrackType.Presence;
			}

			//Если нет интервала проода сотрудника и нет графика, то "Нет данных"
			if (!hasRealTimeTrack && !hasPlannedTimeTrack)
			{
				return TimeTrackType.None;
			}

			//Если есть интервал прохода сотрудника, который не попадает в интервал графика работа, то "Сверхурочно"
			//или, если он попадает в график работ, то "Явка в перерыве"
			if (hasRealTimeTrack) //&& !hasPlannedTimeTrack)
			{
				var type = TimeTrackType.Overtime;
				if (timeTrackPart.StartTime > schedulePlannedInterval.StartTime &&
				    timeTrackPart.EndTime < schedulePlannedInterval.EndTime)
					type = TimeTrackType.PresenceInBrerak;
				return type;
			}

			//Если нет интервала прохода сотрудника, но есть интервал рабочего графика
			timeTrackPart.TimeTrackPartType = TimeTrackType.Absence; //Отсутствие

			//Если учитывается первый вход-последний выход и проход в рамках графика, то "Отсутствие в рамках графика"
			if (RealTimeTrackParts.Any(x => x.StartTime <= combinedInterval.StartTime && x.EndTime >= combinedInterval.EndTime) && IsOnlyFirstEnter)
			{
				return TimeTrackType.AbsenceInsidePlan;
			}

			if (PlannedTimeTrackParts.Any(x => x.StartTime == timeTrackPart.StartTime) &&
				PlannedTimeTrackParts.All(x => x.EndTime != timeTrackPart.EndTime) &&
				RealTimeTrackParts.Any(x => x.StartTime == timeTrackPart.EndTime))
			{
				var firstPlannedTimeTrack = PlannedTimeTrackParts.FirstOrDefault(x => x.StartTime == timeTrackPart.StartTime);
				if (firstPlannedTimeTrack.StartsInPreviousDay)
				{
					return TimeTrackType.Absence;
				}

				if (timeTrackPart.Delta > AllowedLate)
				{
					return TimeTrackType.Late;
				}
			}

			if (PlannedTimeTrackParts.Any(x => x.StartTime == timeTrackPart.StartTime) ||
			    PlannedTimeTrackParts.All(x => x.EndTime != timeTrackPart.EndTime) ||
			    RealTimeTrackParts.All(x => x.EndTime != timeTrackPart.StartTime)) return TimeTrackType.Absence;

			var lastPlannedTimeTrack = PlannedTimeTrackParts.FirstOrDefault(x => x.EndTime == timeTrackPart.EndTime);
			if (lastPlannedTimeTrack.EndsInNextDay)
			{
				return TimeTrackType.Absence;
			}

			return timeTrackPart.Delta > AllowedEarlyLeave ? TimeTrackType.EarlyLeave : TimeTrackType.Absence;
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

		/// <summary>
		/// Расчет времени и баланса для интервалов каждого типа
		/// </summary>
		/// <returns></returns>
		private List<TimeTrackTotal> CalculateTotal()
		{
			var resultTotalCollection = new List<TimeTrackTotal>();

			var totalBalance = GetBalanceForSlideTime(SlideTime, PlannedTimeTrackParts, RealTimeTrackParts);

			foreach (var timeTrack in CombinedTimeTrackParts)
			{
				var el = resultTotalCollection.FirstOrDefault(x => x.TimeTrackType == timeTrack.TimeTrackPartType);
				if (el != null) //TODO: Need refactoring
				{
					el.TimeSpan += GetDeltaForTimeTrack(timeTrack);
				}
				else
				{
					resultTotalCollection.Add(new TimeTrackTotal(timeTrack.TimeTrackPartType)
					{
						TimeSpan = GetDeltaForTimeTrack(timeTrack)
					});
				}

				totalBalance += GetBalance(timeTrack, SlideTime.TotalSeconds);
			}

			resultTotalCollection.Add(
				new TimeTrackTotal(TimeTrackType.Night)
				{
					TimeSpan = GetNightTime(NightSettings)
				});

			resultTotalCollection.Add(
				new TimeTrackTotal(TimeTrackType.Balance)
				{
					TimeSpan = totalBalance
				});

			return FillCollection(resultTotalCollection);
		}

		private List<TimeTrackTotal> FillCollection(List<TimeTrackTotal> inputCollectionTotals)
		{
			var resultCollection = new List<TimeTrackTotal>();
			var timeTrackTypes = new List<TimeTrackType>(Enum.GetValues(typeof(TimeTrackType)).Cast<TimeTrackType>().ToList());
			var collection = timeTrackTypes.Select(x => x).Where(x => !inputCollectionTotals.Select(timeTrack => timeTrack.TimeTrackType).Contains(x) && x != TimeTrackType.Holiday && x != TimeTrackType.DayOff && x != TimeTrackType.None);
			resultCollection.AddRange(collection.Select(x => new TimeTrackTotal(x)));
			resultCollection.AddRange(inputCollectionTotals);
			return resultCollection;
		}

		/// <summary>
		/// Получает часы, необходимые для расчета баланса, для каждого типа интервала времени
		/// </summary>
		/// <param name="timeTrack">Интервал УРВ</param>
		/// <returns>Время интервала</returns>
		private TimeSpan GetDeltaForTimeTrack(TimeTrackPart timeTrack)
		{
			if (!IsHoliday) return timeTrack.Delta;

			switch (timeTrack.TimeTrackPartType)
			{
				case TimeTrackType.Absence:
				case TimeTrackType.Late:
				case TimeTrackType.EarlyLeave:
				case TimeTrackType.DocumentAbsence:
					return default(TimeSpan);
				default:
					return timeTrack.Delta;
			}
		}

		/// <summary>
		/// Расчитывает баланс
		/// </summary>
		/// <param name="timeTrack">Временной интервал УРВ</param>
		/// <param name="slideTimeSecods">время скользящего графика в секундах</param>
		/// <returns>Возвращает баланс</returns>
		private TimeSpan GetBalance(TimeTrackPart timeTrack, double slideTimeSecods)
		{
			const double tolerance = 0.0001;

			switch (timeTrack.TimeTrackPartType)
			{
				case TimeTrackType.DocumentOvertime:
				case TimeTrackType.DocumentPresence:
				case TimeTrackType.Presence:
					return timeTrack.Delta;

				case TimeTrackType.Absence:
				case TimeTrackType.Late:
				case TimeTrackType.EarlyLeave:
					if (Math.Abs(slideTimeSecods) < tolerance)
					{
						return (-timeTrack.Delta);
					}
					break;

				case TimeTrackType.DocumentAbsence:
					return (-timeTrack.Delta);
			}

			return default(TimeSpan);
		}

		private TimeSpan GetBalanceForSlideTime(TimeSpan slideTimeTotalSeconds, List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> realTimeTrackParts)
		{
			if (slideTimeTotalSeconds.TotalSeconds <= 0) return default(TimeSpan);

			var balanceTimeSpan = new TimeSpan();

			if (plannedTimeTrackParts.Count > 0)
				balanceTimeSpan = -TimeSpan.FromSeconds(slideTimeTotalSeconds.TotalSeconds);

			foreach (var realTimeTrackPart in realTimeTrackParts)
			{
				balanceTimeSpan += realTimeTrackPart.Delta;
			}

			return balanceTimeSpan;
		}

		private TimeSpan GetNightTime(NightSettings nightSettings)
		{
			if (nightSettings == null || nightSettings.NightEndTime == nightSettings.NightStartTime) return default(TimeSpan);

			if (nightSettings.NightEndTime > nightSettings.NightStartTime)
			{
				return CalculateEveningTime(nightSettings.NightStartTime, nightSettings.NightEndTime, RealTimeTrackParts);
			}

			return CalculateEveningTime(nightSettings.NightStartTime, new TimeSpan(23, 59, 59), RealTimeTrackParts) +
				   CalculateEveningTime(new TimeSpan(0, 0, 0), nightSettings.NightEndTime, RealTimeTrackParts);
		}

		/// <summary>
		/// Вычисление типа временного интервала
		/// </summary>
		/// <param name="totals">Коллекция, хранящая время интервалов всех типов (за один день)</param>
		/// <param name="plannedTimeTrackParts">Время итервалов рабочего графика</param>
		/// <param name="isHoliday">Флаг выходного дня (праздника). true - выходной, false - нет</param>
		/// <param name="error">Текст ошибки</param>
		/// <returns>Возаращает тип интервала, для отображения в таблице УРВ</returns>
		private TimeTrackType CalculateTimeTrackType(List<TimeTrackTotal> totals, List<TimeTrackPart> plannedTimeTrackParts, bool isHoliday, string error)
		{
			if (!string.IsNullOrEmpty(error))
				return TimeTrackType.None;

			var longestTimeTrackType = TimeTrackType.Presence;
			var longestTimeSpan = new TimeSpan();
			foreach (var total in totals)
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

			if (longestTimeTrackType != TimeTrackType.Presence) return longestTimeTrackType;

			if (isHoliday)
				return TimeTrackType.Holiday;

			return plannedTimeTrackParts.Count == 0 ? TimeTrackType.DayOff : longestTimeTrackType;
		}


		private TimeSpan CalculateEveningTime(TimeSpan start, TimeSpan end, List<TimeTrackPart> realTimeTrackParts)
		{
			var result = new TimeSpan();

			if (end <= TimeSpan.Zero) return result;

			foreach (var trackPart in realTimeTrackParts)
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

		/// <summary>
		/// Получение буквенного кода для дня УРВ
		/// </summary>
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

		/// <summary>
		/// Структура для хранения начального и конечного времени интервалов
		/// </summary>
		private struct ScheduleInterval
		{
			public TimeSpan StartTime;
			public TimeSpan EndTime;

			public ScheduleInterval(TimeSpan startTime, TimeSpan endTime)
			{
				StartTime = startTime;
				EndTime = endTime;
			}
		}
	}
}