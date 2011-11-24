using System.Windows.Controls;
using System.Windows.Input;
using Infrastructure;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public partial class ElementDeviceView : UserControl
    {
        public ElementDeviceView()
        {
            InitializeComponent();
        }

        void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            _mouseOverRectangle.StrokeThickness = 1;
        }

        void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _mouseOverRectangle.StrokeThickness = 0;
        }

        private void _deviceControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ServiceFactory.Events.GetEvent<ElementDeviceSelectedEvent>().Publish(null);
            _selectationRectangle.StrokeThickness = 1;
        }
    }
}