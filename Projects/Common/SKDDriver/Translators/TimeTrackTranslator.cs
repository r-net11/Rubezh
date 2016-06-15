using System.Diagnostics;
using StrazhAPI;
using StrazhAPI.SKD;
using Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using StrazhDAL.DataAccess;
using TimeTrackDocumentType = StrazhAPI.SKD.TimeTrackDocumentType;
using System.Threading.Tasks;

namespace StrazhDAL
{
	public class TimeTrackTranslator
	{
		private SKDDatabaseService DatabaseService;
		private DataAccess.SKDDataContext Context;

		private IEnumerable<DataAccess.Holiday> Holidays { get; set; }

		public TimeTrackTranslator(SKDDatabaseService databaseService)
		{
			DatabaseService = databaseService;
			Context = databaseService.Context;
		}

		private PassJournalTranslator _PassJournalTranslator;
		private List<DataAccess.Employee> _Employees;
		private List<DataAccess.Holiday> _Holidays;
		private List<DataAccess.Schedule> _Schedules;
		private List<DataAccess.ScheduleScheme> _ScheduleSchemes;
		private List<DataAccess.ScheduleDay> _ScheduleDays;
		private List<DataAccess.ScheduleZone> _ScheduleZones;
		private List<DataAccess.PassJournal> _PassJournals;
		private List<DataAccess.DayInterval> _DayIntervals;
		private List<DataAccess.DayIntervalPart> _DayIntervalParts;
		private List<DataAccess.TimeTrackDocument> _TimeTrackDocuments;
		private List<DataAccess.TimeTrackDocumentType> _TimeTrackDocumentTypes;
		private List<DataAccess.NightSetting> _NightSettings;

		private void InitializeData()
		{
			_PassJournalTranslator = DatabaseService.PassJournalTranslator;
			_Employees = Context.Employees.Where(x => !x.IsDeleted).ToList();
			_Holidays = Context.Holidays.Where(x => !x.IsDeleted).ToList();
			_Schedules = Context.Schedules.Where(x => !x.IsDeleted).ToList();
			_ScheduleSchemes = Context.ScheduleSchemes.Where(x => !x.IsDeleted).ToList();
			_ScheduleDays = Context.ScheduleDays.ToList();
			_ScheduleZones = Context.ScheduleZones.ToList();
			_PassJournals = _PassJournalTranslator.Context.PassJournals.ToList();
			_DayIntervals = Context.DayIntervals.ToList();
			_DayIntervalParts = Context.DayIntervalParts.ToList();
			_TimeTrackDocuments = Context.TimeTrackDocuments.ToList();
			_TimeTrackDocumentTypes = Context.TimeTrackDocumentTypes.ToList();
			_NightSettings = Context.NightSettings.ToList();
		}

		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			InitializeData();
		//	_PassJournalTranslator.InvalidatePassJournal();

			Holidays = filter.OrganisationUIDs.IsNotNullOrEmpty()
				? _Holidays.Where(x => x.Date >= startDate && x.Date <= endDate && filter.OrganisationUIDs.Contains(x.OrganisationUID.Value)).ToList()
				: _Holidays.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

