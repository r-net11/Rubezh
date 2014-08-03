using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using LinqKit;
using SKDDriver.Translators;
using System.Data.SqlTypes;

namespace SKDDriver
{
	public class EmployeeTranslator : WithShortTranslator<DataAccess.Employee, Employee, EmployeeFilter, ShortEmployee>
	{
		public EmployeeTranslator(DataAccess.SKDDataContext context,
			PositionTranslator positionTranslator,
			DepartmentTranslator departmentTranslator,
			AdditionalColumnTranslator additionalColumnTranslator,
			CardTranslator cardTranslator,
			PhotoTranslator photoTranslator,
			ScheduleTranslator scheduleTranslator )
			: base(context)
		{
			PositionTranslator = positionTranslator;
			DepartmentTranslator = departmentTranslator;
			AdditionalColumnTranslator = additionalColumnTranslator;
			CardTranslator = cardTranslator;
			PhotoTranslator = photoTranslator;
			ScheduleTranslator = scheduleTranslator;
		}

		PositionTranslator PositionTranslator;
		DepartmentTranslator DepartmentTranslator;
		AdditionalColumnTranslator AdditionalColumnTranslator;
		CardTranslator CardTranslator;
		PhotoTranslator PhotoTranslator;
		ScheduleTranslator ScheduleTranslator;

