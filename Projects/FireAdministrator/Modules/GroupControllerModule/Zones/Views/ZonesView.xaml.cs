using System.Windows.Controls;

namespace GroupControllerModule.Views
{
    public partial class ZonesView : UserControl
    {
        public ZonesView()
        {
            InitializeComponent();
            if (width != 0)
                leftColumn.Width = new System.Windows.GridLength(width);
        }

        static double width = 0;

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            width = leftColumn.Width.Value;
        }
    }
}