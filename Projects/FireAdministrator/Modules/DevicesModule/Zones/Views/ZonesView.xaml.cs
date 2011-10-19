using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class ZonesView : UserControl
    {
        public ZonesView()
        {
            InitializeComponent();
        }

        private void _dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
        }
    }
}