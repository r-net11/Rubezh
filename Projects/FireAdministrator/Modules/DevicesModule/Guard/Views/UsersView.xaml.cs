using System.Windows.Controls;
using DevicesModule.ViewModels;

namespace DevicesModule.Views
{
    public partial class UsersView : UserControl
    {
        public UsersView()
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
            var usersViewModel = dataGrid.DataContext as UsersViewModel;
            if (usersViewModel.EditCommand.CanExecute(null))
                usersViewModel.EditCommand.Execute();
        }
    }
}
