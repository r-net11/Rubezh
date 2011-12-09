using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace PlansModule.Designer
{
    public class PolygonResizeAdorner : Adorner
    {
        private VisualCollection visuals;
        private PolygonResizeChrome chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public PolygonResizeAdorner(DesignerItem designerItem)
            : base(designerItem)
        {
            this.chrome = new PolygonResizeChrome(designerItem);
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
