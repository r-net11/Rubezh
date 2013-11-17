using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

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
