using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevicesModule.ViewModels;

namespace DevicesModule.Views
{
    public partial class DevicesView : UserControl
    {
        public DevicesView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DevicesView_Loaded);
            _devicesDataGrid.SelectionChanged += new SelectionChangedEventHandler(DevicesView_SelectionChanged);
            _devicesDataGrid.PreviewKeyDown += new KeyEventHandler(_devicesDataGrid_PreviewKeyDown);
        }

        void DevicesView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_devicesDataGrid.SelectedItem != null)
                _devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
            Focus();
        }

        void DevicesView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_devicesDataGrid.SelectedItem != null)
                _devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
        }

        private void _devicesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                var dataGrid = sender as DataGrid;
                if (dataGrid != null)
                    dataGrid.BeginEdit();
            }
        }
    }
}