using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.Views
{
    public partial class NewDeviceView : UserControl
    {
        public NewDeviceView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if ((element != null && element is FrameworkElement && ((FrameworkElement)element).Parent is DataGridCell) == true)
            {
                var dataGrid = sender as DataGrid;
                var saveCancelDialogContent = dataGrid.DataContext as SaveCancelDialogViewModel;
                if (saveCancelDialogContent.SaveCommand.CanExecute(null))
                    saveCancelDialogContent.SaveCommand.Execute();
            }
        }
    }
}