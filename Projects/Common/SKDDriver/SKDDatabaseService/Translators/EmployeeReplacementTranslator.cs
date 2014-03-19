using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class EmployeeReplacementTranslator : OrganizationElementTranslator<DataAccess.EmployeeReplacement, EmployeeReplacement, EmployeeReplacementFilter>
	{
		public EmployeeReplacementTranslator(DataAccess.SKUDDataContext context)
			: base(context)
		{

		}

		public EmployeeReplacement GetCurrentReplacement(Guid employeeUID)
		{
			EmployeeReplacement result = null;
			var tableItem = Table.Where(x => !x.IsDeleted && 
				x.EmployeeUID == employeeUID && 
				DateTime.Now >= x.BeginDate && 
				DateTime.Now <= x.EndDate).FirstOrDefault();
			if (tableItem != null)
				result = Translate(tableItem);
			return result;
		}
		
		protected override OperationResult CanSave(EmployeeReplacement item)
		{
			bool notUinque = Table.Any(x => x.EmployeeUID == item.EmployeeUID &&
				x.UID == item.UID && 
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				(x.BeginDate >= item.DateTimePeriod.StartDate || x.EndDate <= item.DateTimePeriod.EndDate) && 
				x.IsDeleted == false);
			if (notUinque)
				return new OperationResult("Для данного пользователя уже существует замена данных в данный временной промежуток");
			return base.CanSave(item);
		}

		protected override EmployeeReplacement Translate(DataAccess.EmployeeReplacement tableItem)
		{
			var result = base.Translate(tableItem);
			result.DateTimePeriod.StartDate = tableItem.BeginDate;
			result.DateTimePeriod.EndDate = tableItem.EndDate;
			result.EmployeeUID = tableItem.EmployeeUID;
			result.DepartmentUID = tableItem.DepartmentUID;
			result.ScheduleUID = tableItem.ScheduleUID;
			return result;
		}

		protected override void TranslateBack(DataAccess.EmployeeReplacement tableItem, EmployeeReplacement apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.ScheduleUID = apiItem.ScheduleUID;
			tableItem.BeginDate = apiItem.DateTimePeriod.StartDate;
			tableItem.EndDate = apiItem.DateTimePeriod.EndDate;
			tableItem.EmployeeUID = apiItem.EmployeeUID;
			tableItem.DepartmentUID = apiItem.DepartmentUID;
		}

		protected override Expression<Func<DataAccess.EmployeeReplacement, bool>> IsInFilter(EmployeeReplacementFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.EmployeeReplacement>();
			result = result.And(base.IsInFilter(filter));
			var employeeUIDs = filter.EmployeeUIDs;
			if (employeeUIDs.IsNotNullOrEmpty())
				result = result.And(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
			var departmentUIDs = filter.DepartmentUIDs;
			if (departmentUIDs.IsNotNullOrEmpty())
				result = result.And(e => e.DepartmentUID != null && departmentUIDs.Contains(e.DepartmentUID.Value));
			var scheduleUIDs = filter.ScheduleUIDs;
			if (scheduleUIDs.IsNotNullOrEmpty())
				result = result.And(e => e.ScheduleUID != null && scheduleUIDs.Contains(e.ScheduleUID.Value));
			var beginDates = filter.ReplacementStartDates;
			if (beginDates != null)
				result = result.And(e => e.BeginDate >= beginDates.StartDate && e.BeginDate <= beginDates.EndDate);
			var endDates = filter.ReplacementStartDates;
			if (endDates != null)
				result = result.And(e => e.EndDate >= endDates.StartDate && e.EndDate <= endDates.EndDate);
			return result;
		}

	}
}