			try
			{
				var operationResult = DatabaseService.EmployeeTranslator.GetList(filter);
				if (operationResult.HasError)
					return OperationResult<TimeTrackResult>.FromError(operationResult.Errors);

				var timeTrackResult = new TimeTrackResult();
				foreach (var shortEmployee in operationResult.Result)
				{
					var timeTrackEmployeeResult = GetEmployeeTimeTrack(shortEmployee, startDate, endDate);
					timeTrackEmployeeResult.ShortEmployee = shortEmployee;
					if (timeTrackEmployeeResult.Error != null)
					{
						for (var date = startDate; date <= endDate; date = date.AddDays(1))
						{
							timeTrackEmployeeResult.DayTimeTracks.Add(new DayTimeTrack { Error = timeTrackEmployeeResult.Error, Date = date });
						}
					}

					var documentTypes = GetAllDocumentTypes(shortEmployee);

					if (documentTypes == null) continue;

					var documentsOperationResult = DatabaseService.TimeTrackDocumentTranslator.GetWithTypes(shortEmployee, startDate, endDate, _TimeTrackDocuments, documentTypes);

					if (!documentsOperationResult.HasError)
					{
						var documents = documentsOperationResult.Result;
						foreach (var document in documents.Where(document => document.TimeTrackDocumentType != null))
						{
							timeTrackEmployeeResult.Documents.Add(document);
						}

						foreach (var document in timeTrackEmployeeResult.Documents)
						{
							for (var date = document.StartDateTime; date < new DateTime(document.EndDateTime.Year, document.EndDateTime.Month, document.EndDateTime.Day).AddDays(1); date = date.AddDays(1))
							{
								var dayTimeTracks = timeTrackEmployeeResult.DayTimeTracks.FirstOrDefault(x => x.Date.Date == date.Date);
								if (dayTimeTracks != null)
								{
									dayTimeTracks.Documents.Add(document);
								}
							}
						}
					}

					timeTrackResult.TimeTrackEmployeeResults.Add(timeTrackEmployeeResult);
				}
				return new OperationResult<TimeTrackResult>(timeTrackResult);
			}
			catch (Exception e)
			{
				return OperationResult<TimeTrackResult>.FromError(e.Message);
			}
		}

