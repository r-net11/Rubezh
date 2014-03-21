using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace SKDDriver
{
	public class DepartmentTranslator : OrganizationElementTranslator<DataAccess.Department, Department, DepartmentFilter>
	{
		public DepartmentTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override OperationResult CanSave(Department item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Отдел с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Department item)
		{
			bool isHasEmployees = Context.Employee.Any(x => !x.IsDeleted && x.DepartmentUID == item.UID);
			if (isHasEmployees)
				return new OperationResult("Не могу удалить отдел, пока он указан содержит действующих сотрудников");

			if (item.ChildDepartmentUIDs.IsNotNullOrEmpty())
				return new OperationResult("Не могу удалить отдел, пока он содержит дочерние отделы");
			return base.CanSave(item);
		}

		protected override Department Translate(DataAccess.Department tableItem)
		{
			var result = base.Translate(tableItem);

			var phoneUIDs = new List<Guid>();
			foreach (var phone in Context.Phone.Where(x => !x.IsDeleted && x.DepartmentUID == tableItem.UID))
			{
				phoneUIDs.Add(phone.UID);
			}
			var childDepartmentUIDs = new List<Guid>();
			foreach (var department in Context.Department.Where(x => !x.IsDeleted && x.ParentDepartmentUID == tableItem.UID))
			{
				childDepartmentUIDs.Add(department.UID);
			}

			tableItem.Department2.ToList().ForEach(x => childDepartmentUIDs.Add(x.UID));
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.ParentDepartmentUID = tableItem.ParentDepartmentUID;
			result.ChildDepartmentUIDs = childDepartmentUIDs;
			result.ContactEmployeeUID = tableItem.ContactEmployeeUID;
			result.AttendantEmployeeUID = tableItem.AttendantUID;
			result.PhoneUIDs = phoneUIDs;
			result.PhotoUID = tableItem.PhotoUID;
			return result;
		}

		protected override void TranslateBack(DataAccess.Department tableItem, Department apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.ParentDepartmentUID = apiItem.ParentDepartmentUID;
			tableItem.ContactEmployeeUID = apiItem.ContactEmployeeUID;
			tableItem.AttendantUID = apiItem.AttendantEmployeeUID;
			tableItem.PhotoUID = apiItem.PhotoUID;
		}
	}
}