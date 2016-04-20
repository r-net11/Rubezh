using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			TestCommand = new RelayCommand(OnTest);
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
		}
	}
}