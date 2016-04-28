using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;
using SKDDriver.Translators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace SKDDriver
{
	public class EmployeeTranslator : WithShortTranslator<DataAccess.Employee, Employee, EmployeeFilter, ShortEmployee>
	{
		private readonly EFDataAccess.SKDEntities1 EFContext;

		public EmployeeTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
			Synchroniser = new EmployeeSynchroniser(Table, databaseService);
			EFContext = new EFDataAccess.SKDEntities1(@"metadata=res://*/EFDataAccess.SKDModel.csdl|res://*/EFDataAccess.SKDModel.ssdl|res://*/EFDataAccess.SKDModel.msl;
				provider=System.Data.SqlClient;provider connection string='data source=02-KBP-NIO-0524\SQLEXPRESS;
				initial catalog=SKD;integrated security=True;
				multipleactiveresultsets=True'");
		}


		public OperationResult<ShortEmployee> GetByUid(Guid id)
		{
			try
			{
				var result = new ShortEmployee();
				Context.CommandTimeout = 600;
				var tableItems =
					from employee in Context.Employees.Where(x => x.UID == id)
					join department in Context.Departments on employee.DepartmentUID equals department.UID into departments
					from department in departments.DefaultIfEmpty()
					join position in Context.Positions on employee.PositionUID equals position.UID into positions
					from position in positions.DefaultIfEmpty()
					join schedule in Context.Schedules on employee.ScheduleUID equals schedule.UID into schedules
					from schedule in schedules.DefaultIfEmpty()
					join organisation in Context.Organisations on employee.OrganisationUID equals organisation.UID into organisations
					from organisation in organisations.DefaultIfEmpty()
					join additionalColumn in Context.AdditionalColumns.Where(x => x.AdditionalColumnType.DataType == (int?)AdditionalColumnDataType.Text).DefaultIfEmpty()
						on employee.UID equals additionalColumn.EmployeeUID into additionalColumns
					select new
					{
						Employee = employee,
						Department = department,
						Position = position,
						Organisation = organisation,
						Schedule = schedule,
						Cards = new List<CardWithDoors>(),
						AdditionalColumns = additionalColumns,
					};

				var emp = tableItems.FirstOrDefault();
				if (emp != null)
					result = TranslateToShort(emp.Employee, emp.Department, emp.Position, emp.Organisation, emp.Schedule, emp.AdditionalColumns);
				return new OperationResult<ShortEmployee>(result);
			}
			catch (Exception e)
			{
				return OperationResult<ShortEmployee>.FromError(e.Message);
			}
		}

		public override OperationResult<IEnumerable<ShortEmployee>> GetList(EmployeeFilter filter)
		{
			try
			{
				var result = new List<ShortEmployee>();
				Context.CommandTimeout = 600;
				var tableItems =
					from employee in Context.Employees.Where(IsInFilter(filter))
					join department in Context.Departments on employee.DepartmentUID equals department.UID into departments
					from department in departments.DefaultIfEmpty()
					join position in Context.Positions on employee.PositionUID equals position.UID into positions
					from position in positions.DefaultIfEmpty()
					join schedule in Context.Schedules on employee.ScheduleUID equals schedule.UID into schedules
					from schedule in schedules.DefaultIfEmpty()
					join organisation in Context.Organisations on employee.OrganisationUID equals organisation.UID into organisations
					from organisation in organisations.DefaultIfEmpty()
					join additionalColumn in Context.AdditionalColumns.Where(x => x.AdditionalColumnType.DataType == (int?)AdditionalColumnDataType.Text).DefaultIfEmpty()
						on employee.UID equals additionalColumn.EmployeeUID into additionalColumns
					select new
						{
							Employee = employee,
							Department = department,
							Position = position,
							Organisation = organisation,
							Schedule = schedule,
							Cards = new List<CardWithDoors>(),
							AdditionalColumns = additionalColumns,
						};

				foreach (var tableItem in tableItems)
					result.Add(TranslateToShort(tableItem.Employee, tableItem.Department, tableItem.Position, tableItem.Organisation, tableItem.Schedule, tableItem.AdditionalColumns));
				return new OperationResult<IEnumerable<ShortEmployee>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<ShortEmployee>>.FromError(e.Message);
			}
		}

		public OperationResult<IEnumerable<ShortEmployee>> EFGetList(EmployeeFilter filter)
		{
			try
			{
				var result = new List<ShortEmployee>();
				EFContext.CommandTimeout = 600;
				var tableItems =
					from employee in EFContext.Employees//.Where(EFIsInFilter(filter))
					join department in EFContext.Departments on employee.DepartmentUID equals department.UID into departments
					from department in departments.DefaultIfEmpty()
					join position in EFContext.Positions on employee.PositionUID equals position.UID into positions
					from position in positions.DefaultIfEmpty()
					join schedule in EFContext.Schedules on employee.ScheduleUID equals schedule.UID into schedules
					from schedule in schedules.DefaultIfEmpty()
					join organisation in EFContext.Organisations on employee.OrganisationUID equals organisation.UID into organisations
					from organisation in organisations.DefaultIfEmpty()
					//join additionalColumn in EFContext.AdditionalColumns.Where(x => x.AdditionalColumnType.DataType == (int?)AdditionalColumnDataType.Text).DefaultIfEmpty()
					//    on employee.UID equals additionalColumn.EmployeeUID into additionalColumns
					select new
					{
						Employee = employee,
						Department = department,
						Position = position,
						Organisation = organisation,
						Schedule = schedule,

						//AdditionalColumns = new List<EFDataAccess.AdditionalColumn>(),
					};

				foreach (var tableItem in tableItems)
					result.Add(EFTranslateToShort(tableItem.Employee, tableItem.Department, tableItem.Position, tableItem.Organisation, tableItem.Schedule, new List<EFDataAccess.AdditionalColumn>()));
				var operationResult = new OperationResult<IEnumerable<ShortEmployee>>();
				operationResult.Result = result;
				return operationResult;
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<ShortEmployee>>.FromError(e.Message);
			}
		}

		public class CardWithDoors { public DataAccess.Card Card; public IEnumerable<DataAccess.CardDoor> CardDoors; }

		protected ShortEmployee TranslateToShort(
			DataAccess.Employee employee,
			DataAccess.Department department,
			DataAccess.Position position,
			DataAccess.Organisation organisation,
			DataAccess.Schedule schedule,
			IEnumerable<DataAccess.AdditionalColumn> additionalColumns)
		{
			var result = base.TranslateToShort(employee);
			result.FirstName = employee.FirstName;
			result.SecondName = employee.SecondName;
			result.LastName = employee.LastName;
			result.Description = employee.Description;
			var cards = new List<SKDCard>();
			result.Type = (PersonType)employee.Type;
			result.TabelNo = employee.TabelNo;
			var textColumns = new List<TextColumn>();
			foreach (var additionalColumn in additionalColumns)
			{
				textColumns.Add(new TextColumn { ColumnTypeUID = additionalColumn.AdditionalColumnTypeUID != null ? additionalColumn.AdditionalColumnTypeUID.Value : Guid.Empty, Text = additionalColumn.TextData });
			}
			result.Phone = employee.Phone;
			result.LastEmployeeDayUpdate = employee.LastEmployeeDayUpdate;
			if (position != null)
			{
				result.PositionName = position.Name;
				result.IsPositionDeleted = position.IsDeleted;
			}
			if (department != null)
			{
				result.DepartmentName = department.Name;
				result.IsDepartmentDeleted = department.IsDeleted;
			}
			if (organisation != null)
				result.OrganisationName = organisation.Name;
			result.ScheduleUID = schedule != null ? schedule.UID : Guid.Empty;
			return result;
		}

		protected ShortEmployee EFTranslateToShort(
			EFDataAccess.Employee employee,
			EFDataAccess.Department department,
			EFDataAccess.Position position,
			EFDataAccess.Organisation organisation,
			EFDataAccess.Schedule schedule,
			IEnumerable<EFDataAccess.AdditionalColumn> additionalColumns)
		{
			var result = new ShortEmployee
			{
				UID = employee.UID,
				IsDeleted = employee.IsDeleted,
				OrganisationUID = employee.OrganisationUID != null ? employee.OrganisationUID.Value : Guid.Empty,
				RemovalDate = employee.RemovalDate
			};
			result.FirstName = employee.FirstName;
			result.SecondName = employee.SecondName;
			result.LastName = employee.LastName;
			result.Description = employee.Description;
			var cards = new List<SKDCard>();
			result.Type = (PersonType)employee.Type;
			result.TabelNo = employee.TabelNo;
			var textColumns = new List<TextColumn>();
			foreach (var additionalColumn in additionalColumns)
			{
				textColumns.Add(new TextColumn { ColumnTypeUID = additionalColumn.AdditionalColumnTypeUID != null ? additionalColumn.AdditionalColumnTypeUID.Value : Guid.Empty, Text = additionalColumn.TextData });
			}
			result.Phone = employee.Phone;
			result.LastEmployeeDayUpdate = employee.LastEmployeeDayUpdate;
			if (position != null)
			{
				result.PositionName = position.Name;
				result.IsPositionDeleted = position.IsDeleted;
			}
			if (department != null)
			{
				result.DepartmentName = department.Name;
				result.IsDepartmentDeleted = department.IsDeleted;
			}
			if (organisation != null)
				result.OrganisationName = organisation.Name;
			result.ScheduleUID = schedule != null ? schedule.UID : Guid.Empty;
			return result;
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
                return new OperationResult(Resources.Language.Translators.EmployeeTranslator.CanSave_Error);
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
			result.Type = (PersonType)tableItem.Type; //TODO: nullable field o nullable type
			result.Cards = DatabaseService.CardTranslator.GetAllByEmployee<DataAccess.Card>(tableItem.UID);
			result.Position = DatabaseService.PositionTranslator.GetSingleShort(tableItem.PositionUID);
			result.Photo = GetResult(DatabaseService.PhotoTranslator.GetSingle(tableItem.PhotoUID));
			result.TabelNo = tableItem.TabelNo;
			result.EscortUID = tableItem.EscortUID;
			result.DocumentNumber = tableItem.DocumentNumber;
			result.BirthDate = tableItem.BirthDate;
			result.BirthPlace = tableItem.BirthPlace;
			result.DocumentGivenBy = tableItem.DocumentGivenBy;
			result.DocumentGivenDate = tableItem.DocumentGivenDate;
			result.DocumentValidTo = tableItem.DocumentValidTo;
			result.Gender = (Gender?)tableItem.Gender;
			result.DocumentDepartmentCode = tableItem.DocumentDepartmentCode;
			result.Citizenship = tableItem.Citizenship;
			result.DocumentType = (EmployeeDocumentType?)tableItem.DocumentType;
			result.Phone = tableItem.Phone;
			result.LastEmployeeDayUpdate = tableItem.LastEmployeeDayUpdate;
			return result;
		}

		private List<DataAccess.Department> _departments;
		private List<DataAccess.Position> _positions;
		private List<DataAccess.Organisation> _organisations;
		private List<DataAccess.Schedule> _schedules;
		private List<Guid> _additionalColumnTypeUIDs;
		private List<DataAccess.AdditionalColumn> _additionalColumns;
		private List<DataAccess.Card> _cards;

		protected override void BeforeGetList()
		{
			base.BeforeGetList();
			_departments = Context.Departments.ToList();
			_positions = Context.Positions.ToList();
			_organisations = Context.Organisations.ToList();
			_schedules = Context.Schedules.ToList();
			_additionalColumnTypeUIDs = DatabaseService.AdditionalColumnTypeTranslator.GetTextColumnTypes();
			_additionalColumns = Context.AdditionalColumns.ToList();
			_cards = Context.Cards.ToList();
		}

		protected override void AfterGetList()
		{
			base.AfterGetList();
			_departments = null;
			_positions = null;
			_organisations = null;
			_schedules = null;
			_additionalColumns = null;
			_additionalColumnTypeUIDs = null;
			_cards = null;
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
			tableItem.Type = (int)apiItem.Type; //TODO: Not null to nullable field
			tableItem.TabelNo = apiItem.TabelNo;
			tableItem.EscortUID = apiItem.EscortUID;
			tableItem.DocumentNumber = apiItem.DocumentNumber;
			tableItem.BirthDate = apiItem.BirthDate;
			tableItem.BirthPlace = apiItem.BirthPlace;
			tableItem.DocumentGivenBy = apiItem.DocumentGivenBy;
			tableItem.DocumentGivenDate = apiItem.DocumentGivenDate;
			tableItem.DocumentValidTo = apiItem.DocumentValidTo;
			tableItem.Gender = (int?)apiItem.Gender;
			tableItem.DocumentDepartmentCode = apiItem.DocumentDepartmentCode;
			tableItem.Citizenship = apiItem.Citizenship;
			tableItem.DocumentType = (int?)apiItem.DocumentType;
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

		public Expression<Func<DataAccess.Employee, bool>> GetFilterExpression(EmployeeFilter filter)
		{
			return IsInFilter(filter);
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(EmployeeFilter filter)
		{
			var result = base.IsInFilter(filter);
			if (!filter.IsAllPersonTypes)
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

		protected Expression<Func<EFDataAccess.Employee, bool>> EFIsInFilter(EmployeeFilter filter)
		{
			var result = PredicateBuilder.True<EFDataAccess.Employee>();
			result = result.And(e => e != null);
			var uids = filter.UIDs;
			if (uids != null && uids.Count != 0)
				result = result.And(e => uids.Contains(e.UID));
			var exceptUIDs = filter.ExceptUIDs;
			if (exceptUIDs != null && exceptUIDs.Count != 0)
				result = result.And(e => !exceptUIDs.Contains(e.UID));
			var IsDeletedExpression = PredicateBuilder.True<EFDataAccess.Employee>();
			switch (filter.LogicalDeletationType)
			{
				case LogicalDeletationType.Deleted:
					IsDeletedExpression = e => e.IsDeleted;
					break;

				case LogicalDeletationType.Active:
					IsDeletedExpression = e => !e.IsDeleted;
					break;

				default:
					break;
			}
			if (filter.OrganisationUIDs.IsNotNullOrEmpty())
				result = result.And(x => x.OrganisationUID != null && filter.OrganisationUIDs.Contains(x.OrganisationUID.Value));
			if (filter.UserUID != Guid.Empty)
				result = result.And(e => (Context.Organisations.Any(x => x.OrganisationUsers.Any(y => y.UserUID == filter.UserUID) && x.UID == e.OrganisationUID)));
			if (!filter.IsAllPersonTypes)
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

		private static DateTime _minDate = new DateTime(1900, 1, 1);

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

		public void TestGet()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			for (int i = 0; i < 100; i++)
			{
				var e1 = GetList(new EmployeeFilter());
			}
			stopWatch.Stop();
			Trace.WriteLine("LinqToSql " + new TimeSpan(stopWatch.Elapsed.Ticks / 100));
			var stopWatch2 = new Stopwatch();
			stopWatch2.Start();
			for (int i = 0; i < 100; i++)
			{
				var e2 = EFGetList(new EmployeeFilter());
			}
			stopWatch2.Stop();
			Trace.WriteLine("LinqToEntities " + new TimeSpan(stopWatch2.Elapsed.Ticks / 100));
			//var e1 = GetList(new EmployeeFilter());
			//var e2 = EFGetList(new EmployeeFilter());

			//int i = 0;
			//string s;
			//foreach (var employee in Table)
			//{
			//    i++;
			//    s = string.Format("{0} {1}", employee.UID, i);
			//}
		}

		#endregion TestData

		public EmployeeSynchroniser Synchroniser;
	}
}