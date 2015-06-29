using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

using LinqKit;
using SKDDriver.DataClasses;
using System.Data.Entity;

namespace SKDDriver
{
    public class DepartmentSynchroniser : Synchroniser<FiresecAPI.SKD.ExportDepartment, Department>
	{
		public DepartmentSynchroniser(DbSet<Department> table, DbService databaseService) : base(table, databaseService) { }

        public override FiresecAPI.SKD.ExportDepartment Translate(Department item)
		{
            return new FiresecAPI.SKD.ExportDepartment 
			{ 
				Name = item.Name, 
				Description = item.Description,	
				Phone = item.Phone,

				OrganisationUID = GetUID(item.OrganisationUID),
				OrganisationExternalKey = GetExternalKey(item.OrganisationUID, item.Organisation),
				ParentDepartmentUID = GetUID(item.ParentDepartmentUID),
				ParentDepartmentExternalKey = GetExternalKey(item.ParentDepartmentUID, item.ParentDepartment)
			};
		}

        //protected override Expression<Func<Department, bool>> IsInFilter(FiresecAPI.SKD.ExportFilter filter)
        //{
        //    return base.IsInFilter(filter).And(x => x.OrganisationUID == filter.OrganisationUID);
        //}

        protected override void UpdateForignKeys(FiresecAPI.SKD.ExportDepartment exportItem, Department tableItem)
		{
			tableItem.OrganisationUID = GetUIDbyExternalKey(exportItem.OrganisationExternalKey, _DatabaseService.Context.Organisations);
			tableItem.ParentDepartmentUID = GetUIDbyExternalKey(exportItem.ParentDepartmentExternalKey, _DatabaseService.Context.Departments);
			tableItem.ChiefUID = GetUIDbyExternalKey(exportItem.ChiefExternalKey, _DatabaseService.Context.Employees);
		}

        public override void TranslateBack(FiresecAPI.SKD.ExportDepartment exportItem, Department tableItem)
		{
			tableItem.Name = exportItem.Name;
			tableItem.Description = exportItem.Description; 
			tableItem.Phone = exportItem.Phone;
		}

        protected override void BeforeSave(List<FiresecAPI.SKD.ExportDepartment> exportItems)
		{
            var resultList = new List<FiresecAPI.SKD.ExportDepartment>();
			resultList = exportItems.Where(x => !exportItems.Any(y => y.ExternalKey == x.ParentDepartmentExternalKey)).ToList();
			while (resultList.Count < exportItems.Count)
			{
				resultList.AddRange(GetChildrenList(resultList, exportItems));
			}
			exportItems = resultList;
		}

        List<FiresecAPI.SKD.ExportDepartment> GetChildrenList(List<FiresecAPI.SKD.ExportDepartment> parentList, List<FiresecAPI.SKD.ExportDepartment> allList)
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
