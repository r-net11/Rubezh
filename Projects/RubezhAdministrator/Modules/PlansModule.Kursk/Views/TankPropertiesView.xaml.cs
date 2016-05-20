using System.Windows.Controls;

namespace PlansModule.Kursk.Views
{
	public partial class TankPropertiesView : UserControl
	{
		public TankPropertiesView()
		{
			InitializeComponent();
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
		}
	}
}