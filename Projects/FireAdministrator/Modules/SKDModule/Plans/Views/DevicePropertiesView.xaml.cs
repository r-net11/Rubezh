using System.Windows.Controls;

namespace SKDModule.Plans.Views
{
	public partial class DevicePropertiesView : UserControl
	{
		public DevicePropertiesView()
		{
			InitializeComponent();
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