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

		public GeometryPainter(ElementBase element)
		{
			Element = element;
		}

		protected abstract T CreateGeometry();
		public abstract void Transform();

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
		public virtual void Draw(DrawingContext drawingContext)
		{
			Geometry = CreateGeometry();
			Transform();
			//Geometry.Freeze();
			drawingContext.DrawGeometry(GetBrush(), GetPen(), Geometry);
		}

		#region IGeometryPainter Members

		Geometry IGeometryPainter.Geometry
		{
			get { return Geometry; }
		}

		#endregion
	}
}
