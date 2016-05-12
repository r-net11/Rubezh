using System.Windows.Controls;

namespace AutomationModule.Plans.Views
{
	public partial class ProcedurePropertiesView : UserControl
	{
		public ProcedurePropertiesView()
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