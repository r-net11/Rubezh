using System.Windows.Controls;

namespace GroupControllerModule.Views
{
    public partial class ZoneDevicesView : UserControl
    {
        public ZoneDevicesView()
        {
            InitializeComponent();
            InitializeComponent();
            if (height != 0)
                bottomRow.Height = new System.Windows.GridLength(height);
        }

        static double height = 0;

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            height = bottomRow.Height.Value;
        }
    }
}