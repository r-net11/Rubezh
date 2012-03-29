using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class ZonesView : UserControl
    {
        public ZonesView()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(ZonesView_Loaded);
            if (width != 0)
                leftColumn.Width = new System.Windows.GridLength(width);
        }

        void ZonesView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _zones.Focus();
        }

        static double width = 0;

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            width = leftColumn.Width.Value;
        }
    }
}