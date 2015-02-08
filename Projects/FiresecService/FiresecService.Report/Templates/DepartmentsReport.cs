using Common;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using SKDDriver;
using DevExpress.XtraReports.UI;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
using FiresecService.Report.Model;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;

namespace FiresecService.Report.Templates
{
	public partial class DepartmentsReport : BaseReport
	{
		public DepartmentsReport()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Список подразделений организации"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<DepartmentsReportFilter>();
			var databaseService = new SKDDatabaseService();
			dataProvider.LoadCache();
			var organisationUID = filter.Organisations.IsEmpty() ? dataProvider.Organisations.First(org => !org.Value.IsDeleted).Value.UID : filter.Organisations.First();
			var departments = dataProvider.Departments.Values.Where(item => item.OrganisationUID == organisationUID);
			if (!filter.UseArchive)
				departments = departments.Where(item => !item.IsDeleted);
			if (!filter.Departments.IsEmpty())
				departments = departments.Where(item => filter.Departments.Contains(item.UID));

			var uids = departments.Select(item => item.UID).ToList();
			var employees = dataProvider.GetEmployees(departments.Where(item => item.Item.ChiefUID != Guid.Empty).Select(item => item.Item.ChiefUID));
			var ds = new DepartmentsDataSet();
			departments.ForEach(department =>
			{
				var row = ds.Data.NewDataRow();
				row.Department = department.Name;
				row.Phone = department.Item.Phone;
				row.Chief = employees.Where(item => item.UID == department.Item.ChiefUID).Select(item => item.Name).FirstOrDefault();
				row.ParentDepartment = department.Item.ParentDepartmentUID.HasValue ? dataProvider.Departments[department.Item.ParentDepartmentUID.Value].Name : string.Empty;
				row.Description = department.Item.Description;
				row.IsArchive = department.IsDeleted;
				var parents = GetParents(dataProvider, department);
				row.Level = parents.Count;
				row.Tag = string.Join("/", parents.Select(item => item.UID));
				ds.Data.AddDataRow(row);
			});
			return ds;
		}
		protected override void ApplySort()
		{
			Detail.SortFields.Add(new GroupField("Tag", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
			Detail.SortFields.Add(new GroupField("Department", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
		}

		private List<OrganisationBaseObjectInfo<Department>> GetParents(DataProvider dataProvider, OrganisationBaseObjectInfo<Department> department)
		{
			var parents = new List<OrganisationBaseObjectInfo<Department>>();
			for (OrganisationBaseObjectInfo<Department> current = department; current.Item.ParentDepartmentUID.HasValue; )
			{
				current = dataProvider.Departments[current.Item.ParentDepartmentUID.Value];
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
			((XRTableCell)sender).Padding = new PaddingInfo((level - 1) * 3, 0, 0, 0);
		}
	}
}
