using System.Windows;
using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class DevicesView : UserControl
    {
        public DevicesView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DevicesView_Loaded);
            _devicesDataGrid.SelectionChanged += new SelectionChangedEventHandler(DevicesView_Loaded);
        }

        void DevicesView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_devicesDataGrid.SelectedItem != null)
                _devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
        }
    }
}