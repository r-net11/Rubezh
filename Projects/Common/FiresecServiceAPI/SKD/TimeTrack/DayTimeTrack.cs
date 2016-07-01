using System.Diagnostics;
using System.Windows.Media;
using FiresecAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class DayTimeTrack
	{
		#region Fields
		private List<NightSettings> _nighTimeSpans;
		#endregion
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
			RealTimeTrackPartsForDrawing = new List<TimeTrackPart>();
			RealTimeTrackPartsForCalculates = new List<TimeTrackPart>();
			BackgroundColor = Colors.DarkGray;
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

		public List<TimeTrackPart> RealTimeTrackPartsForDrawing { get; set; }

		/// <summary>
		/// Интервалы, которые служат для отображения в графике "Итого".
		/// Коллекция данных интервалов добавлена в связи с необходимостью корректно отображать интервалы, переходящие через сутки,
		/// так как в таких случаях отображается интервал: "Время входа - 00:00", вместо "Время входа - Время выхода".
		/// </summary>
		public List<TimeTrackPart> RealTimeDesignTimeTrackParts { get; set; }

		/// <summary>
		/// Интервалы, переходящие через сутки (текущую дату)
		/// <example>EnterDateTime == 03.04.2015 && ExitDateTime == 04.04.2015</example>
		/// </summary>
		public List<TimeTrackPart> CrossNightTimeTrackParts { get; set; }

		public List<TimeTrackPart> RealTimeTrackPartsForCalculates { get; set; }

		private TimeSpan _nightTimeForToday;
		/// <summary>
		/// Содержит ночные часы, которые нужно отработать сотруднику за текущий день (DayTimeTrack.Date)
		/// </summary>
		public TimeSpan NightTimeForToday
		{
			get { return _nightTimeForToday; }
		}

		public Color BackgroundColor { get; set; }

		#endregion

		/// <summary>
		/// Основной метод по вычислению всех интервалов
		/// </summary>
		public void Calculate()
		{
			CalculateDocuments();

			PlannedTimeTrackParts = PlannedTimeTrackParts;
			CrossNightTimeTrackParts = CrossNightTimeTrackParts.Where(x => RealTimeTrackParts.All(y => y.PassJournalUID != x.PassJournalUID)).ToList();
			RealTimeTrackParts.AddRange(CrossNightTimeTrackParts);
			CrossNightTimeTrackParts = CalculateCrossNightTimeTrackParts(RealTimeTrackParts, Date);
			RealTimeTrackParts = RealTimeTrackParts.OrderBy(x => x.EnterDateTime).ThenBy(x => x.ExitDateTime).ToList();
			RealTimeTrackPartsForCalculates = GetRealTimeTracksForCalculate(RealTimeTrackParts.Where(x => x.ExitDateTime.HasValue && x.IsForURVZone && !x.NotTakeInCalculations));
			RealTimeTrackPartsForDrawing = GetRealTimeTrackPartsForDrawing(RealTimeTrackPartsForCalculates, IsOnlyFirstEnter);
			CombinedTimeTrackParts = CalculateCombinedTimeTrackParts(PlannedTimeTrackParts, RealTimeTrackPartsForDrawing, DocumentTrackParts);
			if (SlideTime != TimeSpan.Zero)
			{
				CombinedTimeTrackParts = AdjustmentCombinedTimeTracks(CombinedTimeTrackParts, PlannedTimeTrackParts, SlideTime);
				CombinedTimeTrackParts = TransferPresentToOvertime(CombinedTimeTrackParts, SlideTime);
			}

			if (NightSettings != null && NightSettings.IsNightSettingsEnabled)
				CalculateNightTimeSpans(NightSettings, ref _nightTimeForToday);

			CombinedTimeTrackParts = TransferNightSettings(CombinedTimeTrackParts, _nighTimeSpans);
			CombinedTimeTrackParts = ApplyDeviationPolitics(CombinedTimeTrackParts, AllowedAbsentLowThan, AllowedEarlyLeave, AllowedLate, NotAllowOvertimeLowerThan);

			if(SlideTime != TimeSpan.Zero)
				CombinedTimeTrackParts = CorrectDocumentIntervals(CombinedTimeTrackParts, SlideTime);

			RealTimeTrackPartsForCalculates = FillTypesForRealTimeTrackParts(RealTimeTrackPartsForCalculates, PlannedTimeTrackParts);
			Totals = CalculateTotal(SlideTime, PlannedTimeTrackParts, RealTimeTrackPartsForCalculates, CombinedTimeTrackParts, IsHoliday);
			Totals = GetTotalBalance(Totals);
			TimeTrackType = CalculateTimeTrackType(Totals, PlannedTimeTrackParts, IsHoliday, Error);
			SetLetterCode();
			SetBackgroundColor();
		}

		/// <summary>
		/// Метод установки фона ячейки
		/// </summary>
		private void SetBackgroundColor()
		{
			var balance = Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Balance);
			if (balance == null) return;

			if (PlannedTimeTrackParts.Any())
			{
				var absence = Totals
					.Where(x => x.TimeTrackType == TimeTrackType.Absence
							|| x.TimeTrackType == TimeTrackType.EarlyLeave
							|| x.TimeTrackType == TimeTrackType.Late)
					.ToList();
				BackgroundColor = GetColorBaseOnWeekDay(balance, absence);
			}
			else
				BackgroundColor = GetColorBaseOnHoliday(balance);
		}

		private static Color GetColorBaseOnWeekDay(TimeTrackTotal balance, IEnumerable<TimeTrackTotal> absences)
		{
			var balanceEqualZero = balance.TimeSpan == TimeSpan.Zero;
			var balanceMoreThanMore = balance.TimeSpan > TimeSpan.Zero;
			var balanceLessThanZero = balance.TimeSpan < TimeSpan.Zero;
			var hasAbsence = absences.Any(x => x.TimeSpan != TimeSpan.Zero);

			if (balanceEqualZero && !hasAbsence)
				return Colors.White;
			if ((balanceMoreThanMore || balanceEqualZero) && hasAbsence)
				return Colors.Pink;
			if (balanceMoreThanMore && !hasAbsence)
				return Colors.LightGreen;
			if (balanceLessThanZero && hasAbsence)
				return Colors.LightCoral;
			if (balanceLessThanZero && !hasAbsence)
				return Colors.DarkRed;

			return Colors.DarkGray;
		}

		private static Color GetColorBaseOnHoliday(TimeTrackTotal balance)
		{
			if (balance.TimeSpan == TimeSpan.Zero)
				return Colors.LightGray;
			if (balance.TimeSpan > TimeSpan.Zero)
				return Colors.DarkGreen;
			if (balance.TimeSpan < TimeSpan.Zero)
				return Colors.DarkRed;

			return Colors.DarkGray;
		}

		/// <summary>
		/// Метод для получения количества ночных часов, которые сотруднику необходимо отработать
		/// </summary>
		/// <returns>Ночное время (в часах)</returns>
		public double GetNightTotalTime()
		{
			if (NightSettings == null || !NightSettings.IsNightSettingsEnabled) return default(double);

			var calcNightIntervals = new List<TimeTrackPart>();
			if (NightSettings.NightEndTime >= NightSettings.NightStartTime)
			{
				calcNightIntervals.Add(
					new TimeTrackPart
					{
						EnterDateTime = new DateTime().Add(NightSettings.NightStartTime),
						ExitDateTime = new DateTime().Add(NightSettings.NightEndTime)
					});
			}
			else
			{
				calcNightIntervals.Add(new TimeTrackPart
				{
					EnterDateTime = new DateTime(),
					ExitDateTime = new DateTime() +  NightSettings.NightEndTime
				});
				calcNightIntervals.Add(new TimeTrackPart
				{
					EnterDateTime = new DateTime() + NightSettings.NightStartTime,
					ExitDateTime = new DateTime() + new TimeSpan(0, 23, 59, 59)
				});
			}

			//Если ночной интервал пересекается с интервалом графика работ,
			//то формируем новый интервал, являющийся пересечением этих интервалов
			var resultIntervals = new List<TimeTrackPart>();
			foreach (var planned in PlannedTimeTrackParts)
			{
				foreach (var nightInterval in calcNightIntervals)
				{
					if (!nightInterval.IsIntersectTimeOfDay(planned)) continue;

					var enterTime = nightInterval.EnterDateTime >= planned.EnterDateTime
						? nightInterval.EnterDateTime
						: planned.EnterDateTime;

					Debug.Assert(planned.ExitDateTime != null, "planned.ExitDateTime != null");
					Debug.Assert(nightInterval.ExitDateTime != null, "nightInterval.ExitDateTime != null");
					var endTime = nightInterval.ExitDateTime >= planned.ExitDateTime
						? planned.ExitDateTime.Value
						: nightInterval.ExitDateTime.Value;

					resultIntervals.Add(new TimeTrackPart { EnterDateTime = enterTime, ExitDateTime = endTime });
				}
			}

			return resultIntervals.Aggregate(default(double), (s, tp) => s + Math.Abs(tp.Delta.TotalHours));
		}

		private List<TimeTrackPart> GetRealTimeTrackPartsForDrawing(List<TimeTrackPart> realTimeTrackParts, bool isOnlyFirstEnter)
		{
			if (!isOnlyFirstEnter) return realTimeTrackParts;

			var realTimeTrackPartsForUrv = realTimeTrackParts.Where(x => x.IsForURVZone).ToList();

			if (!realTimeTrackPartsForUrv.Any()) return realTimeTrackParts;

			return new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = realTimeTrackPartsForUrv.Min(x => x.EnterDateTime),
					ExitDateTime = realTimeTrackPartsForUrv.Max(x => x.ExitDateTime),
					TimeTrackPartType = TimeTrackType.Presence,
					IsForURVZone = true
				}
			};
		}

		private void CalculateNightTimeSpans(NightSettings nightSettings, ref TimeSpan nightTimeForToday)
		{
			_nighTimeSpans = new List<NightSettings>();

			if (nightSettings.NightStartTime < nightSettings.NightEndTime)
			{
				_nighTimeSpans.Add(nightSettings);
				nightTimeForToday = nightSettings.NightEndTime - nightSettings.NightEndTime;
			}
			else if(nightSettings.NightStartTime > nightSettings.NightEndTime)
			{
				var tmp = new NightSettings {NightStartTime = nightSettings.NightStartTime, NightEndTime = new TimeSpan(23, 59, 59)};
				nightTimeForToday = tmp.NightEndTime - tmp.NightStartTime;
				_nighTimeSpans.Add(tmp);
				_nighTimeSpans.Add(new NightSettings{NightStartTime = new TimeSpan(), NightEndTime = nightSettings.NightEndTime});
			}
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

		private List<TimeTrackPart> ApplyDeviationPolitics(IEnumerable<TimeTrackPart> combinedTimeTrackParts, int allowedAbsent, int allowedEarlyLeave, int allowedLate, int notAllowedOvertime)
		{
			var resultCollection = new List<TimeTrackPart>();
			foreach (var combinedTimeTrackPart in combinedTimeTrackParts)
			{
				if (CanApplyDeviationPolitics(combinedTimeTrackPart, allowedAbsent, allowedEarlyLeave, allowedLate, notAllowedOvertime))
				{
					if(combinedTimeTrackPart.TimeTrackPartType == TimeTrackType.Overtime && notAllowedOvertime != default(int))
						continue;

					combinedTimeTrackPart.TimeTrackPartType = TimeTrackType.Presence;
				}

				resultCollection.Add(combinedTimeTrackPart);
			}

			return resultCollection;
		}

		private static bool CanApplyDeviationPolitics(TimeTrackPart timeTrackPart, int allowedAbsent, int allowedEarlyLeave, int allowedLate, int notAllowedOvertime)
		{
			var isApplyForAbsence = timeTrackPart.TimeTrackPartType == TimeTrackType.Absence && timeTrackPart.Delta.TotalMinutes < allowedAbsent && allowedAbsent != default(int);
			var isApplyForEarlyLeave = timeTrackPart.TimeTrackPartType == TimeTrackType.EarlyLeave && timeTrackPart.Delta.TotalMinutes < allowedEarlyLeave && allowedEarlyLeave != default(int);
			var isApplyForLate = timeTrackPart.TimeTrackPartType == TimeTrackType.Late && timeTrackPart.Delta.TotalMinutes < allowedLate && allowedLate != default(int);
			var isApplyForOverTime = timeTrackPart.TimeTrackPartType == TimeTrackType.Overtime && timeTrackPart.Delta.TotalMinutes < notAllowedOvertime && notAllowedOvertime != default(int);

			return isApplyForAbsence || isApplyForEarlyLeave || isApplyForLate || isApplyForOverTime;
		}

		private static bool IsIntersectWithNightSettings(TimeTrackPart timeTrackPart, List<NightSettings> nightSettings, out NightSettings currentNightSetting)
		{
			foreach (var nightSetting in nightSettings)
			{
				if (timeTrackPart.EnterDateTime.TimeOfDay <= nightSetting.NightEndTime
					&& timeTrackPart.ExitDateTime.GetValueOrDefault().TimeOfDay >= nightSetting.NightStartTime)
				{
					currentNightSetting = nightSetting;
					return true;
				}
			}
			currentNightSetting = new NightSettings();
			return false;
		}

		private static List<TimeTrackPart> TransferNightSettings(List<TimeTrackPart> combinedTimeTrackParts, List<NightSettings> nightSettings)
		{
			if (nightSettings == null) return combinedTimeTrackParts;

			var resultCollection = new List<TimeTrackPart>();
			foreach (var el in combinedTimeTrackParts)
			{
				NightSettings currentNightSetting;
				if (el.TimeTrackPartType == TimeTrackType.Presence && IsIntersectWithNightSettings(el, nightSettings, out currentNightSetting))
				{
					var night = new TimeTrackPart {TimeTrackPartType = TimeTrackType.Night};
					//Вычисляем время входа нового интервала ночного времени
					if (el.EnterDateTime.TimeOfDay <= currentNightSetting.NightStartTime)
					{
						night.EnterDateTime = el.EnterDateTime.Date + currentNightSetting.NightStartTime;
					}
					else if (el.EnterDateTime.TimeOfDay > currentNightSetting.NightStartTime)
					{
						night.EnterDateTime = el.EnterDateTime;
					}

					//Вычисляем время выхода нового интервала ночного времени
					if (el.ExitDateTime.GetValueOrDefault().TimeOfDay >= currentNightSetting.NightEndTime)
					{
						night.ExitDateTime = el.ExitDateTime.GetValueOrDefault().Date + currentNightSetting.NightEndTime;
					}
					else if (el.ExitDateTime.GetValueOrDefault().TimeOfDay < currentNightSetting.NightEndTime)
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

		private static List<TimeTrackPart> TransferPresentToOvertime(IEnumerable<TimeTrackPart> combinedTimeTrackParts, TimeSpan slideTime) //TODO: Create tests
		{
			var totalPresentTime = - slideTime;

			var resultCollection = new List<TimeTrackPart>();

			foreach (var el in combinedTimeTrackParts)
			{
				if (el.TimeTrackPartType == TimeTrackType.Presence)
				{
					if (totalPresentTime == default(TimeSpan))
					{
						el.TimeTrackPartType = TimeTrackType.Overtime;
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

		private static TimeSpan GetSummOfPlannedTimeTrackParts(IEnumerable<TimeTrackPart> plannedTimeTrackParts)
		{
			return plannedTimeTrackParts.Aggregate(default(TimeSpan), (current, plannedTimeTrackPart) => current + plannedTimeTrackPart.Delta);
		}

		private static List<TimeTrackPart> AdjustmentCombinedTimeTracks(List<TimeTrackPart> combinedTimeTrackParts, List<TimeTrackPart> plannedTimeTrackParts, TimeSpan slideTime)
		{
			var sumPlannedTime = GetSummOfPlannedTimeTrackParts(plannedTimeTrackParts); //Суммарное время графика

			if (sumPlannedTime < slideTime) return combinedTimeTrackParts;

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
							el.ExitDateTime = el.EnterDateTime - differWithPlannedTime;
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

				if (timeTrackPart.ExitDateTime != null && (timeTrackPart.EnterDateTime.Date == Date && timeTrackPart.ExitDateTime.Value.Date > Date))
				{
					timeTrackPartItem = new TimeTrackPart
					{
						EnterDateTime = timeTrackPart.EnterDateTime,
						ExitDateTime = Date.AddDays(1).AddSeconds(-1), //23.59.59.9999999
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
				else if (timeTrackPart.ExitDateTime != null && (timeTrackPart.EnterDateTime.Date < Date && timeTrackPart.ExitDateTime.Value.Date == Date))
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
				else if (timeTrackPart.ExitDateTime != null && (timeTrackPart.EnterDateTime.Date < Date && timeTrackPart.ExitDateTime.Value.Date > Date))
				{
					timeTrackPartItem = new TimeTrackPart
					{
						EnterDateTime = Date, //00:00
						ExitDateTime = Date.AddDays(1).AddSeconds(-1), //23.59.59.9999999
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
				if (timeTrackPart.ExitDateTime != null)
					timeTrackPart.TimeTrackPartType = GetTimeTrackType(timeTrackPart, plannedTimeTrackParts, realTimeTrackParts, scheduleTimeInterval, new ScheduleInterval(timeTrackPart.EnterDateTime, timeTrackPart.ExitDateTime.Value));

				resultCollection.Add(timeTrackPart);
			}

			return resultCollection;
		}

		/// <summary>
		/// Получает интервал, в пределах которого ведётся расчёт документа, основываясь на времени начала и конца длительности оправдательного документа.
		/// </summary>
		/// <param name="document">Оправдательный документ</param>
		/// <returns>Интервал для расчёта, длительность которого расчитывается на базе интервала длительности оправдательного документа</returns>
		private TimeTrackPart InitializeTimeTrackPartBasedOnDocument(TimeTrackDocument document)
		{
			var isDurationMoreThanOneDay = document.StartDateTime.Date < Date && document.EndDateTime.Date > Date;
			var isDurationTillTomorrow = document.StartDateTime.Date == Date && document.EndDateTime.Date > Date;
			var isDurationFromYesterday = document.StartDateTime.Date < Date && document.EndDateTime.Date == Date;

			if (isDurationMoreThanOneDay)
				return new TimeTrackPart
				{
					EnterDateTime = Date.Date,
					ExitDateTime = Date.AddDays(1).AddTicks(-1)
				};

			if (isDurationTillTomorrow)
				return new TimeTrackPart
				{
					EnterDateTime = document.StartDateTime,
					ExitDateTime = document.StartDateTime.AddDays(1).AddTicks(-1)
				};

			if (isDurationFromYesterday)
				return new TimeTrackPart
				{
					EnterDateTime = Date.Date,
					ExitDateTime = document.EndDateTime
				};

			return new TimeTrackPart
			{
				EnterDateTime = document.StartDateTime,
				ExitDateTime = document.EndDateTime
			};
		}

		private void CalculateDocuments()
		{
			DocumentTrackParts = new List<TimeTrackPart>();
			foreach (var document in Documents)
			{
				var timeTrackPart = InitializeTimeTrackPartBasedOnDocument(document);

				if (timeTrackPart == null) continue;

				timeTrackPart.MinTimeTrackDocumentType = document.TimeTrackDocumentType;
				timeTrackPart.IsOutside = document.IsOutside;
				DocumentTrackParts.Add(timeTrackPart);
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
				var combinedInterval = new ScheduleInterval(combinedTimeSpans[i].Value, combinedTimeSpans[i + 1].Value);

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

				timeTrackPart.TimeTrackPartType = GetTimeTrackType(timeTrackPart, plannedTimeTrackParts, realTimeTrackParts, GetPlannedScheduleInterval(plannedTimeTrackParts), combinedInterval);

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
						return TimeTrackType.DocumentAbsenceReasonable;
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
		public TimeTrackType GetTimeTrackType(TimeTrackPart timeTrackPart, List<TimeTrackPart> plannedTimeTrackParts, List<TimeTrackPart> realTimeTrackParts, ScheduleInterval schedulePlannedInterval, ScheduleInterval combinedInterval)
		{
			var hasRealTimeTrack = realTimeTrackParts
				.Where(x => x.ExitDateTime.HasValue && !x.NotTakeInCalculations && x.IsForURVZone)
				.Any(x => combinedInterval.EndTime != null
				          && (x.ExitDateTime != null
				              && (x.EnterDateTime.TimeOfDay <= combinedInterval.StartTime.TimeOfDay && x.ExitDateTime.Value.TimeOfDay >= combinedInterval.EndTime.Value.TimeOfDay)));

			var hasPlannedTimeTrack = plannedTimeTrackParts
				.Where(x => x.ExitDateTime.HasValue)
				.Any(x =>
					combinedInterval.EndTime != null &&
					(x.ExitDateTime != null &&
					 (x.EnterDateTime.TimeOfDay <= combinedInterval.StartTime.TimeOfDay && x.ExitDateTime.Value.TimeOfDay >= combinedInterval.EndTime.Value.TimeOfDay)));

			//Если есть интервал прохода сотрудника, который попадает в гафик работ, то "Явка"
			if (hasRealTimeTrack && hasPlannedTimeTrack)
				return TimeTrackType.Presence;

			//Если нет интервала проода сотрудника и нет графика, то "Нет данных"
			if (!hasRealTimeTrack && !hasPlannedTimeTrack)
				return TimeTrackType.None;

			//Если есть интервал прохода сотрудника, который не попадает в интервал графика работа, то "Сверхурочно"
			//или, если он попадает в график работ, то "Явка в перерыве"
			if (hasRealTimeTrack)
				return TimeTrackType.Overtime;

			//Если нет интервала прохода сотрудника, но есть интервал рабочего графика
			timeTrackPart.TimeTrackPartType = TimeTrackType.Absence; //Отсутствие

			if (plannedTimeTrackParts.Any(x => x.EnterDateTime.TimeOfDay == timeTrackPart.EnterDateTime.TimeOfDay) && //TODO: describe it
				plannedTimeTrackParts.All(x => x.ExitDateTime.Value.TimeOfDay != timeTrackPart.ExitDateTime.Value.TimeOfDay) &&
				realTimeTrackParts.Any(x => x.EnterDateTime.TimeOfDay == timeTrackPart.ExitDateTime.Value.TimeOfDay))
			{
				var firstPlannedTimeTrack = plannedTimeTrackParts.FirstOrDefault(x => x.EnterDateTime.TimeOfDay == timeTrackPart.ExitDateTime.Value.TimeOfDay);
				if (firstPlannedTimeTrack != null && firstPlannedTimeTrack.StartsInPreviousDay)
					return TimeTrackType.Absence;

				var firstPlannedInterval = plannedTimeTrackParts.FirstOrDefault();
				if (firstPlannedInterval != null && firstPlannedInterval.EnterDateTime.TimeOfDay == timeTrackPart.EnterDateTime.TimeOfDay) //Если расчитываемый интервал - первый, то возможен тип опоздание
					return TimeTrackType.Late;
			}

			if (plannedTimeTrackParts.Any(x => x.EnterDateTime.TimeOfDay == timeTrackPart.EnterDateTime.TimeOfDay) ||		//TODO: describe it
				plannedTimeTrackParts.All(x => x.ExitDateTime.Value.TimeOfDay != timeTrackPart.ExitDateTime.Value.TimeOfDay) ||
				realTimeTrackParts.All(x => x.ExitDateTime.Value.TimeOfDay != timeTrackPart.EnterDateTime.TimeOfDay)) return TimeTrackType.Absence;

			var lastPlannedTimeTrack = plannedTimeTrackParts.FirstOrDefault(x => x.ExitDateTime.Value.TimeOfDay == timeTrackPart.ExitDateTime.Value.TimeOfDay);
			if (lastPlannedTimeTrack != null && lastPlannedTimeTrack.EndsInNextDay)
				return TimeTrackType.Absence;

			return TimeTrackType.EarlyLeave;
		}

		private static List<DateTime?> GetCombinedDateTimes(IEnumerable<TimeTrackPart> realTimeTrackParts, IEnumerable<TimeTrackPart> plannedTimeTrackParts, IEnumerable<TimeTrackPart> documentTimeTrackParts)
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

			var totalBalance = GetBalanceForSlideTime(slideTime, plannedTimeTrackParts);

			foreach (var timeTrack in combinedTimeTrackParts)
			{
				var el = resultTotalCollection.FirstOrDefault(x => x.TimeTrackType == timeTrack.TimeTrackPartType);
				if (el != null) //TODO: Need refactoring

					el.TimeSpan += GetDeltaForTimeTrack(timeTrack, isHoliday);
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
					TimeSpan = GetNightTime(NightSettings, realTimeTrackParts)
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
		private static List<TimeTrackTotal> FillTotalsCollection(List<TimeTrackTotal> inputCollectionTotals)
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
					return timeTrack.Delta;

				case TimeTrackType.Absence:
					return timeTrack.Delta;

				case TimeTrackType.EarlyLeave:
					return timeTrack.Delta;

				case TimeTrackType.DocumentAbsence:
					return (-timeTrack.Delta);
			}

			return default(TimeSpan);
		}

		/// <summary>
		/// Расчитывает баланс для каждого прохода сотрудника.
		/// </summary>
		/// <param name="slideTime">Время скользящего графика в секундах (норма)</param>
		/// <param name="plannedTimeTrackParts">Интервалы рабочего графика</param>
		/// <returns>Баланс, вычисленный на основе скользящего графика</returns>
		private static TimeSpan GetBalanceForSlideTime(TimeSpan slideTime, List<TimeTrackPart> plannedTimeTrackParts) //TODO: refactoring
		{
			const double tolerance = 0.0000001;

			if (Math.Abs(slideTime.TotalSeconds) < tolerance && plannedTimeTrackParts.Any())//если норма не указана и есть график работ
				return plannedTimeTrackParts.Aggregate(default(TimeSpan), (current, plannedTimeTrackPart) => current - plannedTimeTrackPart.Delta);


			if (Math.Abs(slideTime.TotalSeconds) < tolerance)//если норма не указана и нет графика работ.
				return default(TimeSpan);

			var balanceTimeSpan = new TimeSpan();

			//если норма указана и есть график работ
			if (plannedTimeTrackParts.Any())
				balanceTimeSpan = -TimeSpan.FromSeconds(slideTime.TotalSeconds);

			return balanceTimeSpan;
		}

		public TimeSpan GetNightTime(NightSettings nightSettings, List<TimeTrackPart> realTimeTrackParts)
		{
			if (nightSettings == null || nightSettings.NightEndTime == nightSettings.NightStartTime || !nightSettings.IsNightSettingsEnabled) return default(TimeSpan);

			if (nightSettings.NightEndTime > nightSettings.NightStartTime)
			{
				return CalculateEveningTime(nightSettings.NightStartTime, nightSettings.NightEndTime, realTimeTrackParts);
			}

			return CalculateEveningTime(nightSettings.NightStartTime, new TimeSpan(23, 59, 59), realTimeTrackParts) +
				   CalculateEveningTime(new TimeSpan(0, 0, 0), nightSettings.NightEndTime, realTimeTrackParts);
		}

		/// <summary>
		/// Вычисление типа преимущественного интервала, для отображения пользователю в таблице УРВ
		/// </summary>
		/// <param name="totals">Коллекция, хранящая время интервалов всех типов (за один день)</param>
		/// <param name="plannedTimeTrackParts">Время итервалов рабочего графика</param>
		/// <param name="isHoliday">Флаг выходного дня (праздника). true - выходной, false - нет</param>
		/// <param name="error">Текст ошибки</param>
		/// <returns>Возаращает тип интервала, для отображения в таблице УРВ</returns>
		private static TimeTrackType CalculateTimeTrackType(List<TimeTrackTotal> totals, List<TimeTrackPart> plannedTimeTrackParts, bool isHoliday, string error)
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


		private static TimeSpan CalculateEveningTime(TimeSpan start, TimeSpan end, IEnumerable<TimeTrackPart> realTimeTrackParts)
		{
			var result = new TimeSpan();

			if (end <= TimeSpan.Zero) return result;

			foreach (var trackPart in realTimeTrackParts.Where(x => x.ExitDateTime.HasValue && x.IsForURVZone && !x.NotTakeInCalculations && x.TimeTrackPartType == TimeTrackType.Night))
			{
				if (trackPart.ExitDateTime != null && (trackPart.EnterDateTime.TimeOfDay <= start && trackPart.ExitDateTime.Value.TimeOfDay >= end))
				{
					result += end - start;
				}
				else
				{
					if (trackPart.ExitDateTime != null && ((trackPart.EnterDateTime.TimeOfDay >= start && trackPart.EnterDateTime.TimeOfDay <= end) ||
					                                       (trackPart.ExitDateTime.Value.TimeOfDay >= start && trackPart.ExitDateTime.Value.TimeOfDay <= end)))
					{
						var minStartTime = trackPart.EnterDateTime.TimeOfDay < start ? start : trackPart.EnterDateTime.TimeOfDay;
						var minEndTime = trackPart.ExitDateTime.Value.TimeOfDay > end ? end : trackPart.ExitDateTime.Value.TimeOfDay;
						result += minEndTime - minStartTime;
					}
				}
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
			return timeTrackParts
				.Where(x => x.ExitDateTime.HasValue)
				.Where(x => x.ExitDateTime != null && ((x.EnterDateTime.Date == date.Date && x.ExitDateTime.Value.Date > date.Date)
				                                       ||(x.EnterDateTime.Date < date.Date && x.ExitDateTime.Value.Date > date.Date)))
				.ToList();
		}

		/// <summary>
		/// Получение буквенного кода для дня УРВ
		/// </summary>
		private void SetLetterCode()
		{
			if (PlannedTimeTrackParts.Any())
			{
				if (DocumentTrackParts.Any())
					LetterCode = GetMaxDocumentCode();
				else
				{
					var totalAbsence = Totals.Where(x => x.TimeTrackType == TimeTrackType.EarlyLeave
														|| x.TimeTrackType == TimeTrackType.Absence
														|| x.TimeTrackType == TimeTrackType.Late).ToList();

					if (totalAbsence.Any(x => x.TimeSpan != default(TimeSpan)))
					{
						var tmp = totalAbsence.FirstOrDefault(x => x.TimeSpan == totalAbsence.Max(total => total.TimeSpan));
						if (tmp != null)
							LetterCode = GetLetterCodeFromAbcense(tmp.TimeTrackType);
					}
					else
						LetterCode = SlideTime == TimeSpan.Zero
							? PlannedTimeTrackParts.Sum(x => x.Delta.TotalHours).ToString("F")
							: SlideTime.TotalHours.ToString("F");
				}
			}
			else
				LetterCode = DocumentTrackParts.Any() ? GetMaxDocumentCode() : "В";

			if (RealTimeTrackParts.Any(x => x.IsManuallyAdded))
				LetterCode += "*";
			//TODO: Add calculating for Tooltip
		}

		private static string GetLetterCodeFromAbcense(TimeTrackType type)
		{
			switch (type)
			{
				case TimeTrackType.Late:
					return "ОП";
				case TimeTrackType.EarlyLeave:
					return "УР";
				case TimeTrackType.Absence:
					return "НН";
				default:
					return string.Empty;
			}
		}

		private string GetMaxDocumentCode()
		{
			var allDocumentTotals = Totals.Where(x => x.TimeTrackType == TimeTrackType.DocumentAbsence
			                               || x.TimeTrackType == TimeTrackType.DocumentAbsenceReasonable
			                               || x.TimeTrackType == TimeTrackType.DocumentOvertime
			                               || x.TimeTrackType == TimeTrackType.DocumentPresence).ToList();
			var maxDocumentIntervals = allDocumentTotals.Where(x => x.TimeSpan == allDocumentTotals.Max(span => span.TimeSpan));
			var documents = Documents.Where(doc => maxDocumentIntervals.Any(x => x.TimeTrackType == GetTimeTrackTypeFromDocument(doc))).ToList();

			var timeTrackDocument = documents.FirstOrDefault(doc => doc.TimeTrackDocumentType.Code == documents.Min(x => x.TimeTrackDocumentType.Code));

			return timeTrackDocument !=null ? timeTrackDocument.TimeTrackDocumentType.ShortName : string.Empty;
		}

		private static TimeTrackType GetTimeTrackTypeFromDocument(TimeTrackDocument document)
		{
			switch (document.TimeTrackDocumentType.DocumentType)
			{
				case DocumentType.Absence:
					return TimeTrackType.DocumentAbsence;
				case DocumentType.AbsenceReasonable:
					return TimeTrackType.DocumentAbsenceReasonable;
				case DocumentType.Overtime:
					return TimeTrackType.DocumentOvertime;
				case DocumentType.Presence:
					return TimeTrackType.DocumentPresence;
				default:
					return TimeTrackType.None;
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