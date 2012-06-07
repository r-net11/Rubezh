using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Infrastructure.Common.Windows.Views
{
	public partial class HeaderedWindowView : UserControl
	{
		public Window Window { get; private set; }

		public HeaderedWindowView()
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
	}
}