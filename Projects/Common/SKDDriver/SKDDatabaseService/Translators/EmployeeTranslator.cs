using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class EmployeeTranslator : WithShortTranslator<DataAccess.Employee, Employee, EmployeeFilter, ShortEmployee>
	{
		public EmployeeTranslator(DataAccess.SKDDataContext context,
			EmployeeReplacementTranslator replacementTranslator,
			PositionTranslator positionTranslator,
			DepartmentTranslator departmentTranslator,
			AdditionalColumnTranslator additionalColumnTranslator,
			CardTranslator cardTranslator,
			PhotoTranslator photoTranslator)
			: base(context)
		{
			EmployeeReplacementTranslator = replacementTranslator;
			PositionTranslator = positionTranslator;
			DepartmentTranslator = departmentTranslator;
			AdditionalColumnTranslator = additionalColumnTranslator;
			CardTranslator = cardTranslator;
			PhotoTranslator = photoTranslator;
		}

		PositionTranslator PositionTranslator;
		EmployeeReplacementTranslator EmployeeReplacementTranslator;
		DepartmentTranslator DepartmentTranslator;
		AdditionalColumnTranslator AdditionalColumnTranslator;
		CardTranslator CardTranslator;
		PhotoTranslator PhotoTranslator;

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
				return new OperationResult("Не могу удалить сотрудника, пока он указан как сопровождающий для одного из отделов");

			bool isContactEmployee = Context.Departments.Any(x => !x.IsDeleted && x.ContactEmployeeUID == uid);
			if (isContactEmployee)
				return new OperationResult("Не могу удалить сотрудника, пока он указан как контактное лицо для одного из отделов");
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
			result.ReplacementUIDs = EmployeeReplacementTranslator.GetReplacementUIDs(tableItem.UID);
			result.CurrentReplacement = EmployeeReplacementTranslator.GetCurrentReplacement(tableItem.UID);
			result.DepartmentUID = tableItem.DepartmentUID;
			result.ScheduleUID = tableItem.ScheduleUID;
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

			Guid? departmentUID;
			var replacement = EmployeeReplacementTranslator.GetCurrentReplacement(tableItem.UID);
			departmentUID = replacement != null ? replacement.DepartmentUID : tableItem.DepartmentUID;

			var department = Context.Departments.FirstOrDefault(x => x.UID == departmentUID);
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
			tableItem.DepartmentUID = apiItem.DepartmentUID;
			tableItem.ScheduleUID = apiItem.ScheduleUID;
			tableItem.ScheduleStartDate = apiItem.ScheduleStartDate;
			if (apiItem.Photo != null)
				tableItem.PhotoUID = apiItem.Photo.UID;
			tableItem.Type = (int)apiItem.Type;
			tableItem.TabelNo = apiItem.TabelNo;
			tableItem.CredentialsStartDate = apiItem.CredentialsStartDate;
			tableItem.EscortUID = apiItem.EscortUID;
			tableItem.DocumentNumber = apiItem.DocumentNumber;
			tableItem.BirthDate = apiItem.BirthDate;
			tableItem.BirthPlace = apiItem.BirthPlace;
			tableItem.DocumentGivenBy = apiItem.DocumentGivenBy;
			tableItem.DocumentGivenDate = apiItem.DocumentGivenDate;
			tableItem.DocumentValidTo = apiItem.DocumentValidTo;
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
			var photoSaveResult = PhotoTranslator.Save(new List<Photo> { apiItem.Photo });
			if (photoSaveResult.HasError)
				return photoSaveResult;
			return base.Save(apiItem);
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

			if (!string.IsNullOrEmpty(filter.LastName))
				result = result.And(e => e.LastName.Contains(filter.LastName));

			if (!string.IsNullOrEmpty(filter.FirstName))
				result = result.And(e => e.FirstName.Contains(filter.FirstName));

			if (!string.IsNullOrEmpty(filter.SecondName))
				result = result.And(e => e.SecondName.Contains(filter.SecondName));

			return result;
		}
	}
}