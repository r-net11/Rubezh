using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Common;
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
			: this(null)
		{
		}
		public WindowBaseView(WindowBaseViewModel model)
		{
			
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
			
		}

		private void TruncateSize()
		{
			
		}
		private void CalculateSize()
		{
			
		}
		private ContentPresenter FindPresenter(DependencyObject obj)
		{
			ContentPresenter result = null;
			return result;
		}

		private void UpdateWindowSize()
		{
			
		}
		private void SaveWindowSize(object sender, EventArgs e)
		{
			
		}
	}
}