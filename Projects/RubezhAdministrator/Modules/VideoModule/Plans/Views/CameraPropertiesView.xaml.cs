using System.Windows.Controls;

namespace VideoModule.Plans.Views
{
	public partial class CameraPropertiesView : UserControl
	{
		public CameraPropertiesView()
		{
			InitializeComponent();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
		}
	}
}