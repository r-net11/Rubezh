using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;
using SKDDriver.Translators;

namespace SKDDriver
{
	public class EmployeeTranslator : WithShortTranslator<DataAccess.Employee, Employee, EmployeeFilter, ShortEmployee>
	{
		public EmployeeTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
			Synchroniser = new EmployeeSynchroniser(Table, databaseService);
		}

		protected override OperationResult CanSave(Employee employee)
		{
			var result = base.CanSave(employee);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.FirstName == employee.FirstName &&
				x.SecondName == employee.SecondName &&
				x.LastName == employee.LastName &&
				x.OrganisationUID == employee.OrganisationUID &&
				x.UID != employee.UID &&
				x.IsDeleted == false);
			if (hasSameName)
				return new OperationResult("Сотрудник с таким же ФИО уже содержится в базе данных");
			else
				return new OperationResult();
		}

		protected override Employee Translate(DataAccess.Employee tableItem)
		{
			var result = base.Translate(tableItem);
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Description = tableItem.Description;
			result.Department = DatabaseService.DepartmentTranslator.GetSingleShort(tableItem.DepartmentUID);
			result.Schedule = DatabaseService.ScheduleTranslator.GetSingleShort(tableItem.ScheduleUID);
			result.ScheduleStartDate = tableItem.ScheduleStartDate;
			result.AdditionalColumns = DatabaseService.AdditionalColumnTranslator.GetAllByEmployee<DataAccess.AdditionalColumn>(tableItem.UID);
			result.Type = (PersonType)tableItem.Type;
			result.Cards = DatabaseService.CardTranslator.GetAllByEmployee<DataAccess.Card>(tableItem.UID);
			result.Position = DatabaseService.PositionTranslator.GetSingleShort(tableItem.PositionUID);
			result.Photo = GetResult(DatabaseService.PhotoTranslator.GetSingle(tableItem.PhotoUID));
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
			result.Phone = tableItem.Phone;
			result.LastEmployeeDayUpdate = tableItem.LastEmployeeDayUpdate;
			return result;
		}

		protected override ShortEmployee TranslateToShort(DataAccess.Employee tableItem)
		{
			var result = base.TranslateToShort(tableItem);
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Description = tableItem.Description;
			result.Cards = DatabaseService.CardTranslator.GetAllByEmployee<DataAccess.Card>(tableItem.UID);
			result.Type = (PersonType)tableItem.Type;
			result.CredentialsStartDate = tableItem.CredentialsStartDate.ToString("d MMM yyyy");
			result.TabelNo = tableItem.TabelNo;
			result.TextColumns = DatabaseService.AdditionalColumnTranslator.GetTextColumns(tableItem.UID);
			result.Phone = tableItem.Phone;
			result.LastEmployeeDayUpdate = tableItem.LastEmployeeDayUpdate;
			var position = Context.Positions.FirstOrDefault(x => x.UID == tableItem.PositionUID);
			if (position != null)
			{
				result.PositionName = position.Name;
				result.IsPositionDeleted = position.IsDeleted;
			}
			var department = Context.Departments.FirstOrDefault(x => x.UID == tableItem.DepartmentUID);
			if (department != null)
			{
				result.DepartmentName = department.Name;
				result.IsDepartmentDeleted = department.IsDeleted;
			}
			var organisation = Context.Organisations.FirstOrDefault(x => x.UID == tableItem.OrganisationUID);
			if (organisation != null)
				result.OrganisationName = organisation.Name;
			var schedule = Context.Schedules.FirstOrDefault(x => x.UID == tableItem.ScheduleUID);
			result.ScheduleUID = schedule != null ? schedule.UID : Guid.Empty;
			return result;
		}

		protected override void TranslateBack(DataAccess.Employee tableItem, Employee apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.FirstName = apiItem.FirstName;
			tableItem.SecondName = apiItem.SecondName;
			tableItem.LastName = apiItem.LastName;
			tableItem.Description = apiItem.Description;
			tableItem.PositionUID = apiItem.Position != null ? apiItem.Position.UID : Guid.Empty;
			tableItem.DepartmentUID = apiItem.Department != null ? apiItem.Department.UID : Guid.Empty;
			tableItem.ScheduleUID = apiItem.Schedule != null ? apiItem.Schedule.UID : Guid.Empty;
			tableItem.ScheduleStartDate = TranslatiorHelper.CheckDate(apiItem.ScheduleStartDate);
			tableItem.PhotoUID = apiItem.Photo != null ? apiItem.Photo.UID : Guid.Empty;
			tableItem.Type = (int)apiItem.Type;
			tableItem.TabelNo = apiItem.TabelNo;
			tableItem.CredentialsStartDate = TranslatiorHelper.CheckDate(apiItem.CredentialsStartDate);
			tableItem.EscortUID = apiItem.EscortUID;
			tableItem.DocumentNumber = apiItem.DocumentNumber;
			tableItem.BirthDate = TranslatiorHelper.CheckDate(apiItem.BirthDate);
			tableItem.BirthPlace = apiItem.BirthPlace;
			tableItem.DocumentGivenBy = apiItem.DocumentGivenBy;
			tableItem.DocumentGivenDate = TranslatiorHelper.CheckDate(apiItem.DocumentGivenDate);
			tableItem.DocumentValidTo = TranslatiorHelper.CheckDate(apiItem.DocumentValidTo);
			tableItem.Gender = (int)apiItem.Gender;
			tableItem.DocumentDepartmentCode = apiItem.DocumentDepartmentCode;
			tableItem.Citizenship = apiItem.Citizenship;
			tableItem.DocumentType = (int)apiItem.DocumentType;
			tableItem.Phone = apiItem.Phone;
			tableItem.LastEmployeeDayUpdate = TranslatiorHelper.CheckDate(apiItem.LastEmployeeDayUpdate);
			if (tableItem.ExternalKey == null)
				tableItem.ExternalKey = "-1";
		}

		public override OperationResult Save(Employee apiItem)
		{
			var columnSaveResult = DatabaseService.AdditionalColumnTranslator.Save(apiItem.AdditionalColumns);
			if (columnSaveResult.HasError)
				return columnSaveResult;
			if (apiItem.Photo != null && apiItem.Photo.Data != null && apiItem.Photo.Data.Count() > 0)
			{
				var photoSaveResult = DatabaseService.PhotoTranslator.Save(apiItem.Photo);
				if (photoSaveResult.HasError)
					return photoSaveResult;
			}
			return base.Save(apiItem);
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(EmployeeFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => e.Type == (int?)filter.PersonType);

			if (filter.DepartmentUIDs.IsNotNullOrEmpty())
			{
				if (filter.WithDeletedDepartments)
				{
					result = result.And(e => filter.DepartmentUIDs.Contains(e.DepartmentUID.Value) || Context.Departments.Any(x => x.IsDeleted && x.UID == e.DepartmentUID));
				}
				else
				{
					result = result.And(e => e != null && filter.DepartmentUIDs.Contains(e.DepartmentUID.Value));
					result = result.And(e => e != null);
				}
			}

			if (filter.PositionUIDs.IsNotNullOrEmpty())
			{
				if (filter.WithDeletedPositions)
					result = result.And(e => filter.PositionUIDs.Contains(e.PositionUID.Value) || Context.Positions.Any(x => x.IsDeleted && x.UID == e.PositionUID));
				else
					result = result.And(e => e != null && filter.PositionUIDs.Contains(e.PositionUID.Value));
			}

			if (filter.ScheduleUIDs.IsNotNullOrEmpty())
				result = result.And(e => e != null && filter.ScheduleUIDs.Contains(e.ScheduleUID.Value));

			if (!string.IsNullOrEmpty(filter.LastName))
				result = result.And(e => e.LastName.Contains(filter.LastName));

			if (!string.IsNullOrEmpty(filter.FirstName))
				result = result.And(e => e.FirstName.Contains(filter.FirstName));

			if (!string.IsNullOrEmpty(filter.SecondName))
				result = result.And(e => e.SecondName.Contains(filter.SecondName));

			return result;
		}

		public OperationResult SaveDepartment(Guid uid, Guid departmentUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				tableItem.DepartmentUID = departmentUID;
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SavePosition(Guid uid, Guid positionUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				tableItem.PositionUID = positionUID;
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult GenerateTestData()
		{
			try
			{
				for (int i = 0; i < 10; i++)
				{
					var org = new DataAccess.Organisation { Name = "Организация " + i, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey="-1" };
					Context.Organisations.InsertOnSubmit(org);
					var posUIDs = new List<Guid>();
					for (int j = 0; j < 10; j++)
					{
						var pos = new DataAccess.Position { Name = "Должность " + i + j, OrganisationUID = org.UID, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
						Context.Positions.InsertOnSubmit(pos);
						posUIDs.Add(pos.UID);
					}
					for (int j = 0; j < 10; j++)
					{
						var dept = new DataAccess.Department { Name = "Подразделение " + i + j + "0", OrganisationUID = org.UID, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
						Context.Departments.InsertOnSubmit(dept);
						for (int k = 0; k < 100; k++)
						{
							var empl = new DataAccess.Employee
							{
								LastName = "Фамилия " + i + j + k + "0",
								FirstName = "Имя " + i + j + k + "0",
								SecondName = "Отчество " + i + j + k + "0",
								DepartmentUID = dept.UID,
								PositionUID = posUIDs.FirstOrDefault(),
								OrganisationUID = org.UID,
								UID = Guid.NewGuid(),
								RemovalDate = new DateTime(1900, 1, 1),
								BirthDate = new DateTime(1900, 1, 1),
								CredentialsStartDate = new DateTime(1900, 1, 1),
								DocumentGivenDate = new DateTime(1900, 1, 1),
								DocumentValidTo = new DateTime(1900, 1, 1),
								LastEmployeeDayUpdate = new DateTime(1900, 1, 1),
								ScheduleStartDate = new DateTime(1900, 1, 1),
								ExternalKey = "-1", 
								Type = 0
							};
							Context.Employees.InsertOnSubmit(empl);
						}
						var dept2 = new DataAccess.Department { Name = "Подразделение " + i + j + "1", OrganisationUID = org.UID, ParentDepartmentUID = dept.UID, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
						Context.Departments.InsertOnSubmit(dept2);
						for (int k = 0; k < 100; k++)
						{
							var empl = new DataAccess.Employee
							{
								LastName = "Фамилия " + i + j + k + "1",
								FirstName = "Имя " + i + j + k + "1",
								SecondName = "Отчество " + i + j + k + "1",
								DepartmentUID = dept2.UID,
								PositionUID = posUIDs.LastOrDefault(),
								OrganisationUID = org.UID,
								UID = Guid.NewGuid(),
								RemovalDate = new DateTime(1900, 1, 1),
								BirthDate = new DateTime(1900, 1, 1),
								CredentialsStartDate = new DateTime(1900, 1, 1),
								DocumentGivenDate = new DateTime(1900, 1, 1),
								DocumentValidTo = new DateTime(1900, 1, 1),
								LastEmployeeDayUpdate = new DateTime(1900, 1, 1),
								ScheduleStartDate = new DateTime(1900, 1, 1),
								ExternalKey = "-1",
								Type = 0
							};
							Context.Employees.InsertOnSubmit(empl);
						}
					}
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult GenerateEmployeeDays()
		{
			try
			{
				var result = new List<EmployeeDay>();
				var employees = Table.Where(x => !x.IsDeleted);
				foreach (var employee in employees)
				{

					var employeeDay = new EmployeeDay();
					employeeDay.EmployeeUID = employee.UID;
					var schedule = Context.Schedules.FirstOrDefault(x => x.UID == employee.ScheduleUID);
					if (schedule != null)
					{
						employeeDay.IsIgnoreHoliday = schedule.IsIgnoreHoliday;
						employeeDay.IsOnlyFirstEnter = schedule.IsOnlyFirstEnter;
						employeeDay.AllowedLate = schedule.AllowedLate;
						employeeDay.AllowedEarlyLeave = schedule.AllowedEarlyLeave;
						employeeDay.Date = DateTime.Now;
						var scheduleScheme = schedule.ScheduleScheme;
						if(scheduleScheme != null)
						{
							var scheduleSchemeType = (ScheduleSchemeType)scheduleScheme.Type;
							int dayNo = -1;
							switch (scheduleSchemeType)
							{
								case ScheduleSchemeType.Week:
									dayNo = (int)employeeDay.Date.DayOfWeek - 1;
									if (dayNo == -1)
										dayNo = 6;
									break;
								case ScheduleSchemeType.SlideDay:
									var ticksDelta = new TimeSpan(employeeDay.Date.Ticks - employee.ScheduleStartDate.Date.Ticks);
									var daysDelta = Math.Abs((int)ticksDelta.TotalDays);
									dayNo = daysDelta % schedule.ScheduleScheme.DaysCount;
									break;
								case ScheduleSchemeType.Month:
									dayNo = (int)employeeDay.Date.Day - 1;
									break;
							}
							var dayIntervalParts = scheduleScheme.ScheduleDays.FirstOrDefault(x => x.Number == dayNo).DayInterval.DayIntervalParts;
							foreach (var dayIntervalPart in dayIntervalParts)
							{
								employeeDay.DayIntervalsString += dayIntervalPart.BeginTime + "-" + dayIntervalPart.EndTime + ";";
							}
							employee.LastEmployeeDayUpdate = employeeDay.Date;
							Context.SubmitChanges();
							result.Add(employeeDay);
						}
					}
				}
				var passJournalTranslator = new PassJournalTranslator();
				return passJournalTranslator.SaveEmployeeDays(result);
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public EmployeeSynchroniser Synchroniser;
	}
}