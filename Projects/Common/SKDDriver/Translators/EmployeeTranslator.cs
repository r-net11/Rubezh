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
			result.AdditionalColumns = DatabaseService.AdditionalColumnTranslator.GetAllByEmployee<DataAccess.AdditionalColumn>(tableItem.UID).Where(x => x.AdditionalColumnType != null).ToList();
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
			result.Cards = new List<SKDCard>();
			result.Cards = DatabaseService.CardTranslator.GetAllByEmployee<DataAccess.Card>(tableItem.UID);
			result.Type = (PersonType)tableItem.Type;
			result.CredentialsStartDate = tableItem.CredentialsStartDate.ToString("d MMM yyyy");
			result.TabelNo = tableItem.TabelNo;
			result.TextColumns = DatabaseService.AdditionalColumnTranslator.GetTextColumns(tableItem.UID);
			result.Phone = tableItem.Phone;
			result.LastEmployeeDayUpdate = tableItem.LastEmployeeDayUpdate;
			var position = _positions != null ? _positions.FirstOrDefault(x => x.UID == tableItem.PositionUID) : Context.Positions.FirstOrDefault(x => x.UID == tableItem.PositionUID);
			if (position != null)
			{
				result.PositionName = position.Name;
				result.IsPositionDeleted = position.IsDeleted;
			}
			var department = _departments != null ? _departments.FirstOrDefault(x => x.UID == tableItem.DepartmentUID) : Context.Departments.FirstOrDefault(x => x.UID == tableItem.DepartmentUID);
			if (department != null)
			{
				result.DepartmentName = department.Name;
				result.IsDepartmentDeleted = department.IsDeleted;
			}
			var organisation = _organisations != null ? _organisations.FirstOrDefault(x => x.UID == tableItem.OrganisationUID) : Context.Organisations.FirstOrDefault(x => x.UID == tableItem.OrganisationUID);
			if (organisation != null)
				result.OrganisationName = organisation.Name;
			var schedule = _schedules != null ? _schedules.FirstOrDefault(x => x.UID == tableItem.ScheduleUID) : Context.Schedules.FirstOrDefault(x => x.UID == tableItem.ScheduleUID);
			result.ScheduleUID = schedule != null ? schedule.UID : Guid.Empty;
			return result;
		}

		List<DataAccess.Department> _departments;
		List<DataAccess.Position> _positions;
		List<DataAccess.Organisation> _organisations;
		List<DataAccess.Schedule> _schedules;

		protected override void BeforeGetList()
		{
			base.BeforeGetList();
			_departments = Context.Departments.ToList();
			_positions = Context.Positions.ToList();
			_organisations = Context.Organisations.ToList();
			_schedules = Context.Schedules.ToList();
		}

		protected override void AfterGetList()
		{
			base.AfterGetList();
			_departments = null;
			_positions = null;
			_organisations = null;
			_schedules = null;
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
			var photoSaveResult = DatabaseService.PhotoTranslator.SaveOrDelete(apiItem.Photo);
			if (photoSaveResult.HasError)
				return photoSaveResult;
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

		#region TestData
		public OperationResult GenerateTestData()
		{
			try
			{
				for (int i = 0; i < 1; i++)
				{
					var org = new DataAccess.Organisation { Name = "Тестовая Организация " + i, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
					Context.Organisations.InsertOnSubmit(org);
					var posUIDs = new List<Guid>();
					for (int j = 0; j < 1000; j++)
					{
						var pos = new DataAccess.Position { Name = "Должность " + i + j, OrganisationUID = org.UID, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
						Context.Positions.InsertOnSubmit(pos);
						posUIDs.Add(pos.UID);
					}
					var deptUIDs = new List<Guid>();
					for (int j = 0; j < 100; j++)
					{
						var dept = CreateDept("Подразделение " + i + j, org.UID);
						deptUIDs.Add(dept.UID);
						Context.Departments.InsertOnSubmit(dept);
						for (int k = 0; k < 2; k++)
						{
							var dept2 = CreateDept("Подразделение " + i + j + k, org.UID, dept.UID);
							deptUIDs.Add(dept2.UID);
							Context.Departments.InsertOnSubmit(dept2);
							for (int m = 0; m < 2; m++)
							{
								var dept3 = CreateDept("Подразделение " + i + j + k + m, org.UID, dept2.UID);
								deptUIDs.Add(dept3.UID);
								Context.Departments.InsertOnSubmit(dept3);
								for (int n = 0; n < 2; n++)
								{
									var dept4 = CreateDept("Подразделение " + i + j + k + m + n, org.UID, dept3.UID);
									deptUIDs.Add(dept4.UID);
									Context.Departments.InsertOnSubmit(dept4);
								}
							}
						}
					}
					for (int j = 0; j < 500; j++)
					{
						var empl = CreateEmpl("Сотрудник " + i + j + "0", org.UID, deptUIDs.FirstOrDefault(), posUIDs.FirstOrDefault());
						Context.Employees.InsertOnSubmit(empl);
					}
					for (int j = 0; j < 500; j++)
					{
						var empl = CreateEmpl("Сотрудник " + i + j + "1", org.UID, deptUIDs.LastOrDefault(), posUIDs.LastOrDefault());
						Context.Employees.InsertOnSubmit(empl);
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

		DataAccess.Department CreateDept(string name, Guid orgUID, Guid? parentUID = null)
		{
			return new DataAccess.Department { Name = name, OrganisationUID = orgUID, UID = Guid.NewGuid(), ParentDepartmentUID = parentUID, RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
		}

		DataAccess.Employee CreateEmpl(string no, Guid orgUID, Guid? deptUID = null, Guid? posUID = null)
		{
			return new DataAccess.Employee
			{
				LastName = "Фамилия " + no,
				FirstName = "Имя " + no,
				SecondName = "Отчество " + no,
				DepartmentUID = deptUID,
				PositionUID = posUID,
				OrganisationUID = orgUID,
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
						if (scheduleScheme != null)
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
		#endregion

		public EmployeeSynchroniser Synchroniser;
	}
}