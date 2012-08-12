using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ReportsModule.Views;

namespace ReportsModule.ViewModels
{
	public class ReportsViewModel : ViewPartViewModel
	{
		public ReportsViewModel()
		{
			ShowReportCommand = new RelayCommand(OnShowReport);
		}

		public RelayCommand ShowReportCommand { get; private set; }
		void OnShowReport()
		{
			var _reportControlView = new ReportControlView();
			_reportControlView.Show();
		}
	}
}