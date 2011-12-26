using System.Windows;
using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class DevicesView : UserControl
    {
        public DevicesView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_devicesDataGrid.SelectedItem != null)
                _devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
        }
    }
}