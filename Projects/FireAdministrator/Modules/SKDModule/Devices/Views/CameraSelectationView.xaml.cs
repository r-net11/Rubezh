using System.Windows.Controls;

namespace SKDModule.Views
{
	public partial class CameraSelectationView : UserControl
	{
		public CameraSelectationView()
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