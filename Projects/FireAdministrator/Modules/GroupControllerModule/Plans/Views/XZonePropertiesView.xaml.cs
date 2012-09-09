using System.Windows.Controls;

namespace GKModule.Plans.Views
{
	public partial class XZonePropertiesView : UserControl
	{
		public XZonePropertiesView()
		{
			InitializeComponent();
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid.SelectedItem != null)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}