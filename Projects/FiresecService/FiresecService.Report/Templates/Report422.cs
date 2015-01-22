using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;
using SKDDriver;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using System.Collections.Generic;

namespace FiresecService.Report.Templates
{
	public partial class Report422 : BaseSKDReport
	{
        public Report422()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Отчет по графикам работы"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet422();
		}
        protected override void DataSourceRequered()
        {
            base.DataSourceRequered();
            FillTestData();
        }
    }
}