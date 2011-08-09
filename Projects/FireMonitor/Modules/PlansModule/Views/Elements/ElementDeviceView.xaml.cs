using System.Windows.Controls;
using System.Windows.Input;

namespace PlansModule.ViewModels
{
    public partial class ElementDeviceView : UserControl
    {
        public ElementDeviceView()
        {
            InitializeComponent();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            _mouseOverRectangle.StrokeThickness = 1;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _mouseOverRectangle.StrokeThickness = 0;
        }
    }
}