using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecService.Report.DataSources;
using FiresecService.Report.Model;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class DepartmentsReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<DepartmentsReportFilter>(f);
			var databaseService = new RubezhDAL.DataClasses.DbService();
			dataProvider.LoadCache();
			var departments = GetDepartments(dataProvider, filter);
			var uids = departments.Select(item => item.UID).ToList();
			var employees = dataProvider.GetEmployees(departments.Where(item => item.Item.ChiefUID != Guid.Empty).Select(item => item.Item.ChiefUID));
			var ds = new DepartmentsDataSet();
			departments.ForEach(department =>
			{
				var row = ds.Data.NewDataRow();
				row.Organisation = department.Organisation;
				row.Department = department.Name;
				row.Phone = department.Item.Phone;
				row.Chief = employees.Where(item => item.UID == department.Item.ChiefUID).Select(item => item.Name).FirstOrDefault();
				row.ParentDepartment = dataProvider.Departments.ContainsKey(department.Item.ParentDepartmentUID) ?
					dataProvider.Departments[department.Item.ParentDepartmentUID].Name : string.Empty;
				row.Description = department.Item.Description;
				row.IsArchive = department.IsDeleted;
				var parents = GetParents(dataProvider, department);
				row.Level = parents.Count;
				row.Tag = string.Join("/", parents.Select(item => item.UID));
				ds.Data.AddDataRow(row);
			});
			return ds;
		}

		private static List<OrganisationBaseObjectInfo<Department>> GetParents(DataProvider dataProvider, OrganisationBaseObjectInfo<Department> department)
		{
			var parents = new List<OrganisationBaseObjectInfo<Department>>();
			for (OrganisationBaseObjectInfo<Department> current = department; current.Item.ParentDepartmentUID != Guid.Empty;)
			{
				current = dataProvider.Departments[current.Item.ParentDepartmentUID];
				parents.Insert(0, current);
			}
			parents.Add(department);
			return parents;
		}

		private static IEnumerable<OrganisationBaseObjectInfo<Department>> GetDepartments(DataProvider dataProvider, DepartmentsReportFilter filter)
		{
			var organisationUID = Guid.Empty;
			var organisations = dataProvider.Organisations.Where(org => org.Value.Item.UserUIDs.Any(y => y == filter.UserUID));
			if (!filter.UseArchive)
				organisations = organisations.Where(org => !org.Value.IsDeleted);
			if (filter.Organisations.IsEmpty())
			{
				if (filter.IsDefault)
					organisationUID = organisations.FirstOrDefault().Key;
			}
			else
			{
				organisationUID = organisations.FirstOrDefault(org => org.Key == filter.Organisations.FirstOrDefault()).Key;
			}

			IEnumerable<OrganisationBaseObjectInfo<Department>> departments = null;
			if (organisationUID != Guid.Empty)
			{
				departments = dataProvider.Departments.Values.Where(item => item.OrganisationUID == organisationUID);

				if (!filter.UseArchive)
					departments = departments.Where(item => !item.IsDeleted);
				if (!filter.Departments.IsEmpty())
					departments = departments.Where(item => filter.Departments.Contains(item.UID));
			}
			return departments != null ? departments : new List<OrganisationBaseObjectInfo<Department>>();
		}
	}
}
