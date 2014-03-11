using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class EmployeeTranslator : OrganizationTranslatorBase<DataAccess.Employee, Employee, EmployeeFilter>
	{
		public EmployeeTranslator(DataAccess.SKUDDataContext context)
			: base(context)
		{
			
		}

		protected override OperationResult CanSave(Employee item)
		{
			bool sameName = Table.Any(x => x.FirstName == item.FirstName &&
				x.SecondName == item.SecondName &&
				x.LastName == item.LastName && 
				x.OrganizationUid == item.OrganizationUid &&
				x.UID != item.UID && 
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Сотрудник с таким же ФИО уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Employee item)
		{
			bool isAttendant = Context.Department.Any(x => !x.IsDeleted && x.AttendantUid == item.UID);
			if (isAttendant)
				return new OperationResult("Не могу удалить сотрудника, пока он указан как сопровождающий для одного из отделов");

			bool isContactEmployee = Context.Department.Any(x => !x.IsDeleted && x.ContactEmployeeUid == item.UID);
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

			var replacement = Context.EmployeeReplacement.Where(x => !x.IsDeleted && x.EmployeeUid == tableItem.UID).FirstOrDefault();
			Guid? replacementUID = null;
			if (replacement != null)
				replacementUID = replacement.UID;
			
			var cardUIDs = new List<Guid>();
			foreach (var card in Context.Card.Where(x => x.EmployeeUid == tableItem.UID))
				cardUIDs.Add(card.UID);
		
			result.FirstName = tableItem.FirstName;
			result.SecondName = tableItem.SecondName;
			result.LastName = tableItem.LastName;
			result.Appointed = tableItem.Appointed;
			result.Dismissed = tableItem.Dismissed;
			result.PositionUID = tableItem.PositionUid;
			result.ReplacementUID = replacementUID;
			result.DepartmentUID = tableItem.DepartmentUid;
			result.ScheduleUID = tableItem.ScheduleUid;
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
			tableItem.PositionUid = apiItem.PositionUID;
			tableItem.DepartmentUid = apiItem.DepartmentUID;
			tableItem.ScheduleUid = apiItem.ScheduleUID;
			tableItem.PhotoUID = apiItem.PhotoUID;
			tableItem.Type = (int)apiItem.Type;
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(EmployeeFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Employee>();
			result = result.And(base.IsInFilter(filter));

			var departmentUids = filter.DepartmentUids;
			if (departmentUids.IsNotNullOrEmpty())
				result = result.And(e => e!=null &&  departmentUids.Contains(e.DepartmentUid.Value));

			var positionUids = filter.PositionUids;
			if (positionUids.IsNotNullOrEmpty())
				result = result.And(e => e != null && positionUids.Contains(e.PositionUid.Value));

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
