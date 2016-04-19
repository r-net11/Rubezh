using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using PointCollection = Common.PointCollection;

namespace Infrastructure.Designer.Adorners
{
	public class ResizeChromeShape : ResizeChrome
	{
		private int _index;
		private List<TranslateTransform> _transforms;
		private PointCollection _points;

		public ResizeChromeShape(DesignerItem designerItem)
			: base(designerItem)
		{
			PrepareSizableBounds();
		}
		public override void ResetElement()
		{
			base.ResetElement();
			ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
			_points = element == null ? new PointCollection() : element.Points;
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
					_transforms.Add(new TranslateTransform(_points[i].X, _points[i].Y));
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
				DesignerCanvas.DesignerChanged();
			}
		}
		protected override void DragDeltaInner(Point point, Vector shift)
		{
			if (!IsMoved)
				CalculateIndex(point);
			if (_index > -1)
			{
				ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
				var newPoint = new Point(element.Points[_index].X + shift.X, element.Points[_index].Y + shift.Y);
				if (newPoint.X < 0)
					newPoint.X = 0;
				else if (newPoint.X > DesignerCanvas.CanvasWidth)
					newPoint.X = DesignerCanvas.CanvasWidth;
				if (newPoint.Y < 0)
					newPoint.Y = 0;
				else if (newPoint.Y > DesignerCanvas.CanvasHeight)
					newPoint.Y = DesignerCanvas.CanvasHeight;
				element.Points[_index] = newPoint;
				DesignerItem.RefreshPainter();
				DesignerCanvas.DesignerChanged();
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