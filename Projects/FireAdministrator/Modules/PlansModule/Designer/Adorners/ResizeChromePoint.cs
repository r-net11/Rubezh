using System.Windows;
using Infrastructure;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using System.Windows.Media;

namespace PlansModule.Designer.Adorners
{
	public class ResizeChromePoint : ResizeChrome
	{
		public ResizeChromePoint(DesignerItem designerItem)
			: base(designerItem)
		{
		}

		protected override void Render(DrawingContext drawingContext)
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
				DesignerItem.Transform();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
	}
}
