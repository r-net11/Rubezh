using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace PlansModule.Designer
{
    public class ResizeAdornerPolygon : Adorner
    {
        private VisualCollection visuals;
        private ResizeChromePolygon chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public ResizeAdornerPolygon(DesignerItem designerItem)
            : base(designerItem)
        {
            this.chrome = new ResizeChromePolygon(designerItem);
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
            this.chrome.DataContext = designerItem;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    }
}
