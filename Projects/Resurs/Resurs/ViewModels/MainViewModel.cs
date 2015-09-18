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
		Dispatcher _dispatcher;

		public MainViewModel()
		{
			AllowHelp = true;
			AllowMaximize = true;
			AllowMinimize = true;
			ContentFotter = null;
			Title = "АРМ Ресурс";

			_dispatcher = Dispatcher.CurrentDispatcher;
			MessageBoxService.SetMessageBoxHandler(MessageBoxHandler);

			DevicesViewModel = new DevicesViewModel();
			ApartmentsViewModel = new ApartmentsViewModel();
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