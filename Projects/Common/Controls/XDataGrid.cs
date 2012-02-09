using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common;

namespace Controls
{
    public class XDataGrid : ScrollDataGrid
    {
        static XDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XDataGrid), new FrameworkPropertyMetadata(typeof(XDataGrid)));
        }

        public XDataGrid()
        {
            SelectionChanged +=new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            PreviewMouseDown +=new System.Windows.Input.MouseButtonEventHandler(DataGrid_PreviewMouseDown);
            MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(DataGrid_MouseDoubleClick);
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
            var zonesViewModel = dataGrid.DataContext as IEditingViewModel;
            if (zonesViewModel.EditCommand.CanExecute(null))
                zonesViewModel.EditCommand.Execute();
        }

        private void DataGrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if ((element != null && element is FrameworkElement && ((FrameworkElement)element).Parent is DataGridCell) == false)
            {
                var dataGrid = sender as DataGrid;
                dataGrid.SelectedItem = null;
            }
        }
    }
}
