using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecAPI.SKD.ReportFilters;

namespace FiresecService.Report.Templates
{
	public partial class Report415 : DevExpress.XtraReports.UI.XtraReport, IFilteredReport
	{
		public Report415()
		{
			InitializeComponent();
		}


		#region IFilteredReport Members

		public void ApplyFilter(object filter)
		{
			var reportFilter = (ReportFilter415)filter;
			if (reportFilter.Organisations != null)
				organisation.Value = reportFilter.Organisations.FirstOrDefault();
		}

		#endregion

		private void Report415_DataSourceDemanded(object sender, EventArgs e)
		{

		}
	}
}
