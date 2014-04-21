using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class EmployeeTranslator : OrganizationElementTranslator<DataAccess.Employee, Employee, EmployeeFilter>
	{
		public EmployeeTranslator(DataAccess.SKDDataContext context,
			EmployeeReplacementTranslator replacementTranslator, 
			PositionTranslator positionTranslator, 
			DepartmentTranslator departmentTranslator, 
			AdditionalColumnTranslator additionalColumnTranslator, 
			CardTranslator cardTranslator, 
			PhotoTranslator photoTranslator,
			EmployeeDocumentTranslator employeeDocumentTranslator)
			: base(context)
		{
			EmployeeReplacementTranslator = replacementTranslator;
			PositionTranslator = positionTranslator;
			DepartmentTranslator = departmentTranslator;
			AdditionalColumnTranslator = additionalColumnTranslator;
			CardTranslator = cardTranslator;
			PhotoTranslator = photoTranslator;
			EmployeeDocumentTranslator = employeeDocumentTranslator;
		}

		PositionTranslator PositionTranslator;
		EmployeeReplacementTranslator EmployeeReplacementTranslator;
		DepartmentTranslator DepartmentTranslator;
		AdditionalColumnTranslator AdditionalColumnTranslator;
		CardTranslator CardTranslator;
		PhotoTranslator PhotoTranslator;
		EmployeeDocumentTranslator EmployeeDocumentTranslator;

		protected override OperationResult CanSave(Employee item)
		{
			bool sameName = Table.Any(x => x.FirstName == item.FirstName &&
				x.SecondName == item.SecondName &&
				x.LastName == item.LastName &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Сотрудник с таким же ФИО уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Employee item)
		{
			bool isAttendant = Context.Departments.Any(x => !x.IsDeleted && x.AttendantUID == item.UID);
			if (isAttendant)
				return new OperationResult("Не могу удалить сотрудника, пока он указан как сопровождающий для одного из отделов");

			bool isContactEmployee = Context.Departments.Any(x => !x.IsDeleted && x.ContactEmployeeUID == item.UID);
			if (isContactEmployee)
				return new OperationResult("Не могу удалить сотрудника, пока он указан как контактное лицо для одного из отделов");
			return base.CanSave(item);
		}

		protected override Employee Translate(DataAccess.Employee tableItem)
		{
			var result = base.Translate(tableItem);
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Appointed = tableItem.Appointed;
			result.Dismissed = tableItem.Dismissed;
			result.ReplacementUIDs = EmployeeReplacementTranslator.GetReplacementUIDs(tableItem.UID);
			result.CurrentReplacement = EmployeeReplacementTranslator.GetCurrentReplacement(tableItem.UID);
			result.DepartmentUID = tableItem.DepartmentUID;
			result.ScheduleUID = tableItem.ScheduleUID;
			result.AdditionalColumns = AdditionalColumnTranslator.GetAllByEmployee<DataAccess.AdditionalColumn>(tableItem.UID);
			result.Type = (FiresecAPI.PersonType)tableItem.Type;
			result.Cards = CardTranslator.GetByEmployee<DataAccess.Card>(tableItem.UID);
			result.Position = PositionTranslator.GetSingleShort(tableItem.PositionUID);
			result.Photo = GetResult(PhotoTranslator.GetSingle(tableItem.PhotoUID));
			result.TabelNo = tableItem.TabelNo;
			result.CredentialsStartDate = tableItem.CredentialsStartDate;
			result.EscortUID = tableItem.EscortUID;
			result.Document = GetResult(EmployeeDocumentTranslator.GetSingle(tableItem.DocumentUID));
			return result;
		}

		ShortEmployee TranslateToShort(DataAccess.Employee tableItem)
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
				Dismissed = tableItem.Dismissed.ToString("d MMM yyyy")
			};
			var position = Context.Positions.FirstOrDefault(x => x.UID == tableItem.PositionUID);
			if (position != null)
				shortEmployee.PositionName = position.Name;

			Guid? departmentUID;
			var replacement = EmployeeReplacementTranslator.GetCurrentReplacement(tableItem.UID);
			departmentUID = replacement != null ? replacement.DepartmentUID : tableItem.DepartmentUID;

			var department = Context.Departments.FirstOrDefault(x => x.UID == departmentUID);
			if (department != null)
				shortEmployee.DepartmentName = department.Name;
			return shortEmployee;
		}

		public OperationResult<IEnumerable<ShortEmployee>> GetList(EmployeeFilter filter)
		{
			try 
			{
				var result = new List<ShortEmployee>();
				foreach (var tableItem in GetTableItems(filter))
				{
					var employeeListItem = TranslateToShort(tableItem);
					result.Add(employeeListItem);
				}
				AdditionalColumnTranslator.SetTextColumns(result);
				var operationResult = new OperationResult<IEnumerable<ShortEmployee>>();
				operationResult.Result = result;
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult<IEnumerable<ShortEmployee>>(e.Message);
			}
		}

		protected override void TranslateBack(DataAccess.Employee tableItem, Employee apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.FirstName = apiItem.FirstName;
			tableItem.SecondName = apiItem.SecondName;
			tableItem.LastName = apiItem.LastName;
			tableItem.Appointed = CheckDate(apiItem.Appointed);
			tableItem.Dismissed = CheckDate(apiItem.Dismissed);
			if(apiItem.Position != null)
				tableItem.PositionUID = apiItem.Position.UID;
			tableItem.DepartmentUID = apiItem.DepartmentUID;
			tableItem.ScheduleUID = apiItem.ScheduleUID;
			if (apiItem.Photo != null)
				tableItem.PhotoUID = apiItem.Photo.UID;
			tableItem.Type = (int)apiItem.Type;
			tableItem.TabelNo = apiItem.TabelNo;
			tableItem.CredentialsStartDate = apiItem.CredentialsStartDate;
			tableItem.EscortUID = apiItem.EscortUID;
			if(apiItem.Document != null)
				tableItem.DocumentUID = apiItem.Document.UID;
		}

		public override OperationResult Save(IEnumerable<Employee> apiItems)
		{
			foreach (var item in apiItems)
			{
				var employeeDocumentResult = EmployeeDocumentTranslator.Save(new List<EmployeeDocument> { item.Document });
				if (employeeDocumentResult.HasError)
					return employeeDocumentResult;
				var columnSaveResult = AdditionalColumnTranslator.Save(item.AdditionalColumns);
				if (columnSaveResult.HasError)
					return columnSaveResult;
				var photoSaveResult = PhotoTranslator.Save(new List<Photo>{ item.Photo });
				if (photoSaveResult.HasError)
					return photoSaveResult;
			}
			return base.Save(apiItems);
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(EmployeeFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Employee>();
			result = result.And(base.IsInFilter(filter));
			result = result.And(e => e.Type == (int?)filter.PersonType);
			var departmentUIDs = filter.DepartmentUIDs;
			if (departmentUIDs.IsNotNullOrEmpty())
			{
				result = result.And(e =>
					e != null &&
					(Context.EmployeeReplacements.Any(x =>
						!x.IsDeleted &&
						x.EmployeeUID == e.UID &&
						DateTime.Now >= x.BeginDate &&
						DateTime.Now <= x.EndDate &&
						departmentUIDs.Contains(x.DepartmentUID.Value)
						) ||
						(!Context.EmployeeReplacements.Any(x =>
								!x.IsDeleted &&
								x.EmployeeUID == e.UID &&
								DateTime.Now >= x.BeginDate &&
								DateTime.Now <= x.EndDate &&
								departmentUIDs.Contains(x.DepartmentUID.Value)
							) &&
							departmentUIDs.Contains(e.DepartmentUID.Value)
						)
					)
				);
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
			return result;
		}
	}
}