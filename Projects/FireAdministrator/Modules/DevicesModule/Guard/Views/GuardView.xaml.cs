using System.Windows.Controls;
using DevicesModule.ViewModels;

namespace DevicesModule.Views
{
	public partial class GuardView : UserControl
	{
		public GuardView()
		{
			InitializeComponent();
		}

		private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			var guardViewModel = dataGrid.DataContext as GuardViewModel;
			guardViewModel.EditDeviceUser();
		}

		private void DataGrid_MouseDoubleClick_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			var guardViewModel = dataGrid.DataContext as GuardViewModel;
			guardViewModel.EditUser();
		}
	}
}