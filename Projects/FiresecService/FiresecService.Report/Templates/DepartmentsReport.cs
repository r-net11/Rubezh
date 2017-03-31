using Common;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using FiresecService.Report.Model;
using Localization.FiresecService.Report.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;

namespace FiresecService.Report.Templates
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
			get { return CommonResources.DepartList; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<DepartmentsReportFilter>();
			dataProvider.LoadCache();
			InitializeFilter(filter, dataProvider);
			var departments = GetDepartments(dataProvider, filter).ToList();
			var employees = dataProvider.GetEmployees(departments.Where(item => item.Item.ChiefUID != Guid.Empty).Select(item => item.Item.ChiefUID));
			var ds = new DepartmentsDataSet();
			departments.ForEach(department =>
			{
				var row = ds.Data.NewDataRow();
				row.Department = department.Name;
				row.Phone = department.Item.Phone;
				row.Chief = employees.Where(item => item.UID == department.Item.ChiefUID).Select(item => item.Name).FirstOrDefault();
				row.ParentDepartment = GetParentDepartmentName(department, dataProvider);
				row.Description = department.Item.Description;
				row.IsArchive = department.IsDeleted;
				var parents = GetParents(dataProvider, department);
				row.Level = parents.Count;
				row.Tag = string.Join("/", parents.Select(item => item.UID));
				ds.Data.AddDataRow(row);
			});
			return ds;
		}

		private void InitializeFilter(DepartmentsReportFilter filter, DataProvider dataProvider)
		{
			if (filter.IsDefault && filter.Organisations.IsEmpty())
			{
				var organisations = dataProvider.Organisations.Where(x => !x.Value.IsDeleted);
				filter.Organisations = organisations.Select(x => x.Key).ToList();
			}
		}

		private string GetParentDepartmentName(OrganisationBaseObjectInfo<Department> department, DataProvider dataProvider)
		{
			if (department.Item.ParentDepartmentUID.HasValue && dataProvider.Departments.ContainsKey(department.Item.ParentDepartmentUID.Value))
				return dataProvider.Departments[department.Item.ParentDepartmentUID.Value].Name;

			return string.Empty;
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
			return departments ?? new List<OrganisationBaseObjectInfo<Department>>();
		}

		protected override void ApplySort()
		{
			Detail.SortFields.Add(new GroupField("Level", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
			Detail.SortFields.Add(new GroupField("ParentDepartment", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
			Detail.SortFields.Add(new GroupField("Department", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
		}

		protected override void BeforeReportPrint()
		{
			base.BeforeReportPrint();

			this.xrTableCell16.Text = CommonResources.Level;
			this.xrTableCell11.Text = CommonResources.Department;
			this.xrTableCell12.Text = CommonResources.Phone;
			this.xrTableCell13.Text = CommonResources.Head;
			this.xrTableCell14.Text = CommonResources.UpstayDepart;
			this.xrTableCell15.Text = CommonResources.Note;
			this.xrTableCellArchiveHeader.Text = CommonResources.Archive;
		}

		private List<OrganisationBaseObjectInfo<Department>> GetParents(DataProvider dataProvider, OrganisationBaseObjectInfo<Department> department)
		{
			var parents = new List<OrganisationBaseObjectInfo<Department>>();

			for (var current = department; current.Item.ParentDepartmentUID.HasValue;)
			{
				if (dataProvider.Departments.ContainsKey(current.Item.ParentDepartmentUID.Value))
				{
					current = dataProvider.Departments[current.Item.ParentDepartmentUID.Value];
					parents.Insert(0, current);
				}
				else
					break;
			}

			parents.Add(department);
			return parents;
		}

		private void Report415_BeforePrint(object sender, PrintEventArgs e)
		{
			var filter = GetFilter<DepartmentsReportFilter>();

			if (filter.UseArchive) return;

			xrTableHeader.BeginInit();
			xrTableHeader.DeleteColumn(xrTableCellArchiveHeader);
			xrTableHeader.EndInit();
			xrTableContent.BeginInit();
			xrTableContent.DeleteColumn(xrTableCellArchive);
			xrTableContent.EndInit();
		}

		private void xrTableCellLevel_BeforePrint(object sender, PrintEventArgs e)
		{
			var level = GetCurrentColumnValue<int>("Level");
			((XRTableCell)sender).Padding = new PaddingInfo(level * 6, 0, 0, 0);
		}
	}
}