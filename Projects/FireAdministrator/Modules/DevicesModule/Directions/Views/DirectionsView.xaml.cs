using System.Windows;
using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class DirectionsView : UserControl
    {
        public DirectionsView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DevicesView_Loaded);
            _directionDataGrid.SelectionChanged += new SelectionChangedEventHandler(DevicesView_Loaded);
        }

        void DevicesView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_directionDataGrid.SelectedItem != null)
                _directionDataGrid.ScrollIntoView(_directionDataGrid.SelectedItem);
        }
    }
}