using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using SKDModule.PassCardDesigner.Model;

namespace SKDModule.Employees.Views.DialogWindows
{
	public partial class ReportPreviewControl : UserControl
	{
		public ReportPreviewControl()
		{
			InitializeComponent();
			var report = XtraReport.FromFile(@"D:\Repositories\ReportsApplication1\TestingReports\bin\Debug\Report3.repx", true);
			//	var dataSet = new NewDataSet();
			//	report.DataSource = dataSet;
			//	report.DataMember = dataSet.Tables[0].TableName;
			report.CreateDocument();
			documentViewer1.DocumentSource = report;
		}

		public void SetReport(PassCardTemplateReport currentReport)
		{
			if (currentReport == null) return;

			currentReport.CreateDocument();
			documentViewer1.DocumentSource = currentReport;
			documentViewer1.Update();
		}
	}
}
