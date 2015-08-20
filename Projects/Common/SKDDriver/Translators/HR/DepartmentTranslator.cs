using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class DepartmentTranslator : OrganisationItemTranslatorBase<Department, API.Department, API.DepartmentFilter>
	{
		DataContractSerializer _serializer;
		public DepartmentShortTranslator ShortTranslator { get; private set; }
		public DepartmentAsyncTranslator AsyncTranslator { get; private set; }
		public DepartmentSynchroniser Synchroniser { get; private set; }
		public DepartmentTranslator(DbService context)
			: base(context)
		{
			_serializer = new DataContractSerializer(typeof(API.Department));
			ShortTranslator = new DepartmentShortTranslator(this);
			AsyncTranslator = new DepartmentAsyncTranslator(ShortTranslator);
			Synchroniser = new DepartmentSynchroniser(Table, DbService);
		}

		public override DbSet<Department> Table
		{
			get { return Context.Departments; }
		}

		public override System.Linq.IQueryable<Department> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.Photo).Include(x => x.ChildDepartments);
		}

		protected override IEnumerable<API.Department> GetAPIItems(IQueryable<Department> tableItems)
		{
			return tableItems.Select(tableItem => new API.Department
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				Photo = tableItem.Photo != null ? new API.Photo { UID = tableItem.Photo.UID, Data = tableItem.Photo.Data } : null,
				ParentDepartmentUID = tableItem.ParentDepartmentUID != null ? tableItem.ParentDepartmentUID.Value : Guid.Empty,
				ChildDepartmentUIDs = tableItem.ChildDepartments.Select(x => x.UID).ToList(),
				Phone = tableItem.Phone
			});
		}

		public override void TranslateBack(API.Department apiItem, Department tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Photo = Photo.Create(apiItem.Photo);
			tableItem.ParentDepartmentUID = apiItem.ParentDepartmentUID.EmptyToNull();
			tableItem.ChiefUID = apiItem.ChiefUID.EmptyToNull();
			tableItem.Phone = apiItem.Phone;
		}

		protected override void ClearDependentData(Department tableItem)
		{
			if (tableItem.Photo != null)
				Context.Photos.Remove(tableItem.Photo);
		}

		protected override OperationResult<bool> CanSave(API.Department item)
		{
			if (item == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			if (item.OrganisationUID == Guid.Empty)
				return OperationResult<bool>.FromError("Не указана организация");
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.ParentDepartmentUID == item.ParentDepartmentUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			else
				return new OperationResult<bool>(true);
		}

		protected override void AfterDelete(Department tableItem)
		{
			base.AfterDelete(tableItem);
			MarkDeletedByParent(tableItem.UID, tableItem.RemovalDate.GetValueOrDefault());
		}

		protected override void BeforeRestore(Department tableItem)
		{
			base.BeforeRestore(tableItem);
			RestoreByChild(tableItem);
		}

		void MarkDeletedByParent(Guid uid, DateTime removalDate)
		{
			var items = new List<Department>();
			var currentItems = Table.Where(x => x.ParentDepartmentUID == uid).ToList();
			items.AddRange(currentItems);
			while (currentItems.Count > 0)
			{
				var childrenItems = new List<Department>();
				foreach (var item in currentItems)
				{
					var childItems = Table.Where(x => x.ParentDepartmentUID == item.UID).ToList();
					childrenItems.AddRange(childItems);
					items.AddRange(childItems);
				}
				currentItems = childrenItems;
			}
			foreach (var item in items)
			{
				item.IsDeleted = true;
				item.RemovalDate = removalDate;
			}
		}

		void RestoreByChild(Department tableItem)
		{
			var items = new List<Department>();
			var parent = tableItem;
			while (parent != null)
			{
				parent = Table.FirstOrDefault(x => x.UID == parent.ParentDepartmentUID);
				if (parent != null)
					items.Add(parent);
			}
			foreach (var item in items)
			{
				item.IsDeleted = true;
				item.RemovalDate = tableItem.RemovalDate;
			}
		}

		public OperationResult SaveChief(Guid uid, Guid? chiefUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem == null)
					return new OperationResult("Запись не найдена");
				tableItem.ChiefUID = chiefUID != null ? chiefUID.Value.EmptyToNull() : null;
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			try
			{
				var result = new List<Guid>();
				var tableItem = Table.Include(x => x.ChildDepartments.Select(y => y.Employees)).FirstOrDefault(x => x.UID == uid);
				result.AddRange(tableItem.Employees.Select(x => x.UID));
				result.AddRange(tableItem.ChildDepartments.SelectMany(x => x.Employees.Select(y => y.UID)));
				return new OperationResult<List<Guid>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<Guid>>.FromError(e.Message);
			}
		}

		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			try
			{
				var result = new List<Guid>();
				var parentItem = Table.Include(x => x.ChildDepartments.Select(y => y.Employees)).FirstOrDefault(x => x.UID == uid);
				bool isRoot = true;
				while (isRoot)
				{
					isRoot = parentItem.ParentDepartment != null;
					result.AddRange(parentItem.Employees.Select(x => x.UID));
					parentItem = parentItem.ParentDepartment;
				}
				return new OperationResult<List<Guid>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<Guid>>.FromError(e.Message);
			}
		}
	}

	public class DepartmentShortTranslator : OrganisationShortTranslatorBase<Department, API.ShortDepartment, API.Department, API.DepartmentFilter>
	{
		public DepartmentShortTranslator(DepartmentTranslator translator) : base(translator) { }

		public override IQueryable<Department> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.ChildDepartments);
		}

		public API.ShortDepartment Translate(Department tableItem)
		{
			if (tableItem == null)
				return null;
			var result = new API.ShortDepartment
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate.GetValueOrDefault(),
				OrganisationUID = tableItem.OrganisationUID.GetValueOrDefault()
			};
			result.ChiefUID = tableItem.ChiefUID.GetValueOrDefault();
			result.Phone = tableItem.Phone;
			result.ParentDepartmentUID = tableItem.ParentDepartmentUID.GetValueOrDefault();
			result.ChildDepartments = tableItem.ChildDepartments.Select(x => new API.TinyDepartment { UID = x.UID, Name = x.Name }).ToList();
			return result;
		}

		protected override IEnumerable<API.ShortDepartment> GetAPIItems(IQueryable<Department> tableItems)
		{
			return tableItems.Select(tableItem =>
				new API.ShortDepartment
				{
					UID = tableItem.UID,
					Name = tableItem.Name,
					Description = tableItem.Description,
					IsDeleted = tableItem.IsDeleted,
					RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
					OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
					ChiefUID = tableItem.ChiefUID != null ? tableItem.ChiefUID.Value : Guid.Empty,
					Phone = tableItem.Phone,
					ParentDepartmentUID = tableItem.ParentDepartmentUID != null ? tableItem.ParentDepartmentUID.Value : Guid.Empty,
					ChildDepartments = tableItem.ChildDepartments.Select(x => new API.TinyDepartment { UID = x.UID, Name = x.Name }).ToList()
				});
		}
	}

	public class DepartmentAsyncTranslator : AsyncTranslator<Department, API.ShortDepartment, API.DepartmentFilter>
	{
		public DepartmentAsyncTranslator(DepartmentShortTranslator translator) : base(translator as ITranslatorGet<Department, API.ShortDepartment, API.DepartmentFilter>) { }
		public override List<API.ShortDepartment> GetCollection(DbCallbackResult callbackResult)
		{
			return callbackResult.Departments;
		}
	}
}