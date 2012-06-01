using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Controls.Primitives;

namespace Infrastructure.Common.Windows.Views
{
	public partial class WindowBaseView : Window
	{
		[Flags]
		public enum ResizeDirection
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

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = _model.OnClosing(e.Cancel);
		}
		private void Window_Closed(object sender, System.EventArgs e)
		{
			_model.OnClosed();
		}
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (_model.CloseOnEscape && e.Key == Key.Escape)
				Close();
		}

		private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			Thumb thumb = (Thumb)sender;
			ResizeDirection direction = (ResizeDirection)thumb.Tag;
			if ((direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
			{
				if (ActualHeight + e.VerticalChange >= MinHeight && ActualHeight + e.VerticalChange <= MaxHeight)
					Height += e.VerticalChange;
				else
					thumb.ReleaseMouseCapture();
			}
			if ((direction & ResizeDirection.Top) == ResizeDirection.Top)
			{
				if (ActualHeight - e.VerticalChange >= MinHeight && ActualHeight - e.VerticalChange <= MaxHeight)
				{
					Height -= e.VerticalChange;
					Top += e.VerticalChange;
				}
				else
					thumb.ReleaseMouseCapture();
			}
			if ((direction & ResizeDirection.Left) == ResizeDirection.Left)
			{
				if (ActualWidth - e.HorizontalChange >= MinWidth && ActualWidth - e.HorizontalChange <= MaxWidth)
				{
					Width -= e.HorizontalChange;
					Left += e.HorizontalChange;
				}
				else
					thumb.ReleaseMouseCapture();
			}
			if ((direction & ResizeDirection.Right) == ResizeDirection.Right)
			{
				if (ActualWidth + e.HorizontalChange >= MinWidth && ActualWidth + e.HorizontalChange <= MaxWidth)
					Width += e.HorizontalChange;
				else
					thumb.ReleaseMouseCapture();
			}
		}
	}
}
