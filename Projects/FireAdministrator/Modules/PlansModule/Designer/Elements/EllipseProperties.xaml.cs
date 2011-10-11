using System.Windows.Controls;

namespace DiagramDesigner.Elemrnts
{
    public partial class EllipseProperties : UserControl
    {
        public EllipseProperties()
        {
            InitializeComponent();
            DataContext = this;
        }

        public double BorderWidht { get; set; }
    }
}
