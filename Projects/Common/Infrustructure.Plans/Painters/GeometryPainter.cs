using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

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

		public GeometryPainter(ElementBase element)
		{
			Element = element;
			Geometry = CreateGeometry();
		}

		public void ResetElement(ElementBase element)
		{
			bool recreateGeometry = element.GetType() == Element.GetType();
			Element = element;
			if (recreateGeometry)
				Geometry = CreateGeometry();
		}

		protected abstract T CreateGeometry();
		protected virtual void InnerDraw(DrawingContext drawingContext)
		{
			drawingContext.DrawGeometry(Brush, Pen, Geometry);
		}

		protected virtual Brush GetBrush()
		{
			return PainterCache.GetBrush(Element);
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

		public virtual Rect Bounds
		{
			get { return Geometry.GetRenderBounds(Pen); }
		}

		public void Draw(DrawingContext drawingContext)
		{
			if (Geometry == null)
				Invalidate();
			InnerDraw(drawingContext);
		}
		public virtual void Invalidate()
		{
			//Geometry.Freeze();
			Brush = GetBrush();
			Pen = GetPen();
			Transform();
		}
		public abstract void Transform();

		public virtual bool HitTest(Point point)
		{
			return Geometry == null ? false : (Brush != null && Geometry.Bounds.Contains(point) && Geometry.FillContains(point, 0, ToleranceType.Absolute)) || Geometry.StrokeContains(Pen, point, 0, ToleranceType.Absolute);
		}

		public virtual object GetToolTip(string title)
		{
			return string.IsNullOrEmpty(title) ? null : title;
		}

		#endregion
	}
}
