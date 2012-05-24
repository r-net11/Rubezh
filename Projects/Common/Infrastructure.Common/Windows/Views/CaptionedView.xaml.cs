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
using System.Windows.Controls.Primitives;

namespace Infrastructure.Common.Windows.Views
{
	public partial class CaptionedView : UserControl
	{
		public Window Window { get; private set; }

		public CaptionedView()
		{
			InitializeComponent();
		}
		
		private void CaptionedView_Loaded(object sender, RoutedEventArgs e)
		{
			Window = Window.GetWindow(this);
		}

		private void Header_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
				Window.WindowState = Window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
		}
		private void Header_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
				Window.DragMove();
		}

		private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Window.ActualWidth + e.HorizontalChange > 10)
				Window.Width = Window.ActualWidth + e.HorizontalChange;
			if (Window.ActualHeight + e.VerticalChange > 10)
				Window.Height = Window.ActualHeight + e.VerticalChange;
		}
	}
}
