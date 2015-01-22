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

namespace FiresecService.Report.Templates
{
	public partial class Report415 : BaseSKDReport
	{
		public Report415()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Список подразделений организации"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet415();
		}
		protected override string BuildSelectRoutine()
		{
			return
				@"select 
							Department.Name as Department, 
							Department.Phone as Phone,  
							Employee.LastName as Chief,  
							ParentDepartment.Name as ParentDepartment,
							Department.Description as Description
						from dbo.Department Department
						left outer join dbo.Employee Employee on Employee.UID = Department.ChiefUID
						left outer join dbo.Department ParentDepartment on ParentDepartment.UID = Department.ParentDepartmentUID";
		}
		protected override string BuildWhereRouting()
		{
			var filter = GetFilter<ReportFilter415>();
			if (filter.Organisations.IsEmpty())
				filter.Organisations = new List<Guid>() { DataHelper.GetDefaultOrganisation() };
			var sb = new StringBuilder();
			SqlBuilder.BuildConditionOR(sb, "Department.OrganisationUID", filter.Organisations);
			if (!filter.Organisations.IsEmpty() && !filter.Departments.IsEmpty())
				sb.Append(SqlBuilder.AND);
			SqlBuilder.BuildConditionOR(sb, "Department.UID", filter.Departments);
			return sb.ToString();
		}
	}
}
