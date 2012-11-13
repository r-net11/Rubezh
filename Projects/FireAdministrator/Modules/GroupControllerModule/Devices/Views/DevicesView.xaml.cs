using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class DevicesView : UserControl
	{
		public DevicesView()
		{
			InitializeComponent();
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}