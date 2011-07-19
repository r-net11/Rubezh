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
using System.Windows.Shapes;

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
