using System.Windows.Controls;

namespace PlansModule.Views
{
    public partial class DevicePropertiesView : UserControl
    {
        public DevicePropertiesView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
        }
    }
}