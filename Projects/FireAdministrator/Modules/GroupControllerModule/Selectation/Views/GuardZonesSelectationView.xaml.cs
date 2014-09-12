using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class GuardZonesSelectationView : UserControl
	{
		public GuardZonesSelectationView()
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