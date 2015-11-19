using System.Windows.Controls;
using System.Windows.Input;

namespace LayoutModule.Views
{
	public partial class MonitorLayoutsView : UserControl
	{
		public MonitorLayoutsView()
		{
			InitializeComponent();
		}

		private void Grid_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			var griddy = (Grid)sender;
			griddy.Focusable = true;
			Keyboard.Focus(griddy);
		}
	}
}