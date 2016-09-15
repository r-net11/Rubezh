using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public class EmployeeTranslator : WithShortTranslator<DataAccess.Employee, Employee, EmployeeFilter, ShortEmployee>
	{
		private readonly EFDataAccess.SKDEntities1 _efContext;

		public EmployeeTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
			Synchroniser = new EmployeeSynchroniser(Table, databaseService);
			_efContext = new EFDataAccess.SKDEntities1(@"metadata=res://*/EFDataAccess.SKDModel.csdl|res://*/EFDataAccess.SKDModel.ssdl|res://*/EFDataAccess.SKDModel.msl;
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
				_efContext.CommandTimeout = 600;

				var tableItems =
					from employee in _efContext.Employees
					join department in _efContext.Departments on employee.DepartmentUID equals department.UID into departments
					from department in departments.DefaultIfEmpty()
					join position in _efContext.Positions on employee.PositionUID equals position.UID into positions
					from position in positions.DefaultIfEmpty()
					join schedule in _efContext.Schedules on employee.ScheduleUID equals schedule.UID into schedules
					from schedule in schedules.DefaultIfEmpty()
					join organisation in _efContext.Organisations on employee.OrganisationUID equals organisation.UID into organisations
					from organisation in organisations.DefaultIfEmpty()
					select new
					{
						Employee = employee,
						Department = department,
						Position = position,
						Organisation = organisation,
						Schedule = schedule
					};

				foreach (var tableItem in tableItems)
					result.Add(EFTranslateToShort(tableItem.Employee, tableItem.Department, tableItem.Position, tableItem.Organisation, tableItem.Schedule, new List<EFDataAccess.AdditionalColumn>()));

				var operationResult = new OperationResult<IEnumerable<ShortEmployee>> {Result = result};

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
				RemovalDate = employee.RemovalDate,
				FirstName = employee.FirstName,
				SecondName = employee.SecondName,
				LastName = employee.LastName,
				Description = employee.Description,
				Type = (PersonType) employee.Type,
				TabelNo = employee.TabelNo
			};
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

			var hasSameName = Table.Any(x => x.FirstName == employee.FirstName &&
				x.SecondName == employee.SecondName &&
				x.LastName == employee.LastName &&
				x.OrganisationUID == employee.OrganisationUID &&
				x.UID != employee.UID &&
				x.IsDeleted == false);

			return hasSameName
				? new OperationResult("Сотрудник с таким же ФИО уже содержится в базе данных")
				: new OperationResult();
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
				result = filter.WithDeletedPositions
					? result.And(e => filter.PositionUIDs.Contains(e.PositionUID.Value) || Context.Positions.Any(x => x.IsDeleted && x.UID == e.PositionUID))
					: result.And(e => e != null && filter.PositionUIDs.Contains(e.PositionUID.Value));
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
				result = filter.WithDeletedPositions
					? result.And(e => filter.PositionUIDs.Contains(e.PositionUID.Value) || Context.Positions.Any(x => x.IsDeleted && x.UID == e.PositionUID))
					: result.And(e => e != null && filter.PositionUIDs.Contains(e.PositionUID.Value));
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

				if(tableItem == null)
					return new OperationResult("Employee not exist");

				if (departmentUID == Guid.Empty)
					DatabaseService.DepartmentTranslator.SaveChief(tableItem.DepartmentUID.GetValueOrDefault(), departmentUID);

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

				if(tableItem == null)
					return new OperationResult("Department not exist");

				tableItem.PositionUID = positionUID;
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public EmployeeSynchroniser Synchroniser;
	}
}