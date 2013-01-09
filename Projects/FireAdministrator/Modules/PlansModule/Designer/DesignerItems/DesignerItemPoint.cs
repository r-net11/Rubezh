using System.Windows.Controls;
using Infrustructure.Plans.Elements;
using PlansModule.Designer.Adorners;
using System.Windows;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemPoint : DesignerItemBase
	{
		public DesignerItemPoint(ElementBase element)
			: base(element)
		{
			ResizeChrome = new ResizeChromePoint(this);
		}

		public override void UpdateZoomPoint()
		{
			if (Shift != DesignerCanvas.PointZoom / 2)
			{
				Shift = DesignerCanvas.PointZoom / 2;
				base.UpdateZoomPoint();
			}
		}
	}
}
