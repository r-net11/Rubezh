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
		public EmployeeTranslator(DbService dbService) : base(dbService) { }
		
		public override DbSet<Employee> Table
		{
			get { return Context.Employees; }
		}

		//protected override IQueryable<Employee> GetTableItems()
		//{
		//    return base.GetTableItems()
		//        .Include(x => x.Photo)
		//        .Include(x => x.Position)
		//        //.Include(x => x.Department)
		//        .Include(x => x.Schedule)
		//        .Include(x => x.AdditionalColumns.Select(additionalColumn => additionalColumn.AdditionalColumnType))
		//        .Include(x => x.Cards.Select(card => card.CardDoors))
		//        .Include(x => x.Cards.Select(card => card.Employee))
		//        .Include(x => x.Cards.Select(card => card.AccessTemplate))
		//        .Include(x => x.Cards.Select(card => card.GKControllerUIDs));
		//}

		//protected IQueryable<Employee> GetShortTableItems()
		//{
		//    return base.GetTableItems()
		//        .Include(x => x.Position)
		//        //.Include(x => x.Department)
		//        .Include(x => x.AdditionalColumns.Select(additionalColumn => additionalColumn.AdditionalColumnType));
		//}

		public IQueryable<Employee> GetFilteredTableItems(API.EmployeeFilter filter)
		{
			return //base.GetFilteredTableItems(filter).Where(x =>
				base.GetTableItems().Where(x =>
				//(filter.DepartmentUIDs.Count() == 0 ||
				//    (x.DepartmentUID != null && filter.DepartmentUIDs.Contains(x.DepartmentUID.Value))) &&
				//(filter.PositionUIDs.Count() == 0 ||
				//    (x.PositionUID != null && filter.PositionUIDs.Contains(x.PositionUID.Value))) &&
				//(filter.ScheduleUIDs.Count() == 0 ||
				//    (x.ScheduleUID != null && filter.ScheduleUIDs.Contains(x.ScheduleUID.Value))) &&
				(string.IsNullOrEmpty(filter.FirstName) ||
					(x.FirstName.Contains(filter.FirstName))) &&
				(string.IsNullOrEmpty(filter.SecondName) ||
					(x.SecondName.Contains(filter.SecondName))) &&
				(string.IsNullOrEmpty(filter.LastName) ||
					(x.LastName.Contains(filter.LastName)))
				);
		}

		public API.ShortEmployee TranslateToShort(Employee employee)
		{
			return new API.ShortEmployee
			{
				UID = employee.UID,
				FirstName = employee.FirstName,
				SecondName = employee.SecondName,
				LastName = employee.LastName,
				Description = employee.Description,
				//DepartmentName = employee.Department != null ? employee.Department.Name : null,
				//IsDepartmentDeleted = employee.Department != null && employee.Department.IsDeleted,
				//PositionName = employee.Position != null ? employee.Position.Name : null,
				//IsPositionDeleted = employee.Position != null && employee.Position.IsDeleted,
				Type = (API.PersonType)employee.Type,
				TextColumns = new List<TextColumn>(),// employee.AdditionalColumns.Where(x => x.AdditionalColumnType.DataType == 0).
					//Select(x => new TextColumn { ColumnTypeUID = x.AdditionalColumnType.UID, Text = x.TextData }).ToList(),
				CredentialsStartDate = employee.CredentialsStartDate.ToString(),
				TabelNo = employee.TabelNo,
				OrganisationUID = employee.OrganisationUID.GetValueOrDefault(),
				OrganisationName = employee.Organisation.Name,
				Phone = employee.Phone,
				IsDeleted = employee.IsDeleted,
				RemovalDate = employee.RemovalDate.GetValueOrDefault(),
				LastEmployeeDayUpdate = employee.LastEmployeeDayUpdate,
				//ScheduleUID = employee.ScheduleUID.GetValueOrDefault()
			};
		}

		public OperationResult<List<API.ShortEmployee>> GetList(API.EmployeeFilter filter)
		{
			try
			{
				return new OperationResult<List<API.ShortEmployee>>(new List<API.ShortEmployee>());
				var tableItems = Table.ToList();// GetFilteredTableItems(filter).ToList();
				var result = tableItems.Select(x => TranslateToShort(x)).ToList();
				return new OperationResult<List<API.ShortEmployee>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<API.ShortEmployee>>.FromError(e.Message);
			}
		}

		public override API.Employee Translate(Employee tableItem)
		{
			var result = base.Translate(tableItem);
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			//result.Position = 
			//result.Department = 
			//result.Schedule = 
			//result.ScheduleName = tableItem.Schedule != null ? tableItem.Schedule.Name : "";
			result.ScheduleStartDate = tableItem.ScheduleStartDate;
			//result.Photo = tableItem.Photo != null ? tableItem.Photo.Translate() : null;
			//result.AdditionalColumns = tableItem.AdditionalColumns.Select(x => new API.AdditionalColumn
			//{
			//    UID = x.UID,
			//    EmployeeUID = x.EmployeeUID,
			//    //AdditionalColumnType = new API.AdditionalColumnType(),// x.AdditionalColumnType,
			//    Photo = x.Photo != null ? x.Photo.Translate() : null,
			//    TextData = x.TextData
			//}).ToList();
			//result.Cards = tableItem.Cards.Select(x => DbService.CardTranslator.Translate(x)).ToList();
			result.Type = (API.PersonType)tableItem.Type;
			result.TabelNo = tableItem.TabelNo;
			result.CredentialsStartDate = tableItem.CredentialsStartDate;
			//result.EscortUID = tableItem.EscortUID;
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
			//result.Position = 
			//result.Department = 
			//result.Schedule = 
			tableItem.ScheduleStartDate = apiItem.ScheduleStartDate;
			//tableItem.Photo = Photo.Create(apiItem.Photo);
			//tableItem.AdditionalColumns = apiItem.AdditionalColumns.Select(x => new AdditionalColumn
			//{
			//    UID = x.UID,
			//    EmployeeUID = x.EmployeeUID,
			//    //AdditionalColumnType = x.AdditionalColumnType,
			//    Photo = Photo.Create(x.Photo),
			//    TextData = x.TextData
			//}).ToList();
			//tableItem.Cards = apiItem.Cards.Select(x => DbService.CardTranslator.CreateCard(x)).ToList();
			tableItem.Type = (int)apiItem.Type;
			tableItem.TabelNo = apiItem.TabelNo;
			tableItem.CredentialsStartDate = apiItem.CredentialsStartDate;
			//tableItem.EscortUID = apiItem.EscortUID;
			tableItem.DocumentNumber = apiItem.DocumentNumber;
			tableItem.BirthDate = apiItem.BirthDate;
			tableItem.BirthPlace = apiItem.BirthPlace;
			tableItem.DocumentGivenBy = apiItem.DocumentGivenBy;
			tableItem.DocumentValidTo = apiItem.DocumentValidTo;
			tableItem.Gender = (int)apiItem.Gender;
			tableItem.DocumentDepartmentCode = tableItem.DocumentDepartmentCode;
			tableItem.Citizenship = apiItem.Citizenship;
			tableItem.DocumentType = (int)apiItem.DocumentType;
			tableItem.LastEmployeeDayUpdate = apiItem.LastEmployeeDayUpdate;
			tableItem.Phone = apiItem.Phone;
		}

		protected override void ClearDependentData(Employee tableItem)
		{
			//Context.AdditionalColumns.RemoveRange(tableItem.AdditionalColumns);
		}

		public OperationResult SaveDepartment(Guid uid, Guid departmentUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				//tableItem.DepartmentUID = departmentUID;
				Context.SaveChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SavePosition(Guid uid, Guid PositionUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				//tableItem.PositionUID = PositionUID;
				Context.SaveChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}
	}
}