using System.Windows;
using SAPBusinessObjects.WPF.Viewer;
using ReportsModule.Reports;

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
			_viewCore.ToggleSidePanel = Constants.SidePanelKind.None;
			_viewCore.ReportSource = baseReport.CreateCrystalReportDocument();
		}

		public void Initialize()
		{
			ShowCrystalReport(new ReportDevicesList());
		}
	}
}