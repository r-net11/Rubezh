using System.Windows.Controls;
using DevicesModule.ViewModels;

namespace DevicesModule.Views
{
    public partial class LevelsView : UserControl
    {
        public LevelsView()
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
            var levelsViewModel = dataGrid.DataContext as LevelsViewModel;
            if (levelsViewModel.EditCommand.CanExecute(null))
                levelsViewModel.EditCommand.Execute();
        }
    }
}
