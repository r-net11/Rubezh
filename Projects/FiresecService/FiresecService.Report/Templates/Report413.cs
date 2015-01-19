using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;

namespace FiresecService.Report.Templates
{
	public partial class Report413 : BaseSKDReport
	{
		public Report413()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Права доступа сотрудников/посетителей"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet413();
		}

        protected override void UpdateDataSource()
        {
            base.UpdateDataSource();
            FillTestData();
        }
	}
}
