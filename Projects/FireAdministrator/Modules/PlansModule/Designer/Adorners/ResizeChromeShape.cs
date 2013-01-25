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
		private List<TranslateTransform> _transforms;
		private PointCollection _points;

		public ResizeChromeShape(DesignerItem designerItem)
			: base(designerItem)
		{
			ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
			_points = element == null ? new PointCollection() : element.Points;
			PrepareSizableBounds();
			_transforms = new List<TranslateTransform>();
			for (int i = 0; i < _points.Count; i++)
				_transforms.Add(new TranslateTransform());
		}

		protected override void Draw(DrawingContext drawingContext)
		{
			DrawSizableBounds(drawingContext);
			if (_transforms.Count != _points.Count)
			{
				_transforms.Clear();
				for (int i = 0; i < _points.Count; i++)
					_transforms.Add(new TranslateTransform());
			}
			for (int i = 0; i < _transforms.Count; i++)
				DrawThumb(drawingContext, _transforms[i]);
		}
		protected override void Translate()
		{
			base.Translate();
			if (_transforms.Count != _points.Count)
				DesignerCanvas.Refresh();
			else
				for (int i = 0; i < _points.Count; i++)
				{
					_transforms[i].X = _points[i].X;
					_transforms[i].Y = _points[i].Y;
				}
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

				_points = new PointCollection();
				foreach (var point in element.Points)
					_points.Add(new Point(placeholder.X + kx * (point.X - rect.X), placeholder.Y + ky * (point.Y - rect.Y)));
				element.Points = _points;

				DesignerItem.RefreshPainter();
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
				DesignerItem.RefreshPainter();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		private void CalculateIndex(Point point)
		{
			_index = -1;
			ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
			for (int i = 0; i < element.Points.Count; i++)
				if (IsInsideThumb(element.Points[i], point))
				{
					_index = i;
					break;
				}
		}

		public override IVisualItem HitTest(Point point)
		{
			foreach (var anglePoint in _points)
				if (IsInsideThumb(anglePoint, point))
					return this;
			return base.HitTest(point);
		}
	}
}
