using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;

namespace FiresecService.Report.Templates
{
	public partial class Report412 : BaseSKDReport
	{
		public Report412()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "412. Отчет по доступу в зоны"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet412();
		}
	}
}