		protected override OperationResult CanSave(Employee item)
		{
			bool sameName = Table.Any(x => x.FirstName == item.FirstName &&
				x.SecondName == item.SecondName &&
				x.LastName == item.LastName &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Сотрудник с таким же ФИО уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			bool isAttendant = Context.Departments.Any(x => !x.IsDeleted && x.AttendantUID == uid);
			if (isAttendant)
				return new OperationResult("Невозможно удалить сотрудника, пока он указан как сопровождающий для одного из отделов");

			bool isContactEmployee = Context.Departments.Any(x => !x.IsDeleted && x.ContactEmployeeUID == uid);
			if (isContactEmployee)
				return new OperationResult("Невозможно удалить сотрудника, пока он указан как контактное лицо для одного из отделов");
			return base.CanDelete(uid);
		}

		protected override Employee Translate(DataAccess.Employee tableItem)
		{
			var result = base.Translate(tableItem);
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Appointed = tableItem.Appointed;
			result.Dismissed = tableItem.Dismissed;
			result.Department = DepartmentTranslator.GetSingleShort(tableItem.DepartmentUID);
			result.Schedule = ScheduleTranslator.GetSingleShort(tableItem.ScheduleUID);
			result.ScheduleStartDate = tableItem.ScheduleStartDate;
			result.AdditionalColumns = AdditionalColumnTranslator.GetAllByEmployee<DataAccess.AdditionalColumn>(tableItem.UID);
			result.Type = (PersonType)tableItem.Type;
			result.Cards = CardTranslator.GetByEmployee<DataAccess.Card>(tableItem.UID);
			result.Position = PositionTranslator.GetSingleShort(tableItem.PositionUID);
			result.Photo = GetResult(PhotoTranslator.GetSingle(tableItem.PhotoUID));
			result.TabelNo = tableItem.TabelNo;
			result.CredentialsStartDate = tableItem.CredentialsStartDate;
			result.EscortUID = tableItem.EscortUID;
			result.DocumentNumber = tableItem.DocumentNumber;
			result.BirthDate = tableItem.BirthDate;
			result.BirthPlace = tableItem.BirthPlace;
			result.DocumentGivenBy = tableItem.DocumentGivenBy;
			result.DocumentGivenDate = tableItem.DocumentGivenDate;
			result.DocumentValidTo = tableItem.DocumentValidTo;
			result.Gender = (Gender)tableItem.Gender;
			result.DocumentDepartmentCode = tableItem.DocumentDepartmentCode;
			result.Citizenship = tableItem.Citizenship;
			result.DocumentType = (EmployeeDocumentType)tableItem.DocumentType;
			var guardZones = (from x in Context.GuardZones.Where(x => x.ParentUID == tableItem.UID) select x);
			foreach (var item in guardZones)
			{
				result.GuardZoneAccesses.Add(new XGuardZoneAccess 
					{ 
						ZoneUID = item.ZoneUID, 
						CanReset = item.CanReset, 
						CanSet = item.CanSet 
					});
			}
			return result;
		}

		protected override ShortEmployee TranslateToShort(DataAccess.Employee tableItem)
		{
			var shortEmployee = new ShortEmployee
			{
				UID = tableItem.UID,
				FirstName = tableItem.FirstName,
				SecondName = tableItem.SecondName,
				LastName = tableItem.LastName,
				Cards = CardTranslator.GetByEmployee<DataAccess.Card>(tableItem.UID),
				Type = (PersonType)tableItem.Type,
				Appointed = tableItem.Appointed.ToString("d MMM yyyy"),
				Dismissed = tableItem.Dismissed.ToString("d MMM yyyy"),
				OrganisationUID = tableItem.OrganisationUID,
			};
			var position = Context.Positions.FirstOrDefault(x => x.UID == tableItem.PositionUID);
			if (position != null)
				shortEmployee.PositionName = position.Name;

			var department = Context.Departments.FirstOrDefault(x => x.UID == tableItem.DepartmentUID);
			if (department != null)
				shortEmployee.DepartmentName = department.Name;
			return shortEmployee;
		}

		public override OperationResult<IEnumerable<ShortEmployee>> GetList(EmployeeFilter filter)
		{
			var result = base.GetList(filter);
			if (result.HasError)
				return result;
			return AdditionalColumnTranslator.SetTextColumns(result);
		}
		
		protected override void TranslateBack(DataAccess.Employee tableItem, Employee apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.FirstName = apiItem.FirstName;
			tableItem.SecondName = apiItem.SecondName;
			tableItem.LastName = apiItem.LastName;
			tableItem.Appointed = CheckDate(apiItem.Appointed);
			tableItem.Dismissed = CheckDate(apiItem.Dismissed);
			if (apiItem.Position != null)
				tableItem.PositionUID = apiItem.Position.UID;
			if (apiItem.Department != null)
				tableItem.DepartmentUID = apiItem.Department.UID;
			if(apiItem.Schedule != null)
				tableItem.ScheduleUID = apiItem.Schedule.UID;
			tableItem.ScheduleStartDate = CheckDate(apiItem.ScheduleStartDate);
			if (apiItem.Photo != null)
				tableItem.PhotoUID = apiItem.Photo.UID;
			tableItem.Type = (int)apiItem.Type;
			tableItem.TabelNo = apiItem.TabelNo;
			tableItem.CredentialsStartDate = CheckDate(apiItem.CredentialsStartDate);
			tableItem.EscortUID = apiItem.EscortUID;
			tableItem.DocumentNumber = apiItem.DocumentNumber;
			tableItem.BirthDate = CheckDate(apiItem.BirthDate);
			tableItem.BirthPlace = apiItem.BirthPlace;
			tableItem.DocumentGivenBy = apiItem.DocumentGivenBy;
			tableItem.DocumentGivenDate = CheckDate(apiItem.DocumentGivenDate);
			tableItem.DocumentValidTo = CheckDate(apiItem.DocumentValidTo);
			tableItem.Gender = (int)apiItem.Gender;
			tableItem.DocumentDepartmentCode = apiItem.DocumentDepartmentCode;
			tableItem.Citizenship = apiItem.Citizenship;
			tableItem.DocumentType = (int)apiItem.DocumentType;
		}

		public override OperationResult Save(Employee apiItem)
		{
			var columnSaveResult = AdditionalColumnTranslator.Save(apiItem.AdditionalColumns);
			if (columnSaveResult.HasError)
				return columnSaveResult;
			var zoneSaveResult = SaveGuardZones(apiItem);
			if (zoneSaveResult.HasError)
				return zoneSaveResult;
			if (apiItem.Photo != null)
			{
				var photoSaveResult = PhotoTranslator.Save(apiItem.Photo);
				if (photoSaveResult.HasError)
					return photoSaveResult;
			}
			return base.Save(apiItem);
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(EmployeeFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => e.Type == (int?)filter.PersonType);
			var departmentUIDs = filter.DepartmentUIDs;
			if (departmentUIDs.IsNotNullOrEmpty())
			{
				result = result.And(e => e != null);
			}

			var positionUIDs = filter.PositionUIDs;
			if (positionUIDs.IsNotNullOrEmpty())
				result = result.And(e => e != null && positionUIDs.Contains(e.PositionUID.Value));

			var appointedDates = filter.Appointed;
			if (appointedDates != null)
				result = result.And(e => e.Appointed >= appointedDates.StartDate && e.Appointed <= appointedDates.EndDate);

			var dismissedDates = filter.Dismissed;
			if (dismissedDates != null)
				result = result.And(e => e.Dismissed >= dismissedDates.StartDate && e.Dismissed <= dismissedDates.EndDate);

			if (!string.IsNullOrEmpty(filter.LastName))
				result = result.And(e => e.LastName.Contains(filter.LastName));

			if (!string.IsNullOrEmpty(filter.FirstName))
				result = result.And(e => e.FirstName.Contains(filter.FirstName));

			if (!string.IsNullOrEmpty(filter.SecondName))
				result = result.And(e => e.SecondName.Contains(filter.SecondName));

			if (filter.CardNo > 0)
			{
				result = result.And(e => Context.Cards.Any(x => x.Number == filter.CardNo && x.EmployeeUID == e.UID));
			}

			return result;
		}

		public OperationResult SaveGuardZones(Employee apiItem)
		{
			return SaveGuardZonesInternal(apiItem.UID, apiItem.GuardZoneAccesses);
		}
				
		OperationResult SaveGuardZonesInternal(Guid parentUID, List<XGuardZoneAccess> GuardZones)
		{
			try
			{
				var tableOrganisationGuardZones = Context.GuardZones.Where(x => x.ParentUID == parentUID);
				Context.GuardZones.DeleteAllOnSubmit(tableOrganisationGuardZones);
				foreach (var guardZone in GuardZones)
				{
					var tableOrganisationGuardZone = new DataAccess.GuardZone();
					tableOrganisationGuardZone.UID = Guid.NewGuid();
					tableOrganisationGuardZone.ParentUID = parentUID;
					tableOrganisationGuardZone.ZoneUID = guardZone.ZoneUID;
					tableOrganisationGuardZone.CanSet = guardZone.CanSet;
					tableOrganisationGuardZone.CanReset = guardZone.CanReset;
					Context.GuardZones.InsertOnSubmit(tableOrganisationGuardZone);
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult<List<EmployeeTimeTrack>> GetTimeTracks(Guid employeeUID, DateTime startDate, DateTime endDate)
		{
			try
			{
				var timeTracks = new List<EmployeeTimeTrack>();
				for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
				{
					timeTracks.Add(GetTimeTrack(employeeUID, date));
				}
				return new OperationResult<List<EmployeeTimeTrack>> { Result = timeTracks };
			}
			catch (Exception e)
			{
				return new OperationResult<List<EmployeeTimeTrack>>(e.Message);
			}
		}

		public OperationResult AddPassJournal(Guid employeeUID, Guid zoneUID)
		{
			try
			{
				var exitPassJournal = Context.PassJournals.FirstOrDefault(x => x.EmployeeUID == employeeUID && x.ExitTime == SqlDateTime.MinValue.Value);
				if (exitPassJournal != null)
				{
					exitPassJournal.ExitTime = DateTime.Now;
				}
				var enterPassJournal = new DataAccess.PassJournal();
				enterPassJournal.UID = Guid.NewGuid();
				enterPassJournal.EmployeeUID = employeeUID;
				enterPassJournal.ZoneUID = zoneUID;
				enterPassJournal.EnterTime = DateTime.Now;
				enterPassJournal.ExitTime = SqlDateTime.MinValue.Value;
				Context.PassJournals.InsertOnSubmit(enterPassJournal);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		EmployeeTimeTrack GetTimeTrack(Guid employeeUID, DateTime date)
		{
			var passJournals = Context.PassJournals.Where(x => x.EmployeeUID == employeeUID && x.EnterTime.Date == date.Date).ToList();
			if (passJournals == null || passJournals.Count == 0)
				return new EmployeeTimeTrack();
			var timeTrack = new EmployeeTimeTrack();
			timeTrack.EmployeeUID = employeeUID;
			timeTrack.Date = date;
			var firstEnterTime = passJournals.Select(x => x.EnterTime).Min();
			var lastExitTime = passJournals.Select(x => x.ExitTime).Max();
			var totalNotMiss = new DateTime();
			foreach (var item in passJournals)
			{
				totalNotMiss = new DateTime(totalNotMiss.Ticks + item.ExitTime.Ticks - item.EnterTime.Ticks);
			}

			var employee = Table.FirstOrDefault(x => x.UID == employeeUID);
			var schedule = Context.Schedules.FirstOrDefault(x => x.UID == employee.ScheduleUID);
			if(schedule == null)
				return new EmployeeTimeTrack();
			var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(x => x.UID == schedule.ScheduleSchemeUID.Value);
			if (scheduleScheme == null)
				return new EmployeeTimeTrack();
			var scheduleSchemeType = (ScheduleSchemeType)scheduleScheme.Type;

			var days = Context.Days.Where(x => x.ScheduleSchemeUID == scheduleScheme.UID && !x.IsDeleted);
			if (days == null || days.Count() == 0)
				return new EmployeeTimeTrack();
			int dayNo = -1;

			switch (scheduleSchemeType)
			{
				case ScheduleSchemeType.Week:
					dayNo = (int)date.DayOfWeek;
					break;
				case ScheduleSchemeType.Shift:
					var daysCount = days.Count();
					var period = new TimeSpan(date.Ticks - employee.ScheduleStartDate.Ticks);
					dayNo = (int)Math.IEEERemainder((int)period.TotalDays, daysCount);
					break;
				case ScheduleSchemeType.Month:
					dayNo = (int)date.Day;
					break;
			}
			var day = days.FirstOrDefault(x => x.Number == dayNo && !x.IsDeleted);
			if (day == null)
				return new EmployeeTimeTrack();
			var nIs = Context.NamedIntervals.ToList();
			var namedInterval = Context.NamedIntervals.FirstOrDefault(x => x.UID == day.NamedIntervalUID && !x.IsDeleted);
			if (namedInterval == null)
				return new EmployeeTimeTrack();
			var intervals = Context.Intervals.Where(x => x.NamedIntervalUID == namedInterval.UID && !x.IsDeleted);
			var totalInSchedule = new TimeSpan();
			var scheduleZones = Context.ScheduleZones.Where(x => x.ScheduleUID == schedule.UID).Select(x => x.ZoneUID).ToList();
			foreach (var interval in intervals)
			{
				foreach (var passJournal in passJournals)
				{
					if (!scheduleZones.Any(x => x == passJournal.ZoneUID))
						continue;
					var enterTimeSpan = new TimeSpan(passJournal.EnterTime.TimeOfDay.Ticks);
					var exitTimeSpan = new TimeSpan(passJournal.ExitTime.TimeOfDay.Ticks);
					if (enterTimeSpan.TotalSeconds >= interval.BeginTime)
					{
						if (exitTimeSpan.TotalSeconds <= interval.EndTime)
							totalInSchedule.Add(new TimeSpan(exitTimeSpan.Ticks - enterTimeSpan.Ticks));
						else
							totalInSchedule.Add(new TimeSpan(Math.BigMul(interval.EndTime, 10000000) - enterTimeSpan.Ticks));
					}
					else if (exitTimeSpan.TotalSeconds <= interval.EndTime)
					{
						if (enterTimeSpan.TotalSeconds >= interval.BeginTime)
							totalInSchedule.Add(new TimeSpan(exitTimeSpan.Ticks - enterTimeSpan.Ticks));
						else
						{
							totalInSchedule.Add(new TimeSpan(exitTimeSpan.Ticks - Math.BigMul(interval.BeginTime, 10000000)));
						}
					}
				}
			}

			timeTrack.Total = new DateTime(lastExitTime.Ticks - firstEnterTime.Ticks);
			timeTrack.TotalMiss = new DateTime(timeTrack.Total.Ticks - totalNotMiss.Ticks);
			timeTrack.TotalInSchedule = new DateTime(totalInSchedule.Ticks);
			timeTrack.TotalOutSchedule = new DateTime(timeTrack.Total.Ticks - timeTrack.TotalInSchedule.Ticks);
			return timeTrack;
		}
	}
}