using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common;

namespace Controls
{
    public class XDataGrid : DataGrid
    {
        static XDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XDataGrid), new FrameworkPropertyMetadata(typeof(XDataGrid)));
        }

        public XDataGrid()
        {
            SelectionChanged += new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(DataGrid_PreviewMouseDown);
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
            if (PreviousDataGridCell != CurrentDataGridCell)
                return;

            var dataGrid = sender as DataGrid;
            var viewModel = dataGrid.DataContext as IEditingViewModel;
            if (viewModel.EditCommand.CanExecute(null))
                viewModel.EditCommand.Execute();
        }

        private void DataGrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PreviousDataGridCell = CurrentDataGridCell;
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement &&
				(((FrameworkElement)element).Parent is DataGridCell || ((FrameworkElement)element).Parent == null))
            {
                CurrentDataGridCell = ((FrameworkElement)element).Parent as DataGridCell;
            }
            else
            {
                CurrentDataGridCell = null;
                var dataGrid = sender as DataGrid;
                dataGrid.SelectedItem = null;
            }
        }

        DataGridCell PreviousDataGridCell;
        DataGridCell CurrentDataGridCell;
    }
}