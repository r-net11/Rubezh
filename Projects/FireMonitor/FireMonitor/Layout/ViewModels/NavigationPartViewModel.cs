using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FireMonitor.Layout.ViewModels
{
	public class NavigationPartViewModel : BaseViewModel
	{
		private ShellViewModel _shellViewModel;
		public NavigationPartViewModel(ShellViewModel monitorLayoutShellViewModel)
		{
			var properties = new List<string>()
			{
				"NavigationItems",
				"MinimizeCommand",
				"TextVisibility",
			};
			_shellViewModel = monitorLayoutShellViewModel;
			_shellViewModel.PropertyChanged += (s, e) =>
			{
				if (properties.Contains(e.PropertyName))
					OnPropertyChanged(e.PropertyName);
			};
			OnPropertyChanged(() => NavigationItems);
		}

		public ReadOnlyCollection<NavigationItem> NavigationItems
		{
			get { return _shellViewModel.NavigationItems; }
		}
		public RelayCommand<MinimizeTarget> MinimizeCommand
		{
			get { return _shellViewModel.MinimizeCommand; }
		}
		public bool TextVisibility
		{
			get { return _shellViewModel.TextVisibility; }
		}
	}
}