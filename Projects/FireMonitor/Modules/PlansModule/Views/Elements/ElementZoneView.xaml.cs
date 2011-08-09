using System.Windows.Controls;
using System.Windows.Input;

namespace PlansModule
{
    public partial class ElementZoneView : UserControl
    {
        public ElementZoneView()
        {
            InitializeComponent();
        }

        private void _polygon_MouseEnter(object sender, MouseEventArgs e)
        {
            _polygon.StrokeThickness = 1;
        }

        private void _polygon_MouseLeave(object sender, MouseEventArgs e)
        {
            _polygon.StrokeThickness = 0;
        }
    }
}