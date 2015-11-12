using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
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
			CrossNightTimeTrackParts = new List<TimeTrackPart>();
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
		public int AllowedLate { get; set; }

		[DataMember]
		public int AllowedEarlyLeave { get; set; }

		[DataMember]
		public int NotAllowOvertimeLowerThan { get; set; }

		/// <summary>
		/// Разрешить отсутствие меньше чем
		/// </summary>
		[DataMember]
		public int AllowedAbsentLowThan { get; set; }

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

		/// <summary>
		/// Интервалы, которые служат для отображения в графике "Итого".
		/// Коллекция данных интервалов добавлена в связи с необходимостью корректно отображать интервалы, переходящие через сутки,
		/// так как в таких случаях отображается интервал: "Время входа - 00:00", вместо "Время входа - Время выхода".
		/// </summary>
		public List<TimeTrackPart> RealTimeDesignTimeTrackParts { get; set; }

		private bool IsCrossNight { get { return false; } }

		/// <summary>
		/// Интервалы, переходящие через сутки (текущую дату)
		/// <example>EnterDateTime == 03.04.2015 && ExitDateTime == 04.04.2015</example>
		/// </summary>
		public List<TimeTrackPart> CrossNightTimeTrackParts { get; set; }

		public List<TimeTrackPart> RealTimeTrackPartsForCalculates { get; set; }

		#endregion

		/// <summary>
		/// Основной метод по вычислению всех интервалов
		/// </summary>
		public void Calculate()
		{
			CalculateDocuments();

			PlannedTimeTrackParts = PlannedTimeTrackParts;
			RealTimeTrackParts.AddRange(CrossNightTimeTrackParts);
			CrossNightTimeTrackParts = CalculateCrossNightTimeTrackParts(RealTimeTrackParts, Date);
			RealTimeTrackParts = RealTimeTrackParts.OrderBy(x => x.EnterDateTime).ThenBy(x => x.ExitDateTime).ToList();
			RealTimeTrackPartsForCalculates = GetRealTimeTracksForCalculate(RealTimeTrackParts.Where(x => x.ExitDateTime.HasValue && x.IsForURVZone && !x.NotTakeInCalculations));
			CombinedTimeTrackParts = CalculateCombinedTimeTrackParts(PlannedTimeTrackParts, RealTimeTrackPartsForCalculates,
																							DocumentTrackParts);
			if (SlideTime != default(TimeSpan))
			{
				CombinedTimeTrackParts = AdjustmentCombinedTimeTracks(CombinedTimeTrackParts, PlannedTimeTrackParts, RealTimeTrackParts, SlideTime);
				CombinedTimeTrackParts = TransferPresentToOvertime(CombinedTimeTrackParts, SlideTime);
			}

			CombinedTimeTrackParts = TransferNightSettings(CombinedTimeTrackParts, NightSettings);
			CombinedTimeTrackParts = ApplyDeviationPolitics(CombinedTimeTrackParts, AllowedAbsentLowThan, AllowedEarlyLeave, AllowedLate, NotAllowOvertimeLowerThan);
			CombinedTimeTrackParts = CorrectDocumentIntervals(CombinedTimeTrackParts, SlideTime);
			RealTimeTrackPartsForCalculates = FillTypesForRealTimeTrackParts(RealTimeTrackPartsForCalculates, PlannedTimeTrackParts);
			Totals = CalculateTotal(SlideTime, PlannedTimeTrackParts, RealTimeTrackPartsForCalculates, CombinedTimeTrackParts, IsHoliday);
			Totals = GetTotalBalance(Totals);
			TimeTrackType = CalculateTimeTrackType(Totals, PlannedTimeTrackParts, IsHoliday, Error);
			CalculateLetterCode();
		}

		private List<TimeTrackTotal> GetTotalBalance(List<TimeTrackTotal> totalCollection)
		{
			var totalAbsence = new TimeSpan();
			var abcence = totalCollection.Where(x => x.TimeTrackType == TimeTrackType.Late
			                                         || x.TimeTrackType == TimeTrackType.Absence
			                                         || x.TimeTrackType == TimeTrackType.EarlyLeave
			                                         || x.TimeTrackType == TimeTrackType.DocumentAbsence)
													 .Select(x => x.TimeSpan)
													 .Aggregate(totalAbsence, (span, timeSpan) => span + timeSpan);
			var totalOvertime = new TimeSpan();
			var overtime = totalCollection.Where(x => x.TimeTrackType == TimeTrackType.Overtime
			                                           || x.TimeTrackType == TimeTrackType.DocumentOvertime)
													   .Select(x => x.TimeSpan)
													   .Aggregate(totalOvertime, (span, timeSpan) => span + timeSpan);

			var totalBalance = totalCollection.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Balance);
			if (totalBalance != null)
			{
				totalBalance.TimeSpan = overtime - abcence;
			}
			else
			{
				return new List<TimeTrackTotal>();
			}

			return totalCollection;
		}

		private List<TimeTrackPart> CorrectDocumentIntervals(List<TimeTrackPart> combinedTimeTrackParts, TimeSpan slideTIme)
		{
			var result = new List<TimeTrackPart>();
			var tmpCalc = 0.0;

			foreach (var timeTrackPart in combinedTimeTrackParts)
			{
				if (timeTrackPart.TimeTrackPartType == TimeTrackType.DocumentPresence)
				{
					tmpCalc += timeTrackPart.Delta.TotalSeconds;

					if (tmpCalc > SlideTime.TotalSeconds)
					{
						timeTrackPart.ExitDateTime = timeTrackPart.ExitDateTime - TimeSpan.FromSeconds(tmpCalc - slideTIme.TotalSeconds);
						result.Add(timeTrackPart);
						break;
					}
				}

				result.Add(timeTrackPart);
			}
			return result;
		}

		private List<TimeTrackPart> ApplyDeviationPolitics(IEnumerable<TimeTrackPart> combinedTimeTrackParts, int allowedAbsent,
			int allowedEarlyLeave, int allowedLate, int notAllowedOvertime)
		{
			var resultCollection = new List<TimeTrackPart>();
			foreach (var combinedTimeTrackPart in combinedTimeTrackParts)
			{
				if (CanApplyDeviationPolitics(combinedTimeTrackPart, allowedAbsent, allowedEarlyLeave, allowedLate, notAllowedOvertime))
					combinedTimeTrackPart.TimeTrackPartType = TimeTrackType.Presence;

				resultCollection.Add(combinedTimeTrackPart);
			}

			return resultCollection;
		}

		private bool CanApplyDeviationPolitics(TimeTrackPart timeTrackPart, int allowedAbsent, int allowedEarlyLeave, int allowedLate, int notAllowedOvertime)
		{
			var isApplyForAbsence = timeTrackPart.TimeTrackPartType == TimeTrackType.Absence && timeTrackPart.Delta.TotalMinutes <= allowedAbsent && allowedAbsent != default (int);
			var isApplyForEarlyLeave = timeTrackPart.TimeTrackPartType == TimeTrackType.EarlyLeave && timeTrackPart.Delta.TotalMinutes <= allowedEarlyLeave && allowedEarlyLeave != default(int);
			var isApplyForLate = timeTrackPart.TimeTrackPartType == TimeTrackType.Late && timeTrackPart.Delta.TotalMinutes <= allowedLate && allowedLate != default(int);
			var isApplyForOverTime = timeTrackPart.TimeTrackPartType == TimeTrackType.Overtime && timeTrackPart.Delta.TotalMinutes <= notAllowedOvertime && notAllowedOvertime != default(int);

			return isApplyForAbsence || isApplyForEarlyLeave || isApplyForLate || isApplyForOverTime;
		}

		private List<TimeTrackPart> TransferNightSettings(List<TimeTrackPart> combinedTimeTrackParts,
			NightSettings nightSettings)
		{
			if (nightSettings == null) return combinedTimeTrackParts;

			var resultCollection = new List<TimeTrackPart>();

			foreach (var el in combinedTimeTrackParts)
			{
				if (el.TimeTrackPartType == TimeTrackType.Presence
					&& el.EnterDateTime.TimeOfDay <= nightSettings.NightEndTime
					&& el.ExitDateTime.GetValueOrDefault().TimeOfDay >= NightSettings.NightStartTime)
				{
					var night = new TimeTrackPart {TimeTrackPartType = TimeTrackType.Night};
					//Вычисляем время входа нового интервала ночного времени
					if (el.EnterDateTime.TimeOfDay <= nightSettings.NightStartTime)
					{
						night.EnterDateTime = el.EnterDateTime.Date + nightSettings.NightStartTime;
					}
					else if (el.EnterDateTime.TimeOfDay > nightSettings.NightStartTime)
					{
						night.EnterDateTime = el.EnterDateTime;
					}

					//Вычисляем время выхода нового интервала ночного времени
					if (el.ExitDateTime.GetValueOrDefault().TimeOfDay >= nightSettings.NightEndTime)
					{
						night.ExitDateTime = el.ExitDateTime.GetValueOrDefault().Date + nightSettings.NightEndTime;
					}
					else if (el.ExitDateTime.GetValueOrDefault().TimeOfDay < nightSettings.NightEndTime)
					{
						night.ExitDateTime = el.ExitDateTime;
					}

					//Вычисляем как и в какую сторону смещать интервал явки
					if (el.EnterDateTime.TimeOfDay < night.EnterDateTime.TimeOfDay
					    && el.ExitDateTime.GetValueOrDefault().TimeOfDay <= night.ExitDateTime.GetValueOrDefault().TimeOfDay)
					{
						el.ExitDateTime = night.EnterDateTime;
					}
					else if (el.EnterDateTime.TimeOfDay >= night.EnterDateTime.TimeOfDay
					         && el.ExitDateTime.GetValueOrDefault().TimeOfDay > night.ExitDateTime.GetValueOrDefault().TimeOfDay)
					{
						el.EnterDateTime = night.ExitDateTime.GetValueOrDefault();
					}
					else if (el.EnterDateTime.TimeOfDay < night.EnterDateTime.TimeOfDay
					         && el.ExitDateTime.GetValueOrDefault().TimeOfDay > night.ExitDateTime.GetValueOrDefault().TimeOfDay)
					{
						var tmpExitTime = el.ExitDateTime;
						el.ExitDateTime = night.EnterDateTime;
						var newIntervalsOfPresent = new TimeTrackPart
						{
							TimeTrackPartType = TimeTrackType.Presence,
							EnterDateTime = night.ExitDateTime.GetValueOrDefault(),
							ExitDateTime = tmpExitTime
						};
						resultCollection.Add(newIntervalsOfPresent);
					}
					else
					{
						el.TimeTrackPartType = TimeTrackType.Night;
					}

					if(el.TimeTrackPartType != TimeTrackType.Night)
						resultCollection.Add(night);
				}

				resultCollection.Add(el);
			}
			resultCollection = resultCollection.OrderBy(x => x.EnterDateTime.TimeOfDay).ThenBy(x => x.ExitDateTime.GetValueOrDefault().TimeOfDay).ToList();
			return resultCollection;
		}

		private List<TimeTrackPart> TransferPresentToOvertime(List<TimeTrackPart> combinedTimeTrackParts, TimeSpan slideTime)
		{
			var totalPresentTime = - slideTime;

			var resultCollection = new List<TimeTrackPart>();

			foreach (var el in combinedTimeTrackParts)
			{
				if (el.TimeTrackPartType == TimeTrackType.Presence)
				{
					if (totalPresentTime == default(TimeSpan))
					{
						resultCollection.Add(el);
						continue;
					}

					totalPresentTime += el.Delta;

					if (totalPresentTime > default(TimeSpan))
					{
						var tmpExitTime = el.ExitDateTime;
						el.ExitDateTime = tmpExitTime - totalPresentTime;
						var overtimeTrackPart = new TimeTrackPart
						{
							EnterDateTime = el.ExitDateTime.GetValueOrDefault(),
							ExitDateTime = tmpExitTime,
							TimeTrackPartType = TimeTrackType.Overtime
						};
						resultCollection.Add(el);
						resultCollection.Add(overtimeTrackPart);
						totalPresentTime = default(TimeSpan);
						continue;
					}
				}

				resultCollection.Add(el);
			}

			return resultCollection;
		}

		private TimeSpan GetSummOfPlannedTimeTrackParts(List<TimeTrackPart> plannedTimeTrackParts)
		{
			var result = new TimeSpan();
			foreach (var plannedTimeTrackPart in plannedTimeTrackParts)
			{
				result += plannedTimeTrackPart.Delta;
			}
			return result;
		}

		private List<TimeTrackPart> AdjustmentCombinedTimeTracks(List<TimeTrackPart> combinedTimeTrackParts, List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> realTimeTrackParts, TimeSpan slideTime)
		{
			var sumPlannedTime = GetSummOfPlannedTimeTrackParts(plannedTimeTrackParts); //Суммарное время графика
			var differWithPlannedTime = sumPlannedTime - slideTime; //Показывает доступное время неявки
			const double TOLERANCE = 0.000001;

			var resultedCombinedCollection = new List<TimeTrackPart>();

			foreach (var el in combinedTimeTrackParts)
			{
				var isInPlannedTime =
					plannedTimeTrackParts.Any(
						x =>
							x.EnterDateTime.TimeOfDay <= el.ExitDateTime.GetValueOrDefault().TimeOfDay &&
							x.ExitDateTime.GetValueOrDefault().TimeOfDay >= el.EnterDateTime.TimeOfDay);

				if (isInPlannedTime)
				{
					if (el.TimeTrackPartType == TimeTrackType.Absence
						|| el.TimeTrackPartType == TimeTrackType.EarlyLeave
						|| el.TimeTrackPartType == TimeTrackType.Late) //Если интервал с типом неявки
					{
						if (Math.Abs(differWithPlannedTime.TotalSeconds) < TOLERANCE)
						{
							resultedCombinedCollection.Add(el);
							continue;
						}

						differWithPlannedTime -= el.Delta;

						if (differWithPlannedTime.TotalSeconds < 0 && el.TimeTrackPartType == TimeTrackType.EarlyLeave)
						{
							el.ExitDateTime += differWithPlannedTime;
							differWithPlannedTime = default(TimeSpan);
							resultedCombinedCollection.Add(el);
						}
						else if (differWithPlannedTime.TotalSeconds < 0 && el.TimeTrackPartType == TimeTrackType.Absence)
						{
							el.EnterDateTime = el.ExitDateTime.GetValueOrDefault() + differWithPlannedTime;
							differWithPlannedTime = default(TimeSpan);
							resultedCombinedCollection.Add(el);
						}
						else if (differWithPlannedTime.TotalSeconds < 0 && el.TimeTrackPartType == TimeTrackType.Late)
						{
							el.EnterDateTime = el.ExitDateTime.GetValueOrDefault() + differWithPlannedTime;
							differWithPlannedTime = default(TimeSpan);
							resultedCombinedCollection.Add(el);
						}

						continue;
					}
				}

				resultedCombinedCollection.Add(el);
			}

			return resultedCombinedCollection;
		}

		private List<TimeTrackPart> GetRealTimeTracksForCalculate(IEnumerable<TimeTrackPart> realTimeTrackParts)
		{
			var resultCollection = new List<TimeTrackPart>();

			foreach (var timeTrackPart in realTimeTrackParts)
			{
				TimeTrackPart timeTrackPartItem = timeTrackPart;

				if (timeTrackPart.EnterDateTime.Date == Date && timeTrackPart.ExitDateTime.Value.Date > Date)
				{
					timeTrackPartItem = new TimeTrackPart
					{
						EnterDateTime = timeTrackPart.EnterDateTime,
						ExitDateTime = Date.AddDays(1).AddTicks(-1), //23.59.59.9999999
						TimeTrackPartType = timeTrackPart.TimeTrackPartType,
						ZoneUID = timeTrackPart.ZoneUID,
						StartsInPreviousDay = timeTrackPart.StartsInPreviousDay,
						EndsInNextDay = timeTrackPart.EndsInNextDay,
						DayName = timeTrackPart.DayName,
						PassJournalUID = timeTrackPart.PassJournalUID,
						IsNeedAdjustment = timeTrackPart.IsNeedAdjustment,
						AdjustmentDate = timeTrackPart.AdjustmentDate,
						CorrectedByUID = timeTrackPart.CorrectedByUID,
						EnterTimeOriginal = timeTrackPart.EnterTimeOriginal,
						ExitTimeOriginal = timeTrackPart.ExitTimeOriginal,
						NotTakeInCalculations = timeTrackPart.NotTakeInCalculations,
						IsForURVZone = timeTrackPart.IsForURVZone,
						IsManuallyAdded = timeTrackPart.IsManuallyAdded,
						IsForceClosed = timeTrackPart.IsForceClosed
					};
				}
				else if (timeTrackPart.EnterDateTime.Date < Date && timeTrackPart.ExitDateTime.Value.Date == Date)
				{
					timeTrackPartItem = new TimeTrackPart
					{
						EnterDateTime = Date, //00:00
						ExitDateTime = timeTrackPart.ExitDateTime,
						TimeTrackPartType = timeTrackPart.TimeTrackPartType,
						ZoneUID = timeTrackPart.ZoneUID,
						StartsInPreviousDay = timeTrackPart.StartsInPreviousDay,
						EndsInNextDay = timeTrackPart.EndsInNextDay,
						DayName = timeTrackPart.DayName,
						PassJournalUID = timeTrackPart.PassJournalUID,
						IsNeedAdjustment = timeTrackPart.IsNeedAdjustment,
						AdjustmentDate = timeTrackPart.AdjustmentDate,
						CorrectedByUID = timeTrackPart.CorrectedByUID,
						EnterTimeOriginal = timeTrackPart.EnterTimeOriginal,
						ExitTimeOriginal = timeTrackPart.ExitTimeOriginal,
						NotTakeInCalculations = timeTrackPart.NotTakeInCalculations,
						IsForURVZone = timeTrackPart.IsForURVZone,
						IsManuallyAdded = timeTrackPart.IsManuallyAdded,
						IsForceClosed = timeTrackPart.IsForceClosed
					};
				}
				else if (timeTrackPart.EnterDateTime.Date < Date && timeTrackPart.ExitDateTime.Value.Date > Date)
				{
					timeTrackPartItem = new TimeTrackPart
					{
						EnterDateTime = Date, //00:00
						ExitDateTime = Date.AddDays(1).AddTicks(-1), //23.59.59.9999999
						TimeTrackPartType = timeTrackPart.TimeTrackPartType,
						ZoneUID = timeTrackPart.ZoneUID,
						StartsInPreviousDay = timeTrackPart.StartsInPreviousDay,
						EndsInNextDay = timeTrackPart.EndsInNextDay,
						DayName = timeTrackPart.DayName,
						PassJournalUID = timeTrackPart.PassJournalUID,
						IsNeedAdjustment = timeTrackPart.IsNeedAdjustment,
						AdjustmentDate = timeTrackPart.AdjustmentDate,
						CorrectedByUID = timeTrackPart.CorrectedByUID,
						EnterTimeOriginal = timeTrackPart.EnterTimeOriginal,
						IsForURVZone = timeTrackPart.IsForURVZone,
						ExitTimeOriginal = timeTrackPart.ExitTimeOriginal,
						NotTakeInCalculations = timeTrackPart.NotTakeInCalculations,
						IsManuallyAdded = timeTrackPart.IsManuallyAdded,
						IsForceClosed = timeTrackPart.IsForceClosed
					};
				}

				resultCollection.Add(timeTrackPartItem);
			}

			return resultCollection;
		}

		/// <summary>
		/// Расчитывает типы для временных интервалов прохода сотрудника
		/// </summary>
		/// <param name="realTimeTrackParts">Коллекция временных интервалов проходов сотрудника</param>
		/// <param name="plannedTimeTrackParts">Коллекция временных интервалов графика работ</param>
		/// <returns>Коллекция временных интервалов проходов сотрудника с заполненными типами проходов</returns>
		private List<TimeTrackPart> FillTypesForRealTimeTrackParts(List<TimeTrackPart> realTimeTrackParts, List<TimeTrackPart> plannedTimeTrackParts)
		{
			var resultCollection = new List<TimeTrackPart>();
			var scheduleTimeInterval = GetPlannedScheduleInterval(plannedTimeTrackParts);

			foreach (var timeTrackPart in realTimeTrackParts.Where(timeTrackPart => timeTrackPart.ExitDateTime.HasValue))
			{
				timeTrackPart.TimeTrackPartType = GetTimeTrackType(timeTrackPart, plannedTimeTrackParts, realTimeTrackParts, IsOnlyFirstEnter, scheduleTimeInterval,
					new ScheduleInterval(timeTrackPart.EnterDateTime, timeTrackPart.ExitDateTime.Value));
				resultCollection.Add(timeTrackPart);
			}

			return resultCollection;
		}

		private void CalculateDocuments()
		{
			DocumentTrackParts = new List<TimeTrackPart>();
			foreach (var document in Documents)
			{
				TimeTrackPart timeTrackPart = null;
				if (document.StartDateTime.Date < Date && document.EndDateTime.Date > Date)
				{
					timeTrackPart = new TimeTrackPart
					{
						EnterDateTime = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0), //TimeSpan.Zero
						ExitDateTime =  new DateTime(Date.Year, Date.Month, (Date.Day + 1), 0, 0, 0)//new TimeSpan(23, 59, 59)
					};
				}
				if (document.StartDateTime.Date == Date && document.EndDateTime.Date > Date)
				{
					timeTrackPart = new TimeTrackPart
					{
						EnterDateTime = document.StartDateTime, //document.StartDateTime.TimeOfDay,
						ExitDateTime = new DateTime(document.StartDateTime.Year, document.StartDateTime.Month, (document.StartDateTime.Day + 1), 0, 0, 0)  //new TimeSpan(23, 59, 59)
					};
				}
				if (document.StartDateTime.Date == Date && document.EndDateTime.Date == Date)
				{
					timeTrackPart = new TimeTrackPart
					{
						EnterDateTime = document.StartDateTime, //document.StartDateTime.TimeOfDay,
						ExitDateTime = document.EndDateTime//document.EndDateTime.TimeOfDay
					};
				}
				if (document.StartDateTime.Date < Date && document.EndDateTime.Date == Date)
				{
					timeTrackPart = new TimeTrackPart
					{
						EnterDateTime = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0), //TimeSpan.Zero,
						ExitDateTime = document.EndDateTime//document.EndDateTime.TimeOfDay
					};
				}
				if (timeTrackPart != null)
				{
					timeTrackPart.MinTimeTrackDocumentType = document.TimeTrackDocumentType;
					timeTrackPart.IsOutside = document.IsOutside;
					DocumentTrackParts.Add(timeTrackPart);
				}
			}
			DocumentTrackParts = DocumentTrackParts.OrderBy(x => x.EnterDateTime.Ticks).ToList();

			var dateTimes = new List<DateTime?>();
			foreach (var trackPart in DocumentTrackParts.Where(x => x.ExitDateTime.HasValue))
			{
				dateTimes.Add(trackPart.EnterDateTime);
				dateTimes.Add(trackPart.ExitDateTime);
			}
			dateTimes = dateTimes.OrderBy(x => x.Value.TimeOfDay.TotalSeconds).ToList();

			var result = new List<TimeTrackPart>();
			for (int i = 0; i < dateTimes.Count - 1; i++)
			{
				var startTimeSpan = dateTimes[i].Value;
				var endTimeSpan = dateTimes[i + 1].Value;
				var timeTrackParts = DocumentTrackParts.Where(x => x.EnterDateTime <= startTimeSpan && x.ExitDateTime > startTimeSpan);

				if (timeTrackParts.Any())
				{
					var newTimeTrackPart = new TimeTrackPart
					{
						EnterDateTime = startTimeSpan,
						ExitDateTime = endTimeSpan,
					};
					foreach (var timeTrackPart in timeTrackParts)
					{
						if (newTimeTrackPart.MinTimeTrackDocumentType == null)
							newTimeTrackPart.MinTimeTrackDocumentType = timeTrackPart.MinTimeTrackDocumentType;
						else if (timeTrackPart.MinTimeTrackDocumentType.DocumentType < newTimeTrackPart.MinTimeTrackDocumentType.DocumentType)
							newTimeTrackPart.MinTimeTrackDocumentType = timeTrackPart.MinTimeTrackDocumentType;
						newTimeTrackPart.IsOutside = timeTrackPart.IsOutside;
						newTimeTrackPart.TimeTrackDocumentTypes.Add(timeTrackPart.MinTimeTrackDocumentType);
					}

					result.Add(newTimeTrackPart);
				}
			}
			DocumentTrackParts = result;
		}

		public ScheduleInterval GetPlannedScheduleInterval(List<TimeTrackPart> plannedTimeTrackParts)
		{
			var plannedScheduleTime = new ScheduleInterval();

			if (plannedTimeTrackParts.Count <= 0) return plannedScheduleTime;

			var startTime = plannedTimeTrackParts.FirstOrDefault();
			var endTime = plannedTimeTrackParts.LastOrDefault();

			plannedScheduleTime.StartTime = startTime != null ? startTime.EnterDateTime : default(DateTime);
			plannedScheduleTime.EndTime = endTime != null ? endTime.ExitDateTime : default(DateTime);

			return plannedScheduleTime;
		}

		/// <summary>
		/// Определяет начальное и конечное время всех интервалов и расчитывает для них тип
		/// </summary>
		/// <param name="plannedTimeTrackParts">Коллекция временных интервалов графика работ</param>
		/// <param name="realTimeTrackParts">Коллекция временных интервалов прохода сотрудника</param>
		/// <param name="documentTimeTrackParts">Коллекция интервалов, закрытых документами</param>
		/// <returns>Возвращает все типы интервалов за текущий день</returns>
		public List<TimeTrackPart> CalculateCombinedTimeTrackParts(List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> realTimeTrackParts, List<TimeTrackPart> documentTimeTrackParts)
		{
			var combinedTimeSpans = GetCombinedDateTimes(realTimeTrackParts, plannedTimeTrackParts, documentTimeTrackParts);
			combinedTimeSpans = combinedTimeSpans.Where(x => x.HasValue).OrderBy(x => x.GetValueOrDefault().TimeOfDay.TotalSeconds).ToList();

			var combinedTimeTrackParts = new List<TimeTrackPart>();

			for (var i = 0; i < combinedTimeSpans.Count - 1; i++)
			{
				var combinedInterval = new ScheduleInterval(combinedTimeSpans[i].Value, combinedTimeSpans[i + 1].Value); //TODO: this variable can be killed. Cuz combinedInterval is equal timeTrackPart

				if(!combinedInterval.EndTime.HasValue) continue;

				var realTimeTrackPart = realTimeTrackParts.FirstOrDefault(x => x.EnterDateTime.TimeOfDay <= combinedInterval.StartTime.TimeOfDay && x.ExitDateTime.GetValueOrDefault().TimeOfDay >= combinedInterval.EndTime.GetValueOrDefault().TimeOfDay);
				var timeTrackPart = new TimeTrackPart
				{
					EnterDateTime = combinedInterval.StartTime,
					ExitDateTime = combinedInterval.EndTime,
					NotTakeInCalculations = realTimeTrackPart != null && realTimeTrackPart.NotTakeInCalculations,
					IsForceClosed = realTimeTrackPart != null && realTimeTrackPart.IsForceClosed
				};

				combinedTimeTrackParts.Add(timeTrackPart);

				var hasRealTimeTrack = realTimeTrackParts.Where(x => x.ExitDateTime.HasValue && !x.NotTakeInCalculations && x.IsForURVZone)
					.Any(x => x.EnterDateTime.TimeOfDay <= combinedInterval.StartTime.TimeOfDay && x.ExitDateTime.Value.TimeOfDay >= combinedInterval.EndTime.Value.TimeOfDay);

				var hasPlannedTimeTrack = plannedTimeTrackParts.Where(x => x.ExitDateTime.HasValue).Any(x => x.EnterDateTime.TimeOfDay <= combinedInterval.StartTime.TimeOfDay
																		&& x.ExitDateTime.Value.TimeOfDay >= combinedInterval.EndTime.Value.TimeOfDay);

				var documentTimeTrack = documentTimeTrackParts.Where(x => x.ExitDateTime.HasValue).FirstOrDefault(x => x.EnterDateTime.TimeOfDay <= combinedInterval.StartTime.TimeOfDay
																			&& x.ExitDateTime.Value.TimeOfDay >= combinedInterval.EndTime.Value.TimeOfDay);

				timeTrackPart.TimeTrackPartType = GetTimeTrackType(timeTrackPart, plannedTimeTrackParts, realTimeTrackParts, IsOnlyFirstEnter, GetPlannedScheduleInterval(plannedTimeTrackParts), combinedInterval);

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
			//TODO: Make test for this method
			var documentType = documentTimeTrack.MinTimeTrackDocumentType.DocumentType;
			timeTrackPart.MinTimeTrackDocumentType = documentTimeTrack.MinTimeTrackDocumentType;

			switch (documentType)
			{
				case DocumentType.Overtime:
					return TimeTrackType.DocumentOvertime;
				case DocumentType.Presence:
					return hasPlannedTimeTrack ? TimeTrackType.DocumentPresence : TimeTrackType.None;
				case DocumentType.Absence:
					if (documentTimeTrack.IsOutside)
						return TimeTrackType.DocumentAbsence;
					return (hasRealTimeTrack || hasPlannedTimeTrack) ? TimeTrackType.DocumentAbsence : TimeTrackType.None;
				case DocumentType.AbsenceReasonable:
					if (documentTimeTrack.IsOutside)
						return TimeTrackType.DocumentAbsence;
					return (hasRealTimeTrack || hasPlannedTimeTrack) ? TimeTrackType.DocumentAbsenceReasonable : TimeTrackType.None;
				default:
					return TimeTrackType.None;
			}
		}

		/// <summary>
		/// Получение типа интервала прохода, формируемого в графике "Итого"
		/// </summary>
		/// <param name="timeTrackPart">Время, отслеженное в УРВ</param>
		/// <param name="realTimeTrackParts">Коллекция интервалов прохода сотрудника</param>
		/// <param name="plannedTimeTrackParts">Коллекция интервалов графика работ</param>
		/// <param name="isOnlyFirstEnter">Флаг, показывающий, работает ли система в режиме "первый вход-последний выход"</param>
		/// <param name="schedulePlannedInterval">Начальное и конечное время графика работы</param>
		/// <param name="combinedInterval">Начальное время и конечное время временного интервала в УРВ</param>
		/// <returns>Возвращает тип интервала прохода для расчета баланса</returns>
		public TimeTrackType GetTimeTrackType(TimeTrackPart timeTrackPart, List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> realTimeTrackParts,  bool isOnlyFirstEnter, ScheduleInterval schedulePlannedInterval, ScheduleInterval combinedInterval)
		{
			bool hasRealTimeTrack = false, hasPlannedTimeTrack = false;
				hasRealTimeTrack = realTimeTrackParts.Where(x => x.ExitDateTime.HasValue && !x.NotTakeInCalculations && x.IsForURVZone).Any(x =>
					combinedInterval.EndTime != null &&
					(x.ExitDateTime != null &&
					(x.EnterDateTime.TimeOfDay <= combinedInterval.StartTime.TimeOfDay && x.ExitDateTime.Value.TimeOfDay >= combinedInterval.EndTime.Value.TimeOfDay)));

				hasPlannedTimeTrack = plannedTimeTrackParts
					.Where(x => x.ExitDateTime.HasValue)
					.Any(x =>
						combinedInterval.EndTime != null &&
						(x.ExitDateTime != null &&
						(x.EnterDateTime.TimeOfDay <= combinedInterval.StartTime.TimeOfDay && x.ExitDateTime.Value.TimeOfDay >= combinedInterval.EndTime.Value.TimeOfDay)));
			//Если есть интервал прохода сотрудника, который попадает в гафик работ, то "Явка"
			if (hasRealTimeTrack && hasPlannedTimeTrack) //TODO: hasPlannedTimeTrack flag may be killed by inserting (timeTrackPart.StartTime >= schedulePlannedInterval.StartTime && timeTrackPart.EndTime <= schedulePlannedInterval.EndTime)
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
			if (hasRealTimeTrack)
			{
				return TimeTrackType.Overtime;
			}

			//Если нет интервала прохода сотрудника, но есть интервал рабочего графика
			timeTrackPart.TimeTrackPartType = TimeTrackType.Absence; //Отсутствие

			//Если учитывается первый вход-последний выход и проход в рамках графика, то "Отсутствие в рамках графика"
			if (realTimeTrackParts
				.Any(x => x.EnterDateTime.TimeOfDay >= combinedInterval.StartTime.TimeOfDay && x.ExitDateTime.Value.TimeOfDay >= combinedInterval.EndTime.Value.TimeOfDay)
				&& isOnlyFirstEnter)
			{
				return TimeTrackType.AbsenceInsidePlan;
			}

			if (plannedTimeTrackParts.Any(x => x.EnterDateTime.TimeOfDay == timeTrackPart.EnterDateTime.TimeOfDay) && //TODO: describe it
				plannedTimeTrackParts.All(x => x.ExitDateTime.Value.TimeOfDay != timeTrackPart.ExitDateTime.Value.TimeOfDay) &&
				realTimeTrackParts.Any(x => x.EnterDateTime.TimeOfDay == timeTrackPart.ExitDateTime.Value.TimeOfDay))
			{
				var firstPlannedTimeTrack = plannedTimeTrackParts.FirstOrDefault(x => x.EnterDateTime.TimeOfDay == timeTrackPart.ExitDateTime.Value.TimeOfDay);
				if (firstPlannedTimeTrack != null && firstPlannedTimeTrack.StartsInPreviousDay)
				{
					return TimeTrackType.Absence;
				}

				var firstPlannedInterval = plannedTimeTrackParts.FirstOrDefault();
				if (firstPlannedInterval != null && firstPlannedInterval.EnterDateTime.TimeOfDay == timeTrackPart.EnterDateTime.TimeOfDay) //Если расчитываемый интервал - первый, то возможен тип опоздание
					return TimeTrackType.Late;
			}

			if (plannedTimeTrackParts.Any(x => x.EnterDateTime.TimeOfDay == timeTrackPart.EnterDateTime.TimeOfDay) ||		//TODO: describe it
				plannedTimeTrackParts.All(x => x.ExitDateTime.Value.TimeOfDay != timeTrackPart.ExitDateTime.Value.TimeOfDay) ||
				realTimeTrackParts.All(x => x.ExitDateTime.Value.TimeOfDay != timeTrackPart.EnterDateTime.TimeOfDay)) return TimeTrackType.Absence;

			var lastPlannedTimeTrack = plannedTimeTrackParts.FirstOrDefault(x => x.ExitDateTime.Value.TimeOfDay == timeTrackPart.ExitDateTime.Value.TimeOfDay);
			if (lastPlannedTimeTrack != null && lastPlannedTimeTrack.EndsInNextDay)
			{
				return TimeTrackType.Absence;
			}

			return TimeTrackType.EarlyLeave;
		}

		private List<DateTime?> GetCombinedDateTimes(List<TimeTrackPart> realTimeTrackParts, List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> documentTimeTrackParts)
		{
			var combinedTimeSpans = new List<DateTime?>();
			foreach (var trackPart in realTimeTrackParts.Where(x => !x.NotTakeInCalculations))
			{
				combinedTimeSpans.Add(trackPart.EnterDateTime);
				combinedTimeSpans.Add(trackPart.ExitDateTime);
			}
			foreach (var trackPart in plannedTimeTrackParts)
			{
				combinedTimeSpans.Add(trackPart.EnterDateTime);
				combinedTimeSpans.Add(trackPart.ExitDateTime);
			}
			foreach (var trackPart in documentTimeTrackParts)
			{
				combinedTimeSpans.Add(trackPart.EnterDateTime);
				combinedTimeSpans.Add(trackPart.ExitDateTime);
			}
			return combinedTimeSpans;
		}

		/// <summary>
		/// Расчитывает баланс и время для интервалов каждого типа
		/// </summary>
		/// <param name="slideTime">Время скользящего графика</param>
		/// <param name="plannedTimeTrackParts">Коллекция временных интервалов графика работ</param>
		/// <param name="realTimeTrackParts">Коллекция интервалов прохода сотрудника</param>
		/// <param name="combinedTimeTrackParts">Коллекция всех интервалов (графика работ, проходов сотрудника, документов)</param>
		/// <returns>Возвращает коллекцию всех типов интервалов</returns>
		public List<TimeTrackTotal> CalculateTotal(TimeSpan slideTime, List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> realTimeTrackParts, List<TimeTrackPart> combinedTimeTrackParts, bool isHoliday)
		{
			var resultTotalCollection = new List<TimeTrackTotal>();

			var totalBalance = GetBalanceForSlideTime(slideTime, plannedTimeTrackParts, realTimeTrackParts);

			foreach (var timeTrack in combinedTimeTrackParts)
			{
				var el = resultTotalCollection.FirstOrDefault(x => x.TimeTrackType == timeTrack.TimeTrackPartType);
				if (el != null) //TODO: Need refactoring
				{
					el.TimeSpan += GetDeltaForTimeTrack(timeTrack, isHoliday);
				}
				else
				{
					resultTotalCollection.Add(new TimeTrackTotal(timeTrack.TimeTrackPartType)
					{
						TimeSpan = GetDeltaForTimeTrack(timeTrack, isHoliday)
					});
				}

				totalBalance += GetBalance(timeTrack, slideTime.TotalSeconds);
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

			return FillTotalsCollection(resultTotalCollection);
		}

		/// <summary>
		/// Заполняет коллекцию проходов, добавляя элементы тех типов, которые не существуют во входной коллекции проходов.
		/// Не добавляет типы: Выходной, Праздник, Нет данных.
		/// </summary>
		/// <param name="inputCollectionTotals">Коллекция интервалов всех типов проходов</param>
		/// <returns>Коллекция интервалов всех типов для отображения пользователю</returns>
		private List<TimeTrackTotal> FillTotalsCollection(List<TimeTrackTotal> inputCollectionTotals)
		{
			var resultCollection = new List<TimeTrackTotal>();
			var timeTrackTypes = new List<TimeTrackType>(Enum.GetValues(typeof(TimeTrackType)).Cast<TimeTrackType>().ToList());
			var collection = timeTrackTypes
								.Select(x => x)
								.Where(x => !inputCollectionTotals
											.Select(timeTrack => timeTrack.TimeTrackType)
											.Contains(x)
											&& x != TimeTrackType.Holiday
											&& x != TimeTrackType.DayOff
											&& x != TimeTrackType.None);
			resultCollection.AddRange(collection.Select(x => new TimeTrackTotal(x)));
			resultCollection.AddRange(inputCollectionTotals);
			return resultCollection;
		}

		/// <summary>
		/// Получает часы, необходимые для расчета баланса, для каждого типа интервала времени
		/// </summary>
		/// <param name="timeTrack">Интервал УРВ</param>
		/// <param name="isHoliday">Флаг, определяющий праздничный день для конкретного дня</param>
		/// <returns>Время интервала</returns>
		public TimeSpan GetDeltaForTimeTrack(TimeTrackPart timeTrack, bool isHoliday)
		{
			if (!isHoliday) return timeTrack.Delta;

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
		public TimeSpan GetBalance(TimeTrackPart timeTrack, double slideTimeSecods)
		{
			if (timeTrack.NotTakeInCalculations) return TimeSpan.Zero;

			switch (timeTrack.TimeTrackPartType)
			{
				case TimeTrackType.DocumentOvertime:
				case TimeTrackType.DocumentPresence:
				case TimeTrackType.Presence:
				case TimeTrackType.Overtime:
					return timeTrack.Delta;

				case TimeTrackType.Late:
					if (timeTrack.Delta.TotalMinutes >= AllowedLate)
					{
						break;
					}
					return timeTrack.Delta;

				case TimeTrackType.Absence:
					if (timeTrack.Delta.TotalMinutes >= AllowedAbsentLowThan)
					{
						break;
					}
					return timeTrack.Delta;
				case TimeTrackType.EarlyLeave:
					if (timeTrack.Delta.TotalMinutes >= AllowedEarlyLeave)
					{
						break;
					}
					return timeTrack.Delta;

				case TimeTrackType.DocumentAbsence:
					return (-timeTrack.Delta);
			}

			return default(TimeSpan);
		}

		/// <summary>
		/// Расчитывает баланс для каждого прохода сотрудника. Не учитывается неразрешенная переработка.
		/// </summary>
		/// <param name="slideTimeTotalSeconds">Время скользящего графика в секундах</param>
		/// <param name="plannedTimeTrackParts">Интервалы рабочего графика</param>
		/// <param name="realTimeTrackParts">Интервалы проходов сотрудника</param>
		/// <returns>Баланс, вычисленный на основе скользящего графика</returns>
		private TimeSpan GetBalanceForSlideTime(TimeSpan slideTimeTotalSeconds, List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> realTimeTrackParts)
		{
			if (slideTimeTotalSeconds.TotalSeconds <= 0 && plannedTimeTrackParts.Any())
			{
				TimeSpan result = new TimeSpan();
				foreach (var plannedTimeTrackPart in plannedTimeTrackParts)
				{
					result -= plannedTimeTrackPart.Delta;
				}
				return result;
			}
			if (slideTimeTotalSeconds.TotalSeconds <= 0)
				return default(TimeSpan);

			var balanceTimeSpan = new TimeSpan();

			if (plannedTimeTrackParts.Count > 0)
				balanceTimeSpan = -TimeSpan.FromSeconds(slideTimeTotalSeconds.TotalSeconds);

			//foreach (var realTimeTrackPart in realTimeTrackParts.Where(x => x.TimeTrackPartType != TimeTrackType.Overtime && !x.NotTakeInCalculations))
			//{
			//	balanceTimeSpan += realTimeTrackPart.Delta;
			//}

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
		/// Вычисление типа преимущественного интервала, для отображения пользователю в таблице УРВ
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

			var presenceTimeTrack = totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Presence);
			if (presenceTimeTrack != null)
				longestTimeSpan = presenceTimeTrack.TimeSpan;

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
					case TimeTrackType.DocumentAbsenceReasonable:
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

			foreach (var trackPart in realTimeTrackParts.Where(x => x.ExitDateTime.HasValue))
			{
				if (trackPart.EnterDateTime.TimeOfDay <= start && trackPart.ExitDateTime.Value.TimeOfDay >= end)
				{
					result += end - start;
				}
				else
				{
					if ((trackPart.EnterDateTime.TimeOfDay >= start && trackPart.EnterDateTime.TimeOfDay <= end) ||
					    (trackPart.ExitDateTime.Value.TimeOfDay >= start && trackPart.ExitDateTime.Value.TimeOfDay <= end))
					{
						var minStartTime = trackPart.EnterDateTime.TimeOfDay < start ? start : trackPart.EnterDateTime.TimeOfDay;
						var minEndTime = trackPart.ExitDateTime.Value.TimeOfDay > end ? end : trackPart.ExitDateTime.Value.TimeOfDay;
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

			var timeScheduleIntervals = timeTrackParts.Select(timeTrackPart => new ScheduleInterval(timeTrackPart.EnterDateTime, timeTrackPart.ExitDateTime)).ToList();
			timeScheduleIntervals = timeScheduleIntervals.OrderBy(x => x.StartTime.TimeOfDay).ToList();

			foreach (var timeScheduleInterval in timeScheduleIntervals.Where(x => x.EndTime.HasValue))
			{
				var timeTrackPart = timeTrackParts.FirstOrDefault(x => x.EnterDateTime <= timeScheduleInterval.StartTime && x.ExitDateTime > timeScheduleInterval.StartTime);

				if(timeTrackPart == null) continue;

				result.Add(new TimeTrackPart
				{
					EnterDateTime = timeScheduleInterval.StartTime,
					ExitDateTime = timeScheduleInterval.EndTime,
					TimeTrackPartType = timeTrackPart.TimeTrackPartType,
					ZoneUID = timeTrackPart.ZoneUID,
					StartsInPreviousDay = timeTrackPart.StartsInPreviousDay,
					EndsInNextDay = timeTrackPart.EndsInNextDay,
					DayName = timeTrackPart.DayName,
					PassJournalUID = timeTrackPart.PassJournalUID,
					IsNeedAdjustment = timeTrackPart.IsNeedAdjustment,
					AdjustmentDate = timeTrackPart.AdjustmentDate,
					CorrectedByUID = timeTrackPart.CorrectedByUID,
					EnterTimeOriginal = timeTrackPart.EnterTimeOriginal,
					ExitTimeOriginal = timeTrackPart.ExitTimeOriginal,
					NotTakeInCalculations = timeTrackPart.NotTakeInCalculations,
					IsManuallyAdded = timeTrackPart.IsManuallyAdded,
					IsOpen = timeTrackPart.IsOpen,
					IsForceClosed = timeTrackPart.IsForceClosed
				});
			}
			return result;
		}

		/// <summary>
		/// Возвращает интервалы, которые переходят через сутки
		/// </summary>
		/// <param name="timeTrackParts">Содержит коллекцию временных интервалов сотрудника</param>
		/// <param name="date">Дата выбранного дня (для которого производится расчет)</param>
		/// <returns>Коллекцию интервалов, переходящих через сутки</returns>
		public List<TimeTrackPart> CalculateCrossNightTimeTrackParts(List<TimeTrackPart> timeTrackParts, DateTime date)
		{
			return timeTrackParts.Where(x => x.ExitDateTime.HasValue).Where(x => (x.EnterDateTime.Date == date.Date && x.ExitDateTime.Value.Date > date.Date)
											||(x.EnterDateTime.Date < date.Date && x.ExitDateTime.Value.Date > date.Date))
											.ToList();
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
				case TimeTrackType.DocumentAbsenceReasonable:
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
		public struct ScheduleInterval
		{
			public DateTime StartTime;
			public DateTime? EndTime;

			public ScheduleInterval(DateTime startTime, DateTime? endTime)
			{
				StartTime = startTime;
				EndTime = endTime;
			}
		}
	}
}