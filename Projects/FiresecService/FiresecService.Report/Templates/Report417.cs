using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;

namespace FiresecService.Report.Templates
{
    public partial class Report417 : BaseSKDReport
	{
        public Report417()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Местонахождение сотрудников/посетителей"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet417();
		}

        protected override void UpdateDataSource()
        {
            base.UpdateDataSource();
            FillTestData();
        }
    }
}
