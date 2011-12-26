using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class ZonesView : UserControl
    {
        public ZonesView()
        {
            InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(OnLoaded);
        }

        void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_zonesListBox.SelectedItem != null)
                _zonesListBox.ScrollIntoView(_zonesListBox.SelectedItem);
        }
    }
}