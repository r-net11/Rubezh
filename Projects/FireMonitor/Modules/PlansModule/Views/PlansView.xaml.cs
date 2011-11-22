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
                rightColumn.Width = new System.Windows.GridLength(width);
        }

        static double width = 0;

        private void Border_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            width = rightColumn.Width.Value;
        }
    }
}