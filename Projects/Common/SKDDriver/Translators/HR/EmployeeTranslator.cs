using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class EmployeeTranslator : OrganisationItemTranslatorBase<Employee, API.Employee, API.EmployeeFilter>
	{
		public EmployeeShortTranslator ShortTranslator { get; private set; }
		public EmployeeAsyncTranslator AsyncTranslator { get; private set; }
		public EmployeeSynchroniser Synchroniser { get; private set; }

		public EmployeeTranslator(DbService dbService)
			: base(dbService)
		{
			ShortTranslator = new EmployeeShortTranslator(this);
			AsyncTranslator = new EmployeeAsyncTranslator(ShortTranslator);
			Synchroniser = new EmployeeSynchroniser(Table, DbService);
		}

		public override DbSet<Employee> Table
		{
			get { return Context.Employees; }
		}

		public override IQueryable<Employee> GetTableItems()
		{
			return base.GetTableItems()
				.Include(x => x.Photo)
				.Include(x => x.Position)
				.Include(x => x.Department)
				.Include(x => x.Schedule)
				.Include(x => x.AdditionalColumns.Select(additionalColumn => additionalColumn.AdditionalColumnType))
				.Include(x => x.AdditionalColumns.Select(additionalColumn => additionalColumn.Photo));
		}

		public override IQueryable<Employee> GetFilteredTableItems(API.EmployeeFilter filter, IQueryable<Employee> tableItems)
		{
			return base.GetFilteredTableItems(filter, tableItems).Where(x =>
				(filter.DepartmentUIDs.Count() == 0 ||
					(x.DepartmentUID != null && filter.DepartmentUIDs.Contains(x.DepartmentUID.Value))) &&
				(!filter.IsEmptyDepartment ||
					x.DepartmentUID == null || x.Department.IsDeleted) &&
				(filter.PositionUIDs.Count() == 0 ||
					(x.PositionUID != null && filter.PositionUIDs.Contains(x.PositionUID.Value))) &&
				(!filter.IsEmptyPosition ||
					x.PositionUID == null || x.Position.IsDeleted) &&
				(filter.ScheduleUIDs.Count() == 0 ||
					(x.ScheduleUID != null && filter.ScheduleUIDs.Contains(x.ScheduleUID.Value))) &&
				(string.IsNullOrEmpty(filter.FirstName) ||
					(x.FirstName.Contains(filter.FirstName))) &&
				(string.IsNullOrEmpty(filter.SecondName) ||
					(x.SecondName.Contains(filter.SecondName))) &&
				(string.IsNullOrEmpty(filter.LastName) ||
					(x.LastName.Contains(filter.LastName))) &&
				x.Type == (int)filter.PersonType
				);
		}

		public override API.Employee Translate(Employee tableItem)
		{
			var result = base.Translate(tableItem);
			if (result == null)
				return null;
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Position = DbService.PositionTranslator.ShortTranslator.Translate(tableItem.Position);
			result.Department = DbService.DepartmentTranslator.ShortTranslator.Translate(tableItem.Department);
			result.Schedule = DbService.ScheduleTranslator.Translate(tableItem.Schedule);
			result.ScheduleName = tableItem.Schedule != null ? tableItem.Schedule.Name : "";
			result.ScheduleStartDate = tableItem.ScheduleStartDate;
			result.Photo = tableItem.Photo != null ? tableItem.Photo.Translate() : null;
			result.AdditionalColumns = tableItem.AdditionalColumns.Select(x => new API.AdditionalColumn
			{
				UID = x.UID,
				EmployeeUID = x.EmployeeUID,
				AdditionalColumnType = DbService.AdditionalColumnTypeTranslator.Translate(x.AdditionalColumnType),
				Photo = x.Photo != null ? x.Photo.Translate() : null,
				TextData = x.TextData
			}).ToList();
			result.Type = (API.PersonType)tableItem.Type;
			result.TabelNo = tableItem.TabelNo;
			result.CredentialsStartDate = tableItem.CredentialsStartDate;
			result.EscortUID = tableItem.EscortUID;
			result.DocumentNumber = tableItem.DocumentNumber;
			result.BirthDate = tableItem.BirthDate;
			result.BirthPlace = tableItem.BirthPlace;
			result.DocumentGivenBy = tableItem.DocumentGivenBy;
			result.DocumentValidTo = tableItem.DocumentValidTo;
			result.Gender = (API.Gender)tableItem.Gender;
			result.DocumentDepartmentCode = result.DocumentDepartmentCode;
			result.Citizenship = tableItem.Citizenship;
			result.DocumentType = (API.EmployeeDocumentType)tableItem.DocumentType;
			result.LastEmployeeDayUpdate = tableItem.LastEmployeeDayUpdate;
			result.Phone = tableItem.Phone;
			return result;
		}

		public override void TranslateBack(API.Employee apiItem, Employee tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.FirstName = apiItem.FirstName;
			tableItem.SecondName = apiItem.SecondName;
			tableItem.LastName = apiItem.LastName;
			tableItem.PositionUID = apiItem.Position != null ? (Guid?)apiItem.Position.UID : null;
			tableItem.DepartmentUID = apiItem.Department != null ? (Guid?)apiItem.Department.UID : null;
			tableItem.ScheduleUID = apiItem.Schedule != null ? (Guid?)apiItem.Schedule.UID : null;
			tableItem.ScheduleStartDate = apiItem.ScheduleStartDate.CheckDate();
			tableItem.Photo = Photo.Create(apiItem.Photo);
			tableItem.AdditionalColumns = apiItem.AdditionalColumns.Select(x => new AdditionalColumn
			{
				UID = x.UID,
				EmployeeUID = x.EmployeeUID,
				AdditionalColumnTypeUID = x.AdditionalColumnType != null ? (Guid?)x.AdditionalColumnType.UID : null,
				Photo = Photo.Create(x.Photo),
				TextData = x.TextData
			}).ToList();
			tableItem.Type = (int)apiItem.Type;
			tableItem.TabelNo = apiItem.TabelNo;
			tableItem.CredentialsStartDate = apiItem.CredentialsStartDate;
			tableItem.EscortUID = apiItem.EscortUID;
			tableItem.BirthDate = apiItem.BirthDate.CheckDate();
			tableItem.BirthPlace = apiItem.BirthPlace;
			tableItem.DocumentNumber = apiItem.DocumentNumber;
			tableItem.DocumentGivenBy = apiItem.DocumentGivenBy;
			tableItem.DocumentValidTo = apiItem.DocumentValidTo.CheckDate();
			tableItem.DocumentGivenDate = apiItem.DocumentGivenDate.CheckDate();
			tableItem.DocumentDepartmentCode = tableItem.DocumentDepartmentCode;
			tableItem.DocumentType = (int)apiItem.DocumentType;
			tableItem.Gender = (int)apiItem.Gender;
			tableItem.Citizenship = apiItem.Citizenship;
			tableItem.LastEmployeeDayUpdate = apiItem.LastEmployeeDayUpdate.CheckDate();
			tableItem.Phone = apiItem.Phone;
		}

		protected override void ClearDependentData(Employee tableItem)
		{
			Context.AdditionalColumns.RemoveRange(tableItem.AdditionalColumns);
			if (tableItem.Photo != null)
				Context.Photos.Remove(tableItem.Photo);
		}

		public OperationResult SaveDepartment(Guid uid, Guid? departmentUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem == null)
					return new OperationResult("Запись не найдена");
				tableItem.DepartmentUID = departmentUID != null ? departmentUID.Value.EmptyToNull() : null;
				Context.SaveChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SavePosition(Guid uid, Guid? positionUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem == null)
					return new OperationResult("Запись не найдена");
				tableItem.PositionUID = positionUID != null ? positionUID.Value.EmptyToNull() : null;
				Context.SaveChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		protected override OperationResult<bool> CanSave(API.Employee item)
		{
			if (item == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			if (item.OrganisationUID == Guid.Empty)
				return OperationResult<bool>.FromError("Не указана организация");
			bool hasSameName = Table.Any(x => x.FirstName == item.FirstName &&
				x.FirstName == item.FirstName &&
				x.SecondName == item.SecondName &&
				x.LastName == item.LastName &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			else
				return new OperationResult<bool>(true);
		}
	}

	public class EmployeeShortTranslator : OrganisationShortTranslatorBase<Employee, API.ShortEmployee, API.Employee, API.EmployeeFilter>
	{
		public EmployeeShortTranslator(EmployeeTranslator translator) : base(translator as ITranslatorGet<Employee, API.Employee, API.EmployeeFilter>) { }

		public override IQueryable<Employee> GetTableItems()
		{
			return base.GetTableItems()
				.Include(x => x.Position)
				.Include(x => x.Department)
				.Include(x => x.AdditionalColumns.Select(additionalColumn => additionalColumn.AdditionalColumnType));
		}

		public override API.ShortEmployee Translate(Employee employee)
		{
			return new API.ShortEmployee
			{
				UID = employee.UID,
				FirstName = employee.FirstName,
				SecondName = employee.SecondName,
				LastName = employee.LastName,
				Description = employee.Description,
				DepartmentName = employee.Department != null ? employee.Department.Name : null,
				IsDepartmentDeleted = employee.Department != null && employee.Department.IsDeleted,
				PositionName = employee.Position != null ? employee.Position.Name : null,
				IsPositionDeleted = employee.Position != null && employee.Position.IsDeleted,
				Type = (API.PersonType)employee.Type,
				TextColumns = employee.AdditionalColumns.Where(x => x.AdditionalColumnType.DataType == 0).
					Select(x => new TextColumn { ColumnTypeUID = x.AdditionalColumnType.UID, Text = x.TextData }).ToList(),
				CredentialsStartDate = employee.CredentialsStartDate.ToString(),
				TabelNo = employee.TabelNo,
				OrganisationUID = employee.OrganisationUID.GetValueOrDefault(),
				OrganisationName = employee.Organisation.Name,
				Phone = employee.Phone,
				IsDeleted = employee.IsDeleted,
				RemovalDate = employee.RemovalDate.GetValueOrDefault(),
				LastEmployeeDayUpdate = employee.LastEmployeeDayUpdate,
				ScheduleUID = employee.ScheduleUID.GetValueOrDefault()
			};
		}
	}

	public class EmployeeAsyncTranslator : AsyncTranslator<Employee, API.ShortEmployee, EmployeeFilter>
	{
		public EmployeeAsyncTranslator(EmployeeShortTranslator translator) : base(translator as ITranslatorGet<Employee, API.ShortEmployee, EmployeeFilter>) { }
		public override List<ShortEmployee> GetCollection(DbCallbackResult callbackResult)
		{
			return callbackResult.Employees;
		}
	}
}