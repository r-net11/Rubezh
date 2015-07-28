using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class DeviceSelectationView
	{
		public DeviceSelectationView()
		{
			InitializeComponent();
		}

		void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}