		private IEnumerable<TimeTrackDocumentType> GetAllDocumentTypes(ShortEmployee shortEmployee)
		{
			var organisationTypesTask = DatabaseService.TimeTrackDocumentTypeTranslator.Get(shortEmployee.OrganisationUID);

			if (organisationTypesTask.Result == null)
				return null;

			return organisationTypesTask.Result;
		}

		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var watch = new Stopwatch();
			watch.Start();
			var timeTracksResult = GetTimeTracks(filter, startDate, endDate).Result;
			watch.Stop();
			var serializer = new DataContractSerializer(typeof(TimeTrackResult));
			var folderName = AppDataFolderHelper.GetFolder("TempServer");
			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);
			var fileName = Path.Combine(folderName, "TimeTrackResult.xml");
			watch.Restart();
			using (var fileStream = File.Open(fileName, FileMode.Create))
			{
				serializer.WriteObject(fileStream, timeTracksResult);
			}
			watch.Stop();
			return new FileStream(fileName, FileMode.Open, FileAccess.Read);
		}

		private TimeTrackEmployeeResult GetEmployeeTimeTrack(ShortEmployee shortEmployee, DateTime startDate, DateTime endDate)
		{
			var employee = _Employees.FirstOrDefault(x => x.UID == shortEmployee.UID);
			if (employee == null)
				return new TimeTrackEmployeeResult("Не найден сотрудник");

			var schedule = _Schedules.FirstOrDefault(x => x.UID == employee.ScheduleUID);
			if (schedule == null)
				return new TimeTrackEmployeeResult("Не найден график");

			if (schedule.ScheduleSchemeUID == null)
				return new TimeTrackEmployeeResult("Не найдена схема работы");

			var scheduleScheme = _ScheduleSchemes.FirstOrDefault(x => x.UID == schedule.ScheduleSchemeUID.Value);
			if (scheduleScheme == null)
				return new TimeTrackEmployeeResult("Не найдена схема работы");

			var days = _ScheduleDays.Where(x => x.ScheduleSchemeUID == scheduleScheme.UID).ToList();
			var nightSettings = DatabaseService.NightSettingsTranslator.GetByOrganisation(employee.OrganisationUID.Value, _NightSettings).Result;

			var timeTrackEmployeeResult = new TimeTrackEmployeeResult { ScheduleName = schedule.Name };

			for (var date = startDate; date <= endDate; date = date.AddDays(1))
			{
				if (employee.ScheduleStartDate.Date > date.Date)
				{
					timeTrackEmployeeResult.DayTimeTracks.Add(new DayTimeTrack("До начала действия графика") { Date = date });
					continue;
				}

				var dayTimeTrack = _PassJournalTranslator.GetRealTimeTrack(employee, schedule, scheduleScheme, date, _PassJournals);
				dayTimeTrack.NightSettings = nightSettings;
				dayTimeTrack.IsIgnoreHoliday = schedule.IsIgnoreHoliday;
				dayTimeTrack.IsOnlyFirstEnter = schedule.IsOnlyFirstEnter;
				dayTimeTrack.AllowedLate = schedule.AllowedLate;
				dayTimeTrack.AllowedEarlyLeave = schedule.AllowedEarlyLeave;
				dayTimeTrack.AllowedAbsentLowThan = schedule.AllowedAbsentLowThan;
				dayTimeTrack.NotAllowOvertimeLowerThan = schedule.NotAllowOvertimeLowerThan;

				var realDate = date;
				var ignoreHolidays = false;
				var holiday = Holidays.FirstOrDefault(x => x.Date == date && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
				if (holiday != null && !schedule.IsIgnoreHoliday)
				{
					if (holiday.TransferDate.HasValue)
					{
						realDate = holiday.TransferDate.Value;
						ignoreHolidays = true;
					}
				}

				var plannedTimeTrackPart = GetPlannedTimeTrackPart(employee, schedule, scheduleScheme, days, realDate, ignoreHolidays);
				dayTimeTrack.PlannedTimeTrackParts = plannedTimeTrackPart.TimeTrackParts;
				dayTimeTrack.IsHoliday = plannedTimeTrackPart.IsHoliday;
				dayTimeTrack.HolidayReduction = plannedTimeTrackPart.HolidayReduction;
				dayTimeTrack.SlideTime = plannedTimeTrackPart.SlideTime;
				dayTimeTrack.Error = plannedTimeTrackPart.Error;

				timeTrackEmployeeResult.DayTimeTracks.Add(dayTimeTrack);
			}
			return timeTrackEmployeeResult;
		}

		private PlannedTimeTrackPart GetPlannedTimeTrackPart(DataAccess.Employee employee, DataAccess.Schedule schedule, DataAccess.ScheduleScheme scheduleScheme, IEnumerable<ScheduleDay> days, DateTime date, bool ignoreHolidays)
		{
			var scheduleSchemeType = (ScheduleSchemeType)scheduleScheme.Type;

			var dayNo = -1;
			switch (scheduleSchemeType)
			{
				case ScheduleSchemeType.Week:
					dayNo = (int)date.DayOfWeek - 1;
					if (dayNo == -1)
						dayNo = 6;
					break;

				case ScheduleSchemeType.SlideDay:
					var daysCount = days.Count();
					var ticksDelta = new TimeSpan(date.Date.Ticks - employee.ScheduleStartDate.Date.Ticks);
					var daysDelta = Math.Abs((int)ticksDelta.TotalDays);
					dayNo = daysDelta % daysCount;
					break;

				case ScheduleSchemeType.Month:
					dayNo = date.Day - 1;
					break;
			}

			var day = days.FirstOrDefault(x => x.Number == dayNo);
			if (day == null)
				return new PlannedTimeTrackPart("Не найден день");

			var intervals = new List<DataAccess.DayIntervalPart>();
			DataAccess.DayInterval dayInterval = null;
			if (day.DayIntervalUID != null)
			{
				dayInterval = _DayIntervals.FirstOrDefault(x => x.UID == day.DayIntervalUID && !x.IsDeleted);
				if (dayInterval == null)
					return new PlannedTimeTrackPart("Не найден дневной интервал");
				intervals = _DayIntervalParts.Where(x => x.DayIntervalUID == dayInterval.UID).ToList();
			}

			TimeTrackPart nightTimeTrackPart = null;
			// Для дней графика отличных от первого учитываем хвосты дневных графиков с переходом из предыдущего дня
			if (date != employee.ScheduleStartDate.Date)
			{
				var previousDay = dayNo > 0
					? days.FirstOrDefault(x => x.Number == dayNo - 1)
					: days.FirstOrDefault(x => x.Number == days.Count() - 1);

				if (previousDay != null)
				{
					if (previousDay.DayIntervalUID != null)
					{
						var previousDayInterval = _DayIntervals.FirstOrDefault(x => x.UID == previousDay.DayIntervalUID && !x.IsDeleted);
						if (previousDayInterval != null)
						{
							var previousIntervals = _DayIntervalParts.Where(x => x.DayIntervalUID == previousDayInterval.UID).ToList();
							var nightInterval = previousIntervals.FirstOrDefault(x => x.EndTime > 60 * 60 * 24); //total seconds of day
							if (nightInterval != null)
							{
								nightTimeTrackPart = new TimeTrackPart
								{
									EnterDateTime = new DateTime() + new TimeSpan(),  //new TimeSpan(),
									ExitDateTime = new DateTime() + TimeSpan.FromSeconds(nightInterval.EndTime - 60*60*24),
									StartsInPreviousDay = true,
									DayName = previousDayInterval.Name
								};
							}
						}
					}
				}
			}

			var result = new PlannedTimeTrackPart();
			if (dayInterval != null)
				result.SlideTime = TimeSpan.FromSeconds(dayInterval.SlideTime);

			if (!ignoreHolidays)
			{
				if (!schedule.IsIgnoreHoliday)
				{
					var holiday = Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.Holiday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
					if (holiday != null)
					{
						result.IsHoliday = true;
					}
					holiday = Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.BeforeHoliday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
					if (holiday != null)
					{
						result.HolidayReduction = holiday.Reduction;
					}
					holiday = Holidays.FirstOrDefault(x => x.TransferDate == date && x.Type == (int)HolidayType.WorkingHoliday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
					if (holiday != null)
					{
						result.IsHoliday = true;
					}
				}
			}

			if (!result.IsHoliday)
			{
				foreach (var tableInterval in intervals)
				{
					var timeTrackPart = new TimeTrackPart
					{
						EnterDateTime = new DateTime() + TimeSpan.FromSeconds(tableInterval.BeginTime),
						ExitDateTime = new DateTime() + TimeSpan.FromSeconds(Math.Min(tableInterval.EndTime, 60*60*24 - 1)),
						TimeTrackPartType = ((DayIntervalPartType)tableInterval.Type == DayIntervalPartType.Break) ? TimeTrackType.Break : TimeTrackType.Presence
					};
					if (tableInterval.EndTime > 60 * 60 * 24)
						timeTrackPart.EndsInNextDay = true;
					if (dayInterval != null)
						timeTrackPart.DayName = dayInterval.Name;
					result.TimeTrackParts.Add(timeTrackPart);
				}
				if (nightTimeTrackPart != null)
				{
					result.TimeTrackParts.Add(nightTimeTrackPart);
				}
				result.TimeTrackParts = result.TimeTrackParts.OrderBy(x => x.EnterDateTime.Ticks).ToList();
			}
			if (result.HolidayReduction > 0)
			{
				var lastTimeTrack = result.TimeTrackParts.LastOrDefault();
				if (lastTimeTrack != null && lastTimeTrack.ExitDateTime.HasValue)
				{
					var reductionTimeSpan = TimeSpan.FromSeconds(result.HolidayReduction);
					if (lastTimeTrack.Delta.TotalHours > reductionTimeSpan.TotalHours)
					{
						lastTimeTrack.ExitDateTime = lastTimeTrack.ExitDateTime.Value.Subtract(reductionTimeSpan);
					}
					else
					{
						result.TimeTrackParts.Remove(lastTimeTrack);
					}

					if(result.SlideTime  > TimeSpan.Zero) //если используется расчёт по норме
						result.SlideTime -= reductionTimeSpan;
				}
			}

			return result;
		}
	}
}