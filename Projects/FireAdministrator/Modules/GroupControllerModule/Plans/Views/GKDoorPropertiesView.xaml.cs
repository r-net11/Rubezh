using System.Windows.Controls;

namespace GKModule.Plans.Views
{
	/// <summary>
	/// Interaction logic for GKDoorPropertiesView.xaml
	/// </summary>
	public partial class GKDoorPropertiesView : UserControl
	{
		public GKDoorPropertiesView()
		{
			InitializeComponent();
		}
		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null && !dataGrid.IsMouseOver)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}
