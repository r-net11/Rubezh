using System.Windows;
using CrystalDecisions.CrystalReports.Engine;
using ReportsModule.Reports;
using SAPBusinessObjects.WPF.Viewer;

namespace ReportsModule.Views
{
	public partial class ReportsWindow : Window
	{
		public ReportsWindow()
		{
			InitializeComponent();
			DataContext = this;
			Closing += new System.ComponentModel.CancelEventHandler(ReportsWindow_Closing);
		}

		void ReportsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		void ShowCrystalReport(BaseReport baseReport)
		{
			baseReport.LoadData();
			ReportDocument reportDocument = new ReportDocument();
			baseReport.LoadCrystalReportDocument(reportDocument);
			_viewCore.ToggleSidePanel = Constants.SidePanelKind.None;
			_viewCore.ReportSource = reportDocument;
		}

		public void Initialize()
		{
			ShowCrystalReport(new ReportDevicesList());
		}
	}
}