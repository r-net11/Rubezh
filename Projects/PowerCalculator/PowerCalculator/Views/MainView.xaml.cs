using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PowerCalculator.ViewModels;

namespace PowerCalculator.Views
{
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
			DevicesDataGrid.SelectionChanged += new SelectionChangedEventHandler(DataGrid_SelectionChanged);
			KeyUp += new System.Windows.Input.KeyEventHandler(MainView_KeyUp);
		}

		void MainView_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.C && Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				(DataContext as MainViewModel).SelectedLine.CopyCommand.Execute(DevicesDataGrid.SelectedItems);
			}
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