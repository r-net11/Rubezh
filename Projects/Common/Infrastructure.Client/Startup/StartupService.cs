using System;
using FiresecAPI;
using Common;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using Infrastructure.Client.Startup.ViewModels;
using Infrastructure.Common.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.IO;
using System.Threading.Tasks;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client.Startup
{
	public class StartupService
	{
		private const string LogoResource = "Logo.jpg";
		private SplashScreen _splash;
		private AutoResetEvent _syncEvent;
		private StartupViewModel _viewModel;
		private ClientType _clientType;

		public static StartupService Instance { get; private set; }
		public bool IsActive { get; private set; }
		public string Login { get; private set; }
		public string Password { get; private set; }

		public StartupService(ClientType clientType)
		{
			_clientType = clientType;
			Instance = this;
		}

		public void Run()
		{
			IsActive = true;
			//_splash = new SplashScreen(LogoResource);
			//_splash.Show(false);
			MessageBoxService.SetMessageBoxHandler(MessageBoxHandler);
		}
		public void Show()
		{
			_syncEvent = new AutoResetEvent(false);
			var splashThread = new Thread(new ParameterizedThreadStart(InternalThreadEntryPoint));
			splashThread.SetApartmentState(ApartmentState.STA);
			splashThread.IsBackground = true;
			splashThread.Start();
			_syncEvent.WaitOne();
			CloseSplashImage();
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
				//Login = ((StartupLoginViewModel)_viewModel.Content).UserName;
				//Password = ((StartupLoginViewModel)_viewModel.Content).Password;
			}
			else
				Close();
			return result;
		}
		public void ShowLoading(string title, int stepCount = 1)
		{
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

		internal Window OwnerWindow
		{
			get { return _viewModel.Surface; }
		}
		public void Invoke(Action action)
		{
			_viewModel.Dispatcher.Invoke(action);
		}

		private void InternalThreadEntryPoint(object parameter)
		{
			_viewModel = new StartupViewModel(_clientType);
			_viewModel.Closed += new EventHandler(StartupClosed);
			_syncEvent.Set();
			DialogService.ShowModalWindow(_viewModel);
			ReleaseResources();
		}

		private void StartupClosed(object sender, EventArgs e)
		{
			if (IsActive)
			{
				MessageBoxService.ResetMessageBoxHandler();
				ApplicationService.Invoke((Action)(() => { throw new StartupCancellationException(); }));
			}
		}
		private void CloseSplashImage()
		{
			if (_splash != null)
			{
				_splash.Close(TimeSpan.Zero);
				_splash = null;
			}
		}
		private void ReleaseResources()
		{
			CloseSplashImage();
			_syncEvent.Dispose();
			_syncEvent = null;
			Dispatcher.CurrentDispatcher.InvokeShutdown();
		}

		public void Capture()
		{
			Capture(LogoResource, new PngBitmapEncoder());
		}
		public void Capture(string filePath, BitmapEncoder encoder)
		{
			RenderTargetBitmap bmp = new RenderTargetBitmap((int)_viewModel.Surface.ActualWidth, (int)_viewModel.Surface.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			bmp.Render(_viewModel.Surface);
			encoder.Frames.Add(BitmapFrame.Create(bmp));
			using (var stream = File.Create(filePath))
				encoder.Save(stream);
		}

		private void MessageBoxHandler(MessageBoxViewModel viewModel, bool isModal)
		{
			Invoke(() =>
			{
				var messageBoxImage = MessageBoxImage.None;
				if (viewModel.IsInformationImageVisible)
					messageBoxImage = MessageBoxImage.Information;
				else if (viewModel.IsQuestionImageVisible)
					messageBoxImage = MessageBoxImage.Question;
				else if (viewModel.IsWarningImageVisible)
					messageBoxImage = MessageBoxImage.Warning;
				else if (viewModel.IsErrorImageVisible)
					messageBoxImage = MessageBoxImage.Error;
				var messageBoxButton = MessageBoxButton.OK;
				if (viewModel.IsOkButtonVisible)
				{
					if (viewModel.IsCancelButtonVisible)
						messageBoxButton = MessageBoxButton.OKCancel;
				}
				else
					messageBoxButton = viewModel.IsCancelButtonVisible ? MessageBoxButton.YesNoCancel : MessageBoxButton.YesNo;
				var startupMessageBoxViewModel = new StartupMessageBoxViewModel(viewModel.Title, viewModel.Message, messageBoxButton, messageBoxImage, viewModel.IsException);
				if (isModal)
					DialogService.ShowModalWindow(startupMessageBoxViewModel);
				else
					DialogService.ShowWindow(startupMessageBoxViewModel);
				viewModel.Result = startupMessageBoxViewModel.Result;
			});
		}
	}
}
