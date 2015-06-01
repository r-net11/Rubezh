using System.Windows;
using System.Windows.Controls;

namespace PowerCalculator.Views
{
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
			DevicesDataGrid.SelectionChanged += new SelectionChangedEventHandler(DataGrid_SelectionChanged);
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}