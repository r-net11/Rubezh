using System.Windows.Controls;
using GKModule.ViewModels;

namespace GKModule.Views
{
	public partial class InstructionsDevicesView : UserControl
	{
		public InstructionsDevicesView()
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