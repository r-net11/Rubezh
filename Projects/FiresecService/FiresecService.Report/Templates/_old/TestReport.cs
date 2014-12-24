using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Threading;
using FiresecAPI.SKD.ReportFilters;

namespace FiresecService.Report.Templates
{
	public partial class TestReport : DevExpress.XtraReports.UI.XtraReport, IFilteredReport
	{
		public TestReport()
		{
			InitializeComponent();
		}


		#region IFilteredReport Members

        public void ApplyFilter(SKDReportFilter args)
		{
            //var filter = args as TestReportFilter;
            //if (filter != null)
            //    Argument.Value = filter.Timestamp;
			//Thread.Sleep(5000);
		}

		#endregion
	}
}
