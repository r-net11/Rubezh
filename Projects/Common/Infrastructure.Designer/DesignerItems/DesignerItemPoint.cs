using System.Windows;
using Infrastructure.Designer.Adorners;
using StrazhAPI.Plans.Elements;

namespace Infrastructure.Designer.DesignerItems
{
	public class DesignerItemPoint : DesignerItemBase
	{
		public DesignerItemPoint(ElementBase element)
			: base(element)
		{
			SetResizeChrome(new ResizeChromePoint(this));
		}

		public override void UpdateZoomPoint()
		{
			//RefreshPainter();
			if (ResizeChrome != null)
				ResizeChrome.InvalidateVisual();
		}

		public override Rect GetRectangle()
		{
			var rect = base.GetRectangle();
			if (DesignerCanvas == null)
				return rect;
			return new Rect(rect.Left - DesignerCanvas.PointZoom / 2, rect.Top - DesignerCanvas.PointZoom / 2, DesignerCanvas.PointZoom, DesignerCanvas.PointZoom);
		}
	}
}