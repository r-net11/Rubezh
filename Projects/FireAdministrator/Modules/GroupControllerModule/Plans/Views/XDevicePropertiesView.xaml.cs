using System.Windows.Controls;

namespace GKModule.Plans.Views
{
    public partial class XDevicePropertiesView : UserControl
    {
		public XDevicePropertiesView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
        }
    }
}