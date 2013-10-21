using DiagnosticsModule.Views;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			ShowDriversCommand = new RelayCommand(OnShowDrivers);
			ShowXDriversCommand = new RelayCommand(OnShowXDrivers);
			Test1Command = new RelayCommand(OnTest1);
		}

		public void StopThreads()
		{
			IsThreadStoping = true;
		}

		bool IsThreadStoping = false;

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public RelayCommand ShowDriversCommand { get; private set; }
		private void OnShowDrivers()
		{
			var driversView = new DriversView();
			driversView.ShowDialog();
		}

		public RelayCommand ShowXDriversCommand { get; private set; }
		private void OnShowXDrivers()
		{
			var driversView = new XDriversView();
			driversView.ShowDialog();
		}

		int counter = 0;

		public RelayCommand Test1Command { get; private set; }
		private void OnTest1()
		{
			var vm = new PerformanceViewModel();
			DialogService.ShowModalWindow(vm);
		}
	}
}