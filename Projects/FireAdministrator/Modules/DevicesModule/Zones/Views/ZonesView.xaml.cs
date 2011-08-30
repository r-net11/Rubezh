using System.Windows.Controls;
using System.Windows;

namespace DevicesModule.Views
{
    public partial class ZonesView : UserControl
    {
        public ZonesView()
        {
            InitializeComponent();
        }

        private void _dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataGrid.SelectedItem != null)
            {
                _dataGrid.ScrollIntoView(_dataGrid.SelectedItem);
            }
        }
    }
}