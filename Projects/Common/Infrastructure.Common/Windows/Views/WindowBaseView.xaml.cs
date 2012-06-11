using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Views
{
	internal partial class WindowBaseView : Window
	{
		private const int AbsolutMinSize = 100;

		[Flags]
		internal enum ResizeDirection
		{
			Left = 1,
			Bottom = 2,
			Top = 4,
			Right = 8,
			TopRight = Top | Right,
			TopLeft = Top | Left,
			BottomRight = Bottom | Right,
			BottomLeft = Bottom | Left,
		}
		private WindowBaseViewModel _model;

		public WindowBaseView()
		{
			InitializeComponent();
		}
		public WindowBaseView(WindowBaseViewModel model)
		{
			_model = model;
			_model.Surface = this;
			DataContext = _model;
			InitializeComponent();
		}
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			CalculateSize();
			if (MinHeight < AbsolutMinSize)
				MinHeight = AbsolutMinSize;
			if (MinWidth < AbsolutMinSize)
				MinWidth = AbsolutMinSize;
			if (MaxHeight > SystemParameters.MaximizedPrimaryScreenHeight)
				MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
			if (MaxWidth > SystemParameters.MaximizedPrimaryScreenWidth)
				MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
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

		private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			Thumb thumb = (Thumb)sender;
			ResizeDirection direction = (ResizeDirection)thumb.Tag;
			if ((direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
			{
				if (Height + e.VerticalChange >= MinHeight && Height + e.VerticalChange <= MaxHeight)
					Height += e.VerticalChange;
				//else
				//    thumb.ReleaseMouseCapture();
			}
			if ((direction & ResizeDirection.Top) == ResizeDirection.Top)
			{
				if (Height - e.VerticalChange >= MinHeight && Height - e.VerticalChange <= MaxHeight)
				{
					Height -= e.VerticalChange;
					Top += e.VerticalChange;
				}
				//else
				//    thumb.ReleaseMouseCapture();
			}
			if ((direction & ResizeDirection.Left) == ResizeDirection.Left)
			{
				if (Width - e.HorizontalChange >= MinWidth && Width - e.HorizontalChange <= MaxWidth)
				{
					Width -= e.HorizontalChange;
					Left += e.HorizontalChange;
				}
				//else
				//    thumb.ReleaseMouseCapture();
			}
			if ((direction & ResizeDirection.Right) == ResizeDirection.Right)
			{
				if (Width + e.HorizontalChange >= MinWidth && Width + e.HorizontalChange <= MaxWidth)
					Width += e.HorizontalChange;
				//else
				//    thumb.ReleaseMouseCapture();
			}
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

					Left += (oldWidth - ActualWidth) / 2;
					Top += (oldHeight - ActualHeight) / 2;
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
	}
}