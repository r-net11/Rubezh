using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace Resurs.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			DevicesViewModel = new DevicesViewModel();
			ApartmentsViewModel = new ApartmentsViewModel();
			ReportsViewModel = new ReportsViewModel();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }
		public ApartmentsViewModel ApartmentsViewModel { get; private set; }
		public ReportsViewModel ReportsViewModel { get; private set; }
    }
}