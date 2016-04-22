using System.Windows;
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

		void Grid_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			var element = (UIElement)sender;
			element.Focusable = true;
			Keyboard.Focus(element);
		}

		private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null && !dataGrid.IsMouseOver)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}