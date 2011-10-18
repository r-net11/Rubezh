using System.Windows;

namespace PlansModule.Designer
{
    public class DesignerItemData
    {
        public string ItemType { get; set; }
        public FrameworkElement FrameworkElement { get; set; }
        public bool IsPolygon { get; set; }
        public double MinWidth { get; set; }
        public double MinHeight { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
