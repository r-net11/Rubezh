using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Navigation;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout.ViewModels
{
	public class NavigationPartViewModel : BaseViewModel
	{
		private MonitorLayoutShellViewModel _monitorLayoutShellViewModel;
		public NavigationPartViewModel(MonitorLayoutShellViewModel monitorLayoutShellViewModel)
		{
			_monitorLayoutShellViewModel = monitorLayoutShellViewModel;
		}

		public List<NavigationItem> NavigationItems
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
