using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using FiresecService.Report.DataSources;

namespace FiresecService.Report.Templates
{
    public partial class Report418 : BaseSKDReport
	{
        public Report418()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Справка о сотруднике/посетителе"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet418();
		}

        protected override void UpdateDataSource()
        {
            base.UpdateDataSource();
            FillTestData();
        }
    }
}