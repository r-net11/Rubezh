using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;

namespace PlansModule.Designer.Adorners
{
	public class ResizeChromeShape : ResizeChrome
	{
		private int _index;

		public ResizeChromeShape(DesignerItem designerItem)
			: base(designerItem)
		{
			//Loaded += new RoutedEventHandler(ResizeChromeShape_Loaded);
		}

		protected override void Render(DrawingContext drawingContext)
		{
			ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
			DrawSizableBounds(drawingContext);
			foreach (var point in element.Points)
				DrawThumb(drawingContext, point);
		}
		protected override void Resize(ResizeDirection direction, Vector vector)
		{
			ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
			if (element != null)
			{
				var rect = element.GetRectangle();
				var placeholder = new Rect(rect.TopLeft, rect.Size);
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top)
				{
					placeholder.Y += vector.Y;
					placeholder.Height -= vector.Y;
				}
				else if ((direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
					placeholder.Height += vector.Y;
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left)
				{
					placeholder.X += vector.X;
					placeholder.Width -= vector.X;
				}
				else if ((direction & ResizeDirection.Right) == ResizeDirection.Right)
					placeholder.Width += vector.X;
				double kx = rect.Width == 0 ? 0 : placeholder.Width / rect.Width;
				double ky = rect.Height == 0 ? 0 : placeholder.Height / rect.Height;

				PointCollection points = new PointCollection();
				foreach (var point in element.Points)
					points.Add(new Point(placeholder.X + kx * (point.X - rect.X), placeholder.Y + ky * (point.Y - rect.Y)));
				element.Points = points;

				DesignerItem.Redraw();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		protected override void DragDeltaInner(Point point, Vector shift)
		{
			if (!IsMoved)
				CalculateIndex(point);
			if (_index > -1)
			{
				ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
				double x = element.Points[_index].X + shift.X;
				if (x < 0)
					x = 0;
				else if (x > DesignerCanvas.CanvasWidth)
					x = DesignerCanvas.CanvasWidth;
				double y = element.Points[_index].Y + shift.Y;
				if (y < 0)
					y = 0;
				else if (y > DesignerCanvas.CanvasHeight)
					y = DesignerCanvas.CanvasHeight;
				element.Points[_index] = new Point(x, y);
				DesignerItem.Redraw();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		private void CalculateIndex(Point point)
		{
			_index = -1;
			ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
			for (int i = 0; i < element.Points.Count; i++)
				if (IsInsideThumb(DesignerItem.Transform.Transform(element.Points[i]), point))
				{
					_index = i;
					break;
				}
		}
	}
}
