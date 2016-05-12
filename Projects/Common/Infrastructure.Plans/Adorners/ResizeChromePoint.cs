using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Windows;
using System.Windows.Media;

namespace Infrastructure.Plans.Adorners
{
	public class ResizeChromePoint : ResizeChrome
	{
		public ResizeChromePoint(DesignerItem designerItem)
			: base(designerItem)
		{
			PrepareBounds();
		}

		protected override void Draw(DrawingContext drawingContext)
		{
			DrawBounds(drawingContext);
		}
		protected override void Resize(ResizeDirection direction, Vector vector)
		{
			ElementBasePoint element = DesignerItem.Element as ElementBasePoint;
			if (element != null)
			{
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top || (direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
					element.Top += vector.Y;
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left || (direction & ResizeDirection.Right) == ResizeDirection.Right)
					element.Left += vector.X;
				//DesignerItem.Translate();
				DesignerItem.RefreshPainter();
				DesignerCanvas.DesignerChanged();
			}
		}
	}
}