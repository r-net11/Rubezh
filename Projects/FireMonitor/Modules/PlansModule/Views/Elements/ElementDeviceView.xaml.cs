using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
