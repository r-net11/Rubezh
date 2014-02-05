using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout.ViewModels
{
	public class ContentPartViewModel : BaseViewModel
	{
		private MonitorLayoutShellViewModel _monitorLayoutShellViewModel;
		public ContentPartViewModel(MonitorLayoutShellViewModel monitorLayoutShellViewModel)
		{
			_monitorLayoutShellViewModel = monitorLayoutShellViewModel;
		}

		public ObservableCollection<IViewPartViewModel> ContentItems
		{
			get { return _monitorLayoutShellViewModel.ContentItems; }
		}
	}
}
