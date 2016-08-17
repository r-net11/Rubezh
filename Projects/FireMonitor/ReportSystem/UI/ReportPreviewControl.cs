using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using ReportSystem.Reports;

namespace ReportSystem.UI
{
	public partial class ReportPreviewControl : UserControl
	{
		public ReportPreviewControl()
		{
			InitializeComponent();
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
