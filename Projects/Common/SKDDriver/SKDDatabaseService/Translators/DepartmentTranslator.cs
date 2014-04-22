using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace SKDDriver
{
	public class DepartmentTranslator : OrganizationElementTranslator<DataAccess.Department, Department, DepartmentFilter>
	{
		public DepartmentTranslator(DataAccess.SKDDataContext context, PhotoTranslator photoTranslator)
			: base(context)
		{
			PhotoTranslator = photoTranslator;
		}

		PhotoTranslator PhotoTranslator;

		protected override OperationResult CanSave(Department item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				x.ParentDepartmentUID == item.ParentDepartmentUID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Отдел с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Department item)
		{
			bool isHasEmployees = Context.Employees.Any(x => !x.IsDeleted && x.DepartmentUID == item.UID);
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
			foreach (var phone in Context.Phones.Where(x => !x.IsDeleted && x.DepartmentUID == tableItem.UID))
			{
				phoneUIDs.Add(phone.UID);
			}
			var childDepartmentUIDs = new List<Guid>();
			foreach (var department in Context.Departments.Where(x => !x.IsDeleted && x.ParentDepartmentUID == tableItem.UID))
			{
				childDepartmentUIDs.Add(department.UID);
			}
			tableItem.Departments.ToList().ForEach(x => childDepartmentUIDs.Add(x.UID));
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.ParentDepartmentUID = tableItem.ParentDepartmentUID;
			result.ChildDepartmentUIDs = childDepartmentUIDs;
			result.ContactEmployeeUID = tableItem.ContactEmployeeUID;
			result.AttendantEmployeeUID = tableItem.AttendantUID;
			result.PhoneUIDs = phoneUIDs;
			result.Photo = GetResult(PhotoTranslator.GetSingle(tableItem.PhotoUID));
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
			if(apiItem.Photo != null)
				tableItem.PhotoUID = apiItem.Photo.UID;
		}

		ShortDepartment TranslateToShort(DataAccess.Department tableItem)
		{
			var shortDepartment = new ShortDepartment
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				ParentDepartmentUID = tableItem.ParentDepartmentUID
			};
			shortDepartment.ChildDepartmentUIDs = new List<Guid>();
			foreach (var department in Context.Departments.Where(x => !x.IsDeleted && x.ParentDepartmentUID == tableItem.UID))
				shortDepartment.ChildDepartmentUIDs.Add(department.UID);
			return shortDepartment;
		}

		public OperationResult<IEnumerable<ShortDepartment>> GetList(DepartmentFilter filter)
		{
			try
			{
				var result = new List<ShortDepartment>();
				foreach (var tableItem in GetTableItems(filter))
				{
					var departmentListItem = TranslateToShort(tableItem);
					result.Add(departmentListItem);
				}
				var operationResult = new OperationResult<IEnumerable<ShortDepartment>>();
				operationResult.Result = result;
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult<IEnumerable<ShortDepartment>>(e.Message);
			}
		}

		public override OperationResult Save(IEnumerable<Department> apiItems)
		{
			foreach (var item in apiItems)
			{
				var photoSaveResult = PhotoTranslator.Save(new List<Photo> { item.Photo });
				if (photoSaveResult.HasError)
					return photoSaveResult;
			}
			return base.Save(apiItems);
		}
	}
}