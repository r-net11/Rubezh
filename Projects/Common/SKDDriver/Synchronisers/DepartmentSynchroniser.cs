using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

using LinqKit;
using RubezhDAL.DataClasses;
using System.Data.Entity;

namespace RubezhDAL
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

		protected override IQueryable<Department> GetFilteredItems(FiresecAPI.SKD.ExportFilter filter)
		{
			return base.GetFilteredItems(filter).Where(x => x.OrganisationUID == filter.OrganisationUID);
		}

        protected override void UpdateForignKeys(FiresecAPI.SKD.ExportDepartment exportItem, Department tableItem, OrganisationHRCash hrCash)
		{
			tableItem.OrganisationUID = hrCash.OrganisationUID;
			tableItem.ParentDepartmentUID = GetUIDbyExternalKey(exportItem.ParentDepartmentExternalKey, hrCash.Departments);
			tableItem.ChiefUID = GetUIDbyExternalKey(exportItem.ChiefExternalKey, hrCash.Employees);
		}

        public override void TranslateBack(FiresecAPI.SKD.ExportDepartment exportItem, Department tableItem)
		{
			tableItem.Name = exportItem.Name;
			tableItem.Description = exportItem.Description; 
			tableItem.Phone = exportItem.Phone;
			tableItem.IsDeleted = exportItem.IsDeleted;
			tableItem.RemovalDate = exportItem.RemovalDate;
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
