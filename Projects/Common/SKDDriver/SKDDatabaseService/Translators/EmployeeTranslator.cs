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
		public EmployeeTranslator(DataAccess.SKUDDataContext context, EmployeeReplacementTranslator replacementTranslator)
			: base(context)
		{
			ReplacementTranslator = replacementTranslator;
		}

		EmployeeReplacementTranslator ReplacementTranslator;

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
			bool isAttendant = Context.Department.Any(x => !x.IsDeleted && x.AttendantUID == item.UID);
			if (isAttendant)
				return new OperationResult("Не могу удалить сотрудника, пока он указан как сопровождающий для одного из отделов");

			bool isContactEmployee = Context.Department.Any(x => !x.IsDeleted && x.ContactEmployeeUID == item.UID);
			if (isContactEmployee)
				return new OperationResult("Не могу удалить сотрудника, пока он указан как контактное лицо для одного из отделов");
			return base.CanSave(item);
		}

		protected override Employee Translate(DataAccess.Employee tableItem)
		{
			var result = base.Translate(tableItem);

			var additionalColumnUIDs = new List<Guid>();
			foreach (var additionalColumn in Context.AdditionalColumn.Where(x => !x.IsDeleted && x.EmployeeUID == tableItem.UID))
				additionalColumnUIDs.Add(additionalColumn.UID);

			var replacements = Context.EmployeeReplacement.Where(x => !x.IsDeleted && x.EmployeeUID == tableItem.UID);
			var replacementUIDs = new List<Guid>();
			foreach (var replacement in replacements)
				replacementUIDs.Add(replacement.UID);
			
			var cardUIDs = new List<Guid>();
			foreach (var card in Context.Card.Where(x => x.EmployeeUID == tableItem.UID && !x.IsDeleted))
				cardUIDs.Add(card.UID);
		
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Appointed = tableItem.Appointed;
			result.Dismissed = tableItem.Dismissed;
			result.PositionUID = tableItem.PositionUID;
			result.ReplacementUIDs = replacementUIDs;
			result.CurrentReplacement = ReplacementTranslator.GetCurrentReplacement(tableItem.UID);
			result.DepartmentUID = tableItem.DepartmentUID;
			result.ScheduleUID = tableItem.ScheduleUID;
			result.CardTemplateUID = tableItem.CardTemplateUID;
			result.AdditionalColumnUIDs = additionalColumnUIDs;
			result.Type = (FiresecAPI.PersonType)tableItem.Type;
			result.CardUIDs = cardUIDs;
			result.PhotoUID = tableItem.PhotoUID;
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
			tableItem.CardTemplateUID = apiItem.CardTemplateUID;
		}

		public OperationResult SaveCardTemplate(Employee apiItem)
		{
			try
			{
				var tableItem = Table.Where(x => x.UID == apiItem.UID).FirstOrDefault();
				if (tableItem == null)
					return new OperationResult("Сотрудник не найден в базе данных");
				tableItem.CardTemplateUID = apiItem.CardTemplateUID;
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);	
			}
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(EmployeeFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Employee>();
			result = result.And(base.IsInFilter(filter));

			var isReplaced = PredicateBuilder.True<DataAccess.EmployeeReplacement>();

			var departmentUIDs = filter.DepartmentUIDs;
			if (departmentUIDs.IsNotNullOrEmpty())
			{
				result = result.And(e => 
					e!=null && 
					(Context.EmployeeReplacement.Any(x => 
						!x.IsDeleted && 
						x.EmployeeUID == e.UID && 
						DateTime.Now >= x.BeginDate && 
						DateTime.Now <= x.EndDate && 
						departmentUIDs.Contains(x.DepartmentUID.Value)
						) ||
						(!Context.EmployeeReplacement.Any(x => 
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
