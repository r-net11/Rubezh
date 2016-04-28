using System.Windows;
using System.Windows.Controls;

namespace Infrastructure.Common.Windows.Views
{
	public partial class HeaderedWindowView : UserControl
	{
		public Window Window { get; private set; }

		public HeaderedWindowView()
		{
			InitializeComponent();
		}

		private void HeaderedView_Loaded(object sender, RoutedEventArgs e)
		{
			Window = Window.GetWindow(this);
		}
	}
}