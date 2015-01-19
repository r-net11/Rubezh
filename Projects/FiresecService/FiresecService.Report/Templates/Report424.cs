using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;

namespace FiresecService.Report.Templates
{
    public partial class Report424: BaseSKDReport
	{
        public Report424()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Справка по отработанному времени"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet424();
		}

        protected override void UpdateDataSource()
        {
            base.UpdateDataSource();
            FillTestData();
        }
    }
}