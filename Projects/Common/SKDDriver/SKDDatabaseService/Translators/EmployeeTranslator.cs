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

			var additionalColumnUIDs = new List<Guid>();
			foreach (var additionalColumn in Context.AdditionalColumns.Where(x => !x.IsDeleted && x.EmployeeUID == tableItem.UID))
				additionalColumnUIDs.Add(additionalColumn.UID);

			var replacements = Context.EmployeeReplacements.Where(x => !x.IsDeleted && x.EmployeeUID == tableItem.UID);
			var replacementUIDs = new List<Guid>();
			foreach (var replacement in replacements)
				replacementUIDs.Add(replacement.UID);

			var cardUIDs = new List<Guid>();
			foreach (var card in Context.Cards.Where(x => x.EmployeeUID == tableItem.UID && !x.IsDeleted))
				cardUIDs.Add(card.UID);

			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Appointed = tableItem.Appointed;
			result.Dismissed = tableItem.Dismissed;
			result.PositionUID = tableItem.PositionUID;
			result.ReplacementUIDs = replacementUIDs;
			result.CurrentReplacement = EmployeeReplacementTranslator.GetCurrentReplacement(tableItem.UID);
			result.DepartmentUID = tableItem.DepartmentUID;
			result.ScheduleUID = tableItem.ScheduleUID;
			result.AdditionalColumnUIDs = additionalColumnUIDs;
			//result.AdditionalTextColumns = AdditionalColumnTranslator.Get()
			result.Type = (FiresecAPI.PersonType)tableItem.Type;
			result.CardUIDs = cardUIDs;
			result.PhotoUID = tableItem.PhotoUID;
			return result;
		}

		public override OperationResult<IEnumerable<Employee>> Get(EmployeeFilter filter)
		{
			var result = base.Get(filter);
			if (!result.HasError)
				AdditionalColumnTranslator.SetTextColumns(result);
			return result;
		}

		protected override void TranslateBack(DataAccess.Employee tableItem, Employee apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.FirstName = apiItem.FirstName;
			tableItem.SecondName = apiItem.SecondName;
			tableItem.LastName = apiItem.LastName;
			tableItem.Appointed = CheckDate(apiItem.Appointed);
			tableItem.Dismissed = CheckDate(apiItem.Dismissed);
			tableItem.PositionUID = apiItem.PositionUID;
			tableItem.DepartmentUID = apiItem.DepartmentUID;
			tableItem.ScheduleUID = apiItem.ScheduleUID;
			tableItem.PhotoUID = apiItem.PhotoUID;
			tableItem.Type = (int)apiItem.Type;
		}

		public OperationResult<EmployeeDetails> GetDetails(Guid uid)
		{
			var result = new OperationResult<EmployeeDetails>();
			try
			{
				var tableItem = Table.Where(x => x.UID == uid).FirstOrDefault();
				if (tableItem == null)
					return new OperationResult<EmployeeDetails>("Запись не найдена в таблице");
				var apiItem = TranslateOrganizationElement<EmployeeDetails, DataAccess.Employee>(tableItem);
				apiItem.FirstName = tableItem.FirstName;
				apiItem.SecondName = tableItem.SecondName;
				apiItem.LastName = tableItem.LastName;
				apiItem.Appointed = tableItem.Appointed;
				apiItem.Dismissed = tableItem.Dismissed;
				apiItem.Position = PositionTranslator.GetSingle(tableItem.PositionUID);
				apiItem.Replacements = EmployeeReplacementTranslator.GetByEmployee<DataAccess.EmployeeReplacement>(uid);
				apiItem.CurrentReplacement = EmployeeReplacementTranslator.GetCurrentReplacement(tableItem.UID);
				apiItem.Department = DepartmentTranslator.GetSingle(tableItem.DepartmentUID);
				//result.ScheduleUID = tableItem.ScheduleUID;
				apiItem.AdditionalColumns = AdditionalColumnTranslator.GetByEmployee<DataAccess.AdditionalColumn>(uid);
				apiItem.Type = (FiresecAPI.PersonType)tableItem.Type;
				apiItem.Cards = CardTranslator.GetByEmployee<DataAccess.Card>(uid);
				apiItem.Photo = PhotoTranslator.GetSingle(tableItem.PhotoUID);
				
				result.Result = apiItem;
				return result;
			}
			catch (Exception e)
			{
				return new OperationResult<EmployeeDetails>(e.Message);
			}
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(EmployeeFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Employee>();
			result = result.And(base.IsInFilter(filter));

			result = result.And(e => e.Type == (int?)filter.PersonType);

			var isReplaced = PredicateBuilder.True<DataAccess.EmployeeReplacement>();

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