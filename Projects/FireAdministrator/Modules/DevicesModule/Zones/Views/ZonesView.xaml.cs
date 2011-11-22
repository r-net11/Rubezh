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
            if (width != 0)
                leftColumn.Width = new System.Windows.GridLength(width);
        }

        static double width = 0;

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            width = leftColumn.Width.Value;
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