using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public class DepartmentSynchroniser : Synchroniser<ExportDepartment, DataAccess.Department>
	{
		public DepartmentSynchroniser(Table<DataAccess.Department> table, SKDDatabaseService databaseService)
			: base(table, databaseService)
		{
		}

		public override ExportDepartment Translate(DataAccess.Department item)
		{
			return new ExportDepartment
			{
				Name = item.Name,
				Description = item.Description,
				Phone = item.Phone,

				OrganisationUID = GetUID(item.OrganisationUID),
				OrganisationExternalKey = GetExternalKey(item.OrganisationUID, item.Organisation),
				ParentDepartmentUID = GetUID(item.ParentDepartmentUID),
				ParentDepartmentExternalKey = GetExternalKey(item.ParentDepartmentUID, item.Department1),
				ContactEmployeeUID = GetUID(item.ContactEmployeeUID),
				ContactEmployeeExternalKey = GetExternalKey(item.ContactEmployeeUID, item.Employee2),
				AttendantUID = GetUID(item.AttendantUID),
				AttendantExternalKey = GetExternalKey(item.AttendantUID, item.Employee1),
				ChiefUID = GetUID(item.ChiefUID),
				ChiefExternalKey = GetExternalKey(item.ChiefUID, item.Employee),
			};
		}

		protected override Expression<Func<DataAccess.Department, bool>> IsInFilter(ExportFilter filter)
		{
			return base.IsInFilter(filter).And(x => x.OrganisationUID == filter.OrganisationUID);
		}

		protected override void UpdateForignKeys(ExportDepartment exportItem, DataAccess.Department tableItem)
		{
			tableItem.OrganisationUID = GetUIDbyExternalKey(exportItem.OrganisationExternalKey, _DatabaseService.Context.Organisations);
			tableItem.ParentDepartmentUID = GetUIDbyExternalKey(exportItem.ParentDepartmentExternalKey, _DatabaseService.Context.Departments);
			tableItem.ContactEmployeeUID = GetUIDbyExternalKey(exportItem.ContactEmployeeExternalKey, _DatabaseService.Context.Employees);
			tableItem.AttendantUID = GetUIDbyExternalKey(exportItem.AttendantExternalKey, _DatabaseService.Context.Employees);
			tableItem.ChiefUID = GetUIDbyExternalKey(exportItem.ChiefExternalKey, _DatabaseService.Context.Employees);
		}

		public override void TranslateBack(ExportDepartment exportItem, DataAccess.Department tableItem)
		{
			tableItem.Name = exportItem.Name;
			tableItem.Description = exportItem.Description;
			tableItem.Phone = exportItem.Phone;
		}

		protected override void BeforeSave(List<ExportDepartment> exportItems)
		{
			var resultList = new List<ExportDepartment>();
			resultList = exportItems.Where(x => !exportItems.Any(y => y.ExternalKey == x.ParentDepartmentExternalKey)).ToList();
			while (resultList.Count < exportItems.Count)
			{
				resultList.AddRange(GetChildrenList(resultList, exportItems));
			}
			exportItems = resultList;
		}

		private List<ExportDepartment> GetChildrenList(List<ExportDepartment> parentList, List<ExportDepartment> allList)
		{
			var result = allList.Where(x => parentList.Any(y => y.ExternalKey == x.ParentDepartmentExternalKey)).ToList();
			return result;
		}

		protected override string Name
		{
			get { return "Departments"; }
		}

		protected override string XmlHeaderName
		{
			get { return "ArrayOfExportDepartment"; }
		}
	}
}