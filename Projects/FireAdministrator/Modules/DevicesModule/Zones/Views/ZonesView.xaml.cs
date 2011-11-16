using System.Windows.Controls;
using Microsoft.Windows.Controls;
using DevicesModule.ViewModels;

namespace DevicesModule.Views
{
    public partial class ZonesView : UserControl
    {
        public ZonesView()
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

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var zonesViewModel = dataGrid.DataContext as ZonesViewModel;
            if (zonesViewModel.EditCommand.CanExecute(null))
                zonesViewModel.EditCommand.Execute();
        }
    }
}