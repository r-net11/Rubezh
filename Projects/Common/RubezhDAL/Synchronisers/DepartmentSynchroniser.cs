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
	public class DepartmentSynchroniser : Synchroniser<RubezhAPI.SKD.ExportDepartment, Department>
	{
		public DepartmentSynchroniser(DbSet<Department> table, DbService databaseService) : base(table, databaseService) { }

		public override RubezhAPI.SKD.ExportDepartment Translate(Department item)
		{
			return new RubezhAPI.SKD.ExportDepartment 
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

		protected override IQueryable<Department> GetFilteredItems(RubezhAPI.SKD.ExportFilter filter)
		{
			return base.GetFilteredItems(filter).Where(x => x.OrganisationUID == filter.OrganisationUID);
		}

		protected override void UpdateForignKeys(RubezhAPI.SKD.ExportDepartment exportItem, Department tableItem, OrganisationHRCash hrCash)
		{
			tableItem.OrganisationUID = hrCash.OrganisationUID;
			tableItem.ParentDepartmentUID = GetUIDbyExternalKey(exportItem.ParentDepartmentExternalKey, hrCash.Departments);
			tableItem.ChiefUID = GetUIDbyExternalKey(exportItem.ChiefExternalKey, hrCash.Employees);
		}

		public override void TranslateBack(RubezhAPI.SKD.ExportDepartment exportItem, Department tableItem)
		{
			tableItem.Name = exportItem.Name;
			tableItem.Description = exportItem.Description; 
			tableItem.Phone = exportItem.Phone;
			tableItem.IsDeleted = exportItem.IsDeleted;
			tableItem.RemovalDate = exportItem.RemovalDate;
		}

		protected override void BeforeSave(List<RubezhAPI.SKD.ExportDepartment> exportItems)
		{
			var resultList = new List<RubezhAPI.SKD.ExportDepartment>();
			resultList = exportItems.Where(x => !exportItems.Any(y => y.ExternalKey == x.ParentDepartmentExternalKey)).ToList();
			while (resultList.Count < exportItems.Count)
			{
				resultList.AddRange(GetChildrenList(resultList, exportItems));
			}
			exportItems = resultList;
		}

		List<RubezhAPI.SKD.ExportDepartment> GetChildrenList(List<RubezhAPI.SKD.ExportDepartment> parentList, List<RubezhAPI.SKD.ExportDepartment> allList)
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
