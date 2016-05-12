using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Windows;
using System.Windows.Media;

namespace Infrastructure.Plans.Adorners
{
	public class ResizeChromeRectangle : ResizeChrome
	{
		public ResizeChromeRectangle(DesignerItem designerItem)
			: base(designerItem)
		{
			PrepareSizableBounds();
		}

		protected override void Draw(DrawingContext drawingContext)
		{
			DrawSizableBounds(drawingContext);
		}
		protected override void Resize(ResizeDirection direction, Vector vector)
		{
			ElementBaseRectangle element = DesignerItem.Element as ElementBaseRectangle;
			if (element != null)
			{
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top)
				{
					element.Top += vector.Y;
					element.Height -= vector.Y;
				}
				else if ((direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
					element.Height += vector.Y;
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left)
				{
					element.Left += vector.X;
					element.Width -= vector.X;
				}
				else if ((direction & ResizeDirection.Right) == ResizeDirection.Right)
					element.Width += vector.X;
				if (element.Height < 0)
					element.Height = 0;
				if (element.Width < 0)
					element.Width = 0;
				DesignerItem.RefreshPainter();
				DesignerCanvas.DesignerChanged();
			}
		}
	}
}