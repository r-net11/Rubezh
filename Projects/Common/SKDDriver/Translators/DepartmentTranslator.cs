using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;

namespace SKDDriver
{
	public class DepartmentTranslator : WithShortTranslator<DataAccess.Department, Department, DepartmentFilter, ShortDepartment>
	{
		public DepartmentTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override OperationResult CanSave(Department item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				x.ParentDepartmentUID == item.ParentDepartmentUID &&
				x.IsDeleted == false);
			if (hasSameName)
				return new OperationResult("Отдел с таким же названием уже содержится в базе данных");
			else
				return new OperationResult();
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			bool isHasEmployees = Context.Employees.Any(x => !x.IsDeleted && x.DepartmentUID == uid);
			if (isHasEmployees)
				return new OperationResult("Невозможно удалить отдел, пока он содержит действующих сотрудников");

			bool isHasChildren = Table.Any(x => !x.IsDeleted && x.ParentDepartmentUID == uid);
			if (isHasChildren)
				return new OperationResult("Невозможно удалить отдел, пока он содержит дочерние отделы");
			return base.CanDelete(uid);
		}



		protected override Department Translate(DataAccess.Department tableItem)
		{
			var result = base.Translate(tableItem);

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
			result.Photo = GetResult(DatabaseService.PhotoTranslator.GetSingle(tableItem.PhotoUID));
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

		protected override ShortDepartment TranslateToShort(DataAccess.Department tableItem)
		{
			var shortDepartment = new ShortDepartment
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				ParentDepartmentUID = tableItem.ParentDepartmentUID,
				OrganisationUID = tableItem.OrganisationUID.HasValue ? tableItem.OrganisationUID.Value : Guid.Empty
			};
			shortDepartment.ChildDepartmentUIDs = new List<Guid>();
			foreach (var department in Context.Departments.Where(x => !x.IsDeleted && x.ParentDepartmentUID == tableItem.UID))
				shortDepartment.ChildDepartmentUIDs.Add(department.UID);
			return shortDepartment;
		}

		public string GetName(Guid? uid)
		{
			if (uid == null)
				return "";
			var tableItem = Table.FirstOrDefault(x => x.UID == uid.Value);
			if(tableItem == null)
				return "";
			return tableItem.Name;
		}

		public override OperationResult Save(Department apiItem)
		{
			if (apiItem.Photo != null && apiItem.Photo.Data != null && apiItem.Photo.Data.Count() > 0)
			{
				var photoSaveResult = DatabaseService.PhotoTranslator.Save(apiItem.Photo);
				if (photoSaveResult.HasError)
					return photoSaveResult;
			}
			return base.Save(apiItem);
		}
	}
}