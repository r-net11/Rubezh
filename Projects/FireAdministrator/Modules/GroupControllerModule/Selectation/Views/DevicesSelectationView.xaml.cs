using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class DevicesSelectationView : UserControl
	{
		public DevicesSelectationView()
		{
			InitializeComponent();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}