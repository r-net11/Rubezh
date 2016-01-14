using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace FireMonitor.Layout.ViewModels
{
	public class ContentPartViewModel : BaseViewModel
	{
		private ShellViewModel _shellViewModel;
		public ContentPartViewModel(ShellViewModel shellViewModel)
		{
			_shellViewModel = shellViewModel;
		}

		public ObservableCollection<IViewPartViewModel> ContentItems
		{
			get { return _shellViewModel.ContentItems; }
		}
	}
}