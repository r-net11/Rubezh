using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;

namespace FiresecService.Report.Templates
{
	public partial class Report411 : BaseSKDReport
	{
		public Report411()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "411. Отчет по пропускам"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet411();
		}
	}
}
