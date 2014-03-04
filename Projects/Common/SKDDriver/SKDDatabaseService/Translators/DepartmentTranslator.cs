using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class DepartmentTranslator : OrganizationTranslatorBase<DataAccess.Department, Department, DepartmentFilter>
	{
		public DepartmentTranslator(Table<DataAccess.Department> table, DataAccess.SKUDDataContext context)
			: base(table, context)
		{
			
		}

		protected override OperationResult CanSave(Department item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUid == item.OrganizationUid && 
				x.Uid != item.UID && 
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Отдел с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Department item)
		{
			bool isHasEmployees = Context.Employee.Any(x => !x.IsDeleted && x.DepartmentUid == item.UID);
			if (isHasEmployees)
				return new OperationResult("Не могу удалить отдел, пока он указан содержит действующих сотрудников");

			if(item.ChildDepartmentUids.IsNotNullOrEmpty())
				return new OperationResult("Не могу удалить отдел, пока он содержит дочерние отделы");
			return base.CanSave(item);
		}

		protected override Department Translate(DataAccess.Department tableItem)
		{
			var result = base.Translate(tableItem);

			var phoneUids = new List<Guid>();
			foreach (var phone in Context.Phone.Where(x => !x.IsDeleted && x.DepartmentUid == tableItem.Uid))
			{
				phoneUids.Add(phone.Uid);
			} 
			var childDepartmentUids = new List<Guid>();
			foreach (var department in Context.Department.Where(x => !x.IsDeleted && x.ParentDepartmentUid == tableItem.Uid))
			{
				childDepartmentUids.Add(department.Uid);
			}

			tableItem.Department2.ToList().ForEach(x => childDepartmentUids.Add(x.Uid));
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.ParentDepartmentUid = tableItem.ParentDepartmentUid;
			result.ChildDepartmentUids = childDepartmentUids;
			result.ContactEmployeeUid = tableItem.ContactEmployeeUid;
			result.AttendantEmployeeUId = tableItem.AttendantUid;
			result.PhoneUids = phoneUids;
			return result;
		}

		protected override void TranslateBack(DataAccess.Department tableItem, Department apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.ParentDepartmentUid = apiItem.ParentDepartmentUid;
			tableItem.ContactEmployeeUid = apiItem.ContactEmployeeUid;
			tableItem.AttendantUid = apiItem.AttendantEmployeeUId;
		}
	}
}
