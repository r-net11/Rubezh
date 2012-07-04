using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;

namespace PlansModule.Designer
{
    public class ResizeAdornerPolyline : Adorner
    {
        private VisualCollection visuals;
        private ResizeChromePolyline chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public ResizeAdornerPolyline(DesignerItem designerItem)
            : base(designerItem)
        {
            this.chrome = new ResizeChromePolyline(designerItem);
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