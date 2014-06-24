using System.Windows.Controls;

namespace GKModule.Plans.Views
{
	public partial class GuardZonePropertiesView : UserControl
	{
		public GuardZonePropertiesView()
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