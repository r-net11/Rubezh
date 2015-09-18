using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace Resurs.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }
		Dispatcher _dispatcher;

		public MainViewModel()
		{
			Current = this;
			_dispatcher = Dispatcher.CurrentDispatcher;
			MessageBoxService.SetMessageBoxHandler(MessageBoxHandler);
			SetTitle();
			DevicesViewModel = new DevicesViewModel();
			ApartmentsViewModel = new ApartmentsViewModel();
		}

		void SetTitle()
		{
			Title = "АРМ Ресурс";
		}

		void MessageBoxHandler(MessageBoxViewModel viewModel, bool isModal)
		{
			_dispatcher.Invoke((Action)(() =>
			{
				var startupMessageBoxViewModel = new ResursMessageBoxViewModel(viewModel.Title, viewModel.Message, viewModel.MessageBoxButton, viewModel.MessageBoxImage, viewModel.IsException);
				if (isModal)
					DialogService.ShowModalWindow(startupMessageBoxViewModel);
				else
					DialogService.ShowWindow(startupMessageBoxViewModel);
				viewModel.Result = startupMessageBoxViewModel.Result;
			}));
		}
		

		public override int GetPreferedMonitor()
		{
			return MonitorHelper.PrimaryMonitor;
		}

		public DevicesViewModel DevicesViewModel { get; private set; }
		public ApartmentsViewModel ApartmentsViewModel { get; private set; }

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}
    }
}