using System.Windows.Controls;
using System.Windows;
using SecurityModule.ViewModels;

namespace SecurityModule.Views
{
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var zonesViewModel = dataGrid.DataContext as UsersViewModel;
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