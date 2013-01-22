using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public abstract class GeometryPainter<T> : IGeometryPainter
		where T : Geometry
	{
		protected T Geometry { get; private set; }
		protected ElementBase Element { get; private set; }
		protected Rect Rect { get; private set; }
		protected Brush Brush { get; private set; }
		protected Pen Pen { get; private set; }
		private ScaleTransform _scaleTransform;

		public GeometryPainter(ElementBase element)
		{
			_scaleTransform = new ScaleTransform();
			Element = element;
		}

		protected abstract T CreateGeometry();
		protected virtual void InnerDraw(DrawingContext drawingContext)
		{
			Geometry = CreateGeometry();
			Transform();
			//Geometry.Freeze();
			Brush = GetBrush();
			Pen = GetPen();
			drawingContext.DrawGeometry(Brush, Pen, Geometry);
		}

		protected virtual Brush GetBrush()
		{
			return PainterCache.GetBrush(Element.BackgroundColor, Element.BackgroundPixels);
		}
		protected virtual Pen GetPen()
		{
			return PainterCache.GetPen(Element.BorderColor, Element.BorderThickness);
		}
		protected void CalculateRectangle()
		{
			Rect = Element.GetRectangle();
		}

		#region IGeometryPainter Members

		Geometry IGeometryPainter.Geometry
		{
			get { return Geometry; }
		}

		#endregion

		#region IPainter Members

		public Rect Bounds
		{
			get { return Geometry.GetRenderBounds(Pen); }
		}

		public void Draw(DrawingContext drawingContext)
		{
			drawingContext.PushTransform(_scaleTransform);
			InnerDraw(drawingContext);
			drawingContext.Pop();
		}
		public abstract void Transform();

		public void Show()
		{
			_scaleTransform.ScaleX = 1;
			_scaleTransform.ScaleY = 1;
		}
		public void Hide()
		{
			_scaleTransform.ScaleX = 0;
			_scaleTransform.ScaleY = 0;
		}

		public bool HitTest(Point point)
		{
			return Geometry == null ? false : Geometry.FillContains(point) || Geometry.StrokeContains(Pen, point);
		}

		#endregion
	}
}
