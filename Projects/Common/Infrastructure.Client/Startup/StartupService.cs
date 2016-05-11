using Infrastructure.Client.Startup.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Infrastructure.Client.Startup
{
	public class StartupService
	{
		AutoResetEvent _syncEvent;
		StartupViewModel _viewModel;
		ClientType _clientType;

		public static StartupService Instance { get; private set; }
		public bool IsActive { get; private set; }
		public string Login { get; private set; }
		public string Password { get; private set; }

		public event Action OnCloseEvent;

		public StartupService(ClientType clientType)
		{
			_clientType = clientType;
			Instance = this;
		}

		public void Run()
		{
			IsActive = true;
		}
		public void Show()
		{
			_syncEvent = new AutoResetEvent(false);
			var splashThread = new Thread(new ParameterizedThreadStart(InternalThreadEntryPoint));
			splashThread.SetApartmentState(ApartmentState.STA);
			splashThread.IsBackground = true;
			splashThread.Name = "Startup Service Thread";
			splashThread.Start();
			_syncEvent.WaitOne();
		}
		public void Close()
		{
			if (IsActive)
			{
				IsActive = false;
				MessageBoxService.ResetMessageBoxHandler();
				_viewModel.Dispatcher.BeginInvoke((Action)_viewModel.Close);
			}
		}
		public bool PerformLogin()
		{
			return PerformLogin(null, null);
		}
		public bool PerformLogin(string login, string password)
		{
			ApplicationService.DoEvents();
			var result = _viewModel.PerformLogin(login, password);
			if (result)
			{
				Login = ((StartupLoginViewModel)_viewModel.Content).UserName;
				Password = ((StartupLoginViewModel)_viewModel.Content).Password;
			}
			else
				Close();
			return result;
		}
		public void ShowLoading(string title, int stepCount = 1)
		{
			ApplicationService.DoEvents();
			_viewModel.ShowLoading(title, stepCount);
			ApplicationService.DoEvents();
		}
		public void DoStep(string text)
		{
			_viewModel.DoStep(text);
			ApplicationService.DoEvents();
		}
		public void AddCount(int count)
		{
			_viewModel.AddCount(count);
			ApplicationService.DoEvents();
		}

		public Window OwnerWindow
		{
			get { return _viewModel.Surface; }
		}
		public void Invoke(Action action)
		{
			_viewModel.Dispatcher.Invoke(action);
		}

		void InternalThreadEntryPoint(object parameter)
		{
			_viewModel = new StartupViewModel(_clientType);
			_viewModel.Closed += new EventHandler(StartupClosed);
			MessageBoxService.SetMessageBoxHandler(MessageBoxHandler);
			_syncEvent.Set();
			DialogService.ShowModalWindow(_viewModel);
			ReleaseResources();
		}

		void StartupClosed(object sender, EventArgs e)
		{
			if (_viewModel != null && _viewModel.StartupSettingsWaitHandler != null)
			{
				_viewModel.StartupSettingsWaitHandler.Set();
			}

			if (IsActive)
			{
				MessageBoxService.ResetMessageBoxHandler();
				OnCloseEvent();
			}
		}

		void ReleaseResources()
		{
			_syncEvent.Dispose();
			_syncEvent = null;
			Dispatcher.CurrentDispatcher.InvokeShutdown();
		}

		void MessageBoxHandler(MessageBoxViewModel viewModel, bool isModal)
		{
			Invoke(() =>
			{
				var startupMessageBoxViewModel = new StartupMessageBoxViewModel(viewModel.Title, viewModel.Message, viewModel.MessageBoxButton, viewModel.MessageBoxImage, viewModel.IsException);
				if (isModal)
					DialogService.ShowModalWindow(startupMessageBoxViewModel);
				else
					DialogService.ShowWindow(startupMessageBoxViewModel);
				viewModel.Result = startupMessageBoxViewModel.Result;
			});
		}
	}
}