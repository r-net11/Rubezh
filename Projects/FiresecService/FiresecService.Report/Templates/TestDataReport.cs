using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using SKDDriver;
using FiresecService.Report.DataSources;
using FiresecService.Report.DataSources.TestDataReportDataSetTableAdapters;
using System.Data.SqlClient;
using Infrastructure.Common.SKDReports.Filters;
using System.Threading;

namespace FiresecService.Report.Templates
{
	public partial class TestDataReport : DevExpress.XtraReports.UI.XtraReport, IFilteredReport
	{
		public TestDataReport()
		{
			InitializeComponent();
		}


		#region IFilteredReport Members

		public void ApplyFilter(object args)
		{
			var filter = args as TestReportFilter;
			if (filter != null)
				timestamp.Value = filter.Timestamp;
			if (args is bool)
			{
				if ((bool)args)
					Thread.Sleep(5000);
			}
			using (var connection = new SqlConnection(@"Data Source=.\\SQLEXPRESS;Initial Catalog=SKD;Integrated Security=True"))
			{
				var ds = new TestDataReportDataSet();
				var orgAdapter = new OrganisationTableAdapter();
				orgAdapter.Fill(ds.Organisation);
				var empAdapter = new EmployeeTableAdapter();
				empAdapter.Fill(ds.Employee);

				DataSource = ds;
			}
		}

		#endregion
	}
}
