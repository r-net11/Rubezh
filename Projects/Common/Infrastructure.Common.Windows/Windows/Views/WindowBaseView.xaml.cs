using Common;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrastructure.Common.Windows.Views
{
	internal partial class WindowBaseView : Window
	{
		private const int AbsolutMinSize = 100;
		private WindowBaseViewModel _model;

		public WindowBaseView()
			: this(null)
		{
		}
		public WindowBaseView(WindowBaseViewModel model)
		{
			if (model != null)
			{
				_model = model;
				_model.SetSurface(this);
				DataContext = _model;
			}
			InitializeComponent();

			Loaded += (s, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			if (WindowState == WindowState.Minimized)
			{
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
			}
			CalculateSize();
			UpdateWindowSize();
			var shellViewModel = _model as ShellViewModel;
			if (shellViewModel != null)
			{
				MinHeight = shellViewModel.MinHeight;
				MinWidth = shellViewModel.MinWidth;
				Height = shellViewModel.Height;
				Width = shellViewModel.Width;
			}
			if (_model.HideInTaskbar)
				ShowInTaskbar = false;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			_model.InternalClosing(e);
		}
		private void Window_Closed(object sender, System.EventArgs e)
		{
			_model.InternalClosed();
		}
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (_model.CloseOnEscape && e.Key == Key.Escape)
				Close();
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_model.Loaded();
		}
		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			_model.Unloaded();
		}

		private void CalculateSize()
		{
			ContentPresenter presenter = FindPresenter(this);
			if (presenter != null && VisualTreeHelper.GetChildrenCount(presenter) == 1)
			{
				var control = VisualTreeHelper.GetChild(presenter, 0) as FrameworkElement;
				if (control != null)
				{
					var oldHeight = ActualHeight;
					var oldWidth = ActualWidth;
					var borderHeight = oldHeight - presenter.ActualHeight;
					var borderWidth = oldWidth - presenter.ActualWidth;

					MinHeight = control.MinHeight + borderHeight;
					MinWidth = control.MinWidth + borderWidth;
					MaxHeight = control.MaxHeight + borderHeight;
					MaxWidth = control.MaxWidth + borderWidth;

					Height = double.IsNaN(control.Height) ? MinHeight : control.Height + borderHeight;
					Width = double.IsNaN(control.Width) ? MinWidth : control.Width + borderWidth;

					if (!double.IsNaN(control.Height))
						control.Height = double.NaN;
					if (!double.IsNaN(control.Width))
						control.Width = double.NaN;

					if (WindowStartupLocation != WindowStartupLocation.Manual)
					{
						Left += (oldWidth - ActualWidth) / 2;
						Top += (oldHeight - ActualHeight) / 2;
					}
				}
			}
		}
		private ContentPresenter FindPresenter(DependencyObject obj)
		{
			ContentPresenter result = null;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject childObj = VisualTreeHelper.GetChild(obj, i);
				if (childObj != null)
				{
					var presenter = childObj as ContentPresenter;
					if (presenter != null && presenter.Content == DataContext)
						result = presenter;
					result = FindPresenter(childObj) ?? result;
				}
			}
			return result;
		}

		private void UpdateWindowSize()
		{
			try
			{
				var isSaveSize = _model.GetType().GetCustomAttributes(typeof(SaveSizeAttribute), true).Length > 0;
				if (isSaveSize)
				{
					string key = "WindowRect." + _model.GetType().AssemblyQualifiedName;
					var windowRect = RegistrySettingsHelper.GetWindowRect(key);
					if (windowRect != null)
					{
						Top = windowRect.Top;
						Left = windowRect.Left;
						Width = windowRect.Width;
						Height = windowRect.Height;
						WindowStartupLocation = WindowStartupLocation.Manual;
					}
					Closed += SaveWindowSize;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "WindowBaseView.UpdateWindowSize");
			}
		}
		private void SaveWindowSize(object sender, EventArgs e)
		{
			try
			{
				string key = "WindowRect." + _model.GetType().AssemblyQualifiedName;
				var windowRect = new WindowRect()
				{
					Left = Left,
					Top = Top,
					Width = Width,
					Height = Height
				};
				RegistrySettingsHelper.SetWindowRect(key, windowRect);
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "WindowBaseView.SaveWindowSize");
			}
		}
	}
}