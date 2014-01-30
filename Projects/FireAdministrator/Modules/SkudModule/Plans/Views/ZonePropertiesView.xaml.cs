using System.Windows.Controls;

namespace SKDModule.Plans.Views
{
	public partial class ZonePropertiesView : UserControl
	{
		public ZonePropertiesView()
		{
			InitializeComponent();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}