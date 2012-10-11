using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class ZonesSelectionView : UserControl
    {
        public ZonesSelectionView()
        {
            InitializeComponent();
            _zones1.SelectionChanged += new SelectionChangedEventHandler(_zonesListBox_SelectionChanged);
            _zones2.SelectionChanged += new SelectionChangedEventHandler(_zonesListBox_SelectionChanged);
        }

        void _zonesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_zones1.SelectedItem != null)
                _zones1.ScrollIntoView(_zones1.SelectedItems[_zones1.SelectedItems.Count - 1]);
            if (_zones2.SelectedItem != null)
                _zones2.ScrollIntoView(_zones2.SelectedItems[_zones2.SelectedItems.Count - 1]);
        }
    }
}