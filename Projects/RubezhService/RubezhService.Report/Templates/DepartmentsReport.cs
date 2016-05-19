using Common;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using RubezhService.Report.DataSources;
using RubezhService.Report.Model;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;

namespace RubezhService.Report.Templates
{
	public partial class DepartmentsReport : BaseReport
	{
		public DepartmentsReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Портретная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return false; }
		}

		public override string ReportTitle
		{
			get { return "Список подразделений организации"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<DepartmentsReportFilter>();
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

		private static IEnumerable<OrganisationBaseObjectInfo<Department>> GetDepartments(DataProvider dataProvider, DepartmentsReportFilter filter)
		{
			var organisationUID = Guid.Empty;
			var organisations = dataProvider.Organisations.Where(org => filter.User == null || filter.User.IsAdm || org.Value.Item.UserUIDs.Any(y => y == filter.User.UID));
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
		protected override void ApplySort()
		{
			Detail.SortFields.Add(new GroupField("Level", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
			Detail.SortFields.Add(new GroupField("ParentDepartment", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
			Detail.SortFields.Add(new GroupField("Department", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
		}

		private List<OrganisationBaseObjectInfo<Department>> GetParents(DataProvider dataProvider, OrganisationBaseObjectInfo<Department> department)
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

		private void Report415_BeforePrint(object sender, PrintEventArgs e)
		{
			var filter = GetFilter<DepartmentsReportFilter>();
			if (!filter.UseArchive)
			{
				xrTableHeader.BeginInit();
				xrTableHeader.DeleteColumn(xrTableCellArchiveHeader);
				xrTableHeader.EndInit();
				xrTableContent.BeginInit();
				xrTableContent.DeleteColumn(xrTableCellArchive);
				xrTableContent.EndInit();
			}
		}
		private void xrTableCellLevel_BeforePrint(object sender, PrintEventArgs e)
		{
			var level = GetCurrentColumnValue<int>("Level");
			((XRTableCell)sender).Padding = new PaddingInfo(level * 6, 0, 0, 0);
		}
	}
}