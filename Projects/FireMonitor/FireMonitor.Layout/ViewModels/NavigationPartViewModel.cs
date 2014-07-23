using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout.ViewModels
{
	public class NavigationPartViewModel : BaseViewModel
	{
		private MonitorLayoutShellViewModel _monitorLayoutShellViewModel;
		public NavigationPartViewModel(MonitorLayoutShellViewModel monitorLayoutShellViewModel)
		{
			var properties = new List<string>()
			{
				"NavigationItems",
				"MinimizeCommand",
				"TextVisibility",
			};
			_monitorLayoutShellViewModel = monitorLayoutShellViewModel;
			_monitorLayoutShellViewModel.PropertyChanged += (s, e) =>
			{
				if (properties.Contains(e.PropertyName))
					OnPropertyChanged(e.PropertyName);
			};
		}

		public ReadOnlyCollection<NavigationItem> NavigationItems
		{
			get { return _monitorLayoutShellViewModel.NavigationItems; }
		}
		public RelayCommand<MinimizeTarget> MinimizeCommand
		{
			get { return _monitorLayoutShellViewModel.MinimizeCommand; }
		}
		public bool TextVisibility
		{
			get { return _monitorLayoutShellViewModel.TextVisibility; }
		}
	}
}