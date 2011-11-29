using System.Windows.Controls;
using System.Diagnostics;

namespace PlansModule.Views
{
    public partial class PlansView : UserControl
    {
        public PlansView()
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
