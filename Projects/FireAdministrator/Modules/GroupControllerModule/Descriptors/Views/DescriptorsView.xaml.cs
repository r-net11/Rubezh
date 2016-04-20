using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class DescriptorsView : UserControl
	{
		public DescriptorsView()
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