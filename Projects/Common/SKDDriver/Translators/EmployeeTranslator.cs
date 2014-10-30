﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class EmployeeTranslator : WithShortTranslator<DataAccess.Employee, Employee, EmployeeFilter, ShortEmployee>
	{
		public EmployeeTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
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
			result.Appointed = tableItem.Appointed;
			result.Department = DatabaseService.DepartmentTranslator.GetSingleShort(tableItem.DepartmentUID);
			result.Schedule = DatabaseService.ScheduleTranslator.GetSingleShort(tableItem.ScheduleUID);
			result.ScheduleStartDate = tableItem.ScheduleStartDate;
			result.AdditionalColumns = DatabaseService.AdditionalColumnTranslator.GetAllByEmployee<DataAccess.AdditionalColumn>(tableItem.UID);
			result.Type = (PersonType)tableItem.Type;
			result.Cards = DatabaseService.CardTranslator.GetByEmployee<DataAccess.Card>(tableItem.UID);
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
			return result;
		}

		protected override ShortEmployee TranslateToShort(DataAccess.Employee tableItem)
		{
			var result = base.TranslateToShort(tableItem);
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Cards = DatabaseService.CardTranslator.GetByEmployee<DataAccess.Card>(tableItem.UID);
			result.Type = (PersonType)tableItem.Type;
			result.Appointed = tableItem.Appointed.ToString("d MMM yyyy");
			result.TabelNo = tableItem.TabelNo;
			result.TextColumns = DatabaseService.AdditionalColumnTranslator.GetTextColumns(tableItem.UID);
			result.Phone = tableItem.Phone;
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
			return result;
		}

		protected override void TranslateBack(DataAccess.Employee tableItem, Employee apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.FirstName = apiItem.FirstName;
			tableItem.SecondName = apiItem.SecondName;
			tableItem.LastName = apiItem.LastName;
			tableItem.Appointed = CheckDate(apiItem.Appointed);
			tableItem.PositionUID = apiItem.Position != null ? apiItem.Position.UID : Guid.Empty;
			tableItem.DepartmentUID = apiItem.Department != null ? apiItem.Department.UID : Guid.Empty;
			tableItem.ScheduleUID = apiItem.Schedule != null ? apiItem.Schedule.UID : Guid.Empty;
			tableItem.ScheduleStartDate = CheckDate(apiItem.ScheduleStartDate);
			tableItem.PhotoUID = apiItem.Photo != null ? apiItem.Photo.UID : Guid.Empty;
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
			tableItem.Phone = apiItem.Phone;
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
				result = result.And(e => e != null && filter.DepartmentUIDs.Contains(e.DepartmentUID.Value));
				result = result.And(e => e != null);
			}

			if (filter.PositionUIDs.IsNotNullOrEmpty())
				result = result.And(e => e != null && filter.PositionUIDs.Contains(e.PositionUID.Value));

			if (filter.ScheduleUIDs.IsNotNullOrEmpty())
				result = result.And(e => e != null && filter.ScheduleUIDs.Contains(e.PositionUID.Value));

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
	}
}