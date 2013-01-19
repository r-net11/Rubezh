using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows.Controls;
using System;

namespace Infrustructure.Plans.Painters
{
	public abstract class GeometryPainter<T> : IGeometryPainter
		where T : Geometry
	{
		protected T Geometry { get; private set; }
		protected Brush Brush { get; private set; }
		protected Pen Pen { get; private set; }
		protected SolidColorBrush PenBrush { get; private set; }
		protected GeometryDrawing GeometryDrawing { get; private set; }
		protected SolidColorBrush SolidColorBrush { get { return Brush as SolidColorBrush; } }
		protected ImageBrush ImageBrush { get { return Brush as ImageBrush; } }

		#region IPainter Members

		private bool _isVisible;
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				UpdateVisible();
			}
		}
		public virtual Rect Bounds
		{
			get { return GeometryDrawing == null ? Rect.Empty : GeometryDrawing.Bounds; }
		}

		public virtual void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
			Geometry = CreateShape();
			Brush = CreateBrush(element, rect);
			Pen = CreatePen(element, rect);
			PenBrush = Pen == null ? null : Pen.Brush as SolidColorBrush;
			GeometryDrawing = new GeometryDrawing(Brush, Pen, Geometry);
			UpdateVisible();
			Redraw(element, rect);
			drawingContext.DrawDrawing(GeometryDrawing);
		}
		public void Redraw(ElementBase element, Rect rect)
		{
			System.Console.WriteLine("{0} {1} REDRAW", DateTime.Now, element);
			if (GeometryDrawing != null)
			{
				UpdateBrush(element, rect);
				UpdatePen(element, rect);
				Transform(element, rect);
				InnerRedraw(element, rect);
			}
		}
		public void Transform(ElementBase element, Rect rect)
		{
			System.Console.WriteLine("{0} {1} Transform", DateTime.Now, element);
			if (!Geometry.IsFrozen)
				InnerTransform(element, rect);
		}

		public bool HitTest(Point point)
		{
			if (Geometry == null)
				return false;
			return Geometry.StrokeContains(Pen, point) || Geometry.FillContains(point);
		}

		#endregion

		protected abstract T CreateShape();
		protected abstract void InnerTransform(ElementBase element, Rect rect);
		protected virtual void InnerRedraw(ElementBase element, Rect rect)
		{
		}
		protected virtual Brush CreateBrush(ElementBase element, Rect rect)
		{
			if (element.BackgroundPixels != null)
				return new ImageBrush();
			else
				return new SolidColorBrush();
		}
		protected virtual Pen CreatePen(ElementBase element, Rect rect)
		{
			var pen = new Pen();
			pen.Brush = new SolidColorBrush();
			return pen;
		}
		protected virtual void UpdateBrush(ElementBase element, Rect rect)
		{
			if (SolidColorBrush != null && !SolidColorBrush.IsFrozen)
				SolidColorBrush.Color = element.BackgroundColor;
			if (ImageBrush != null && !ImageBrush.IsFrozen)
				ImageBrush.ImageSource = element.BackgroundPixels == null || element.BackgroundPixels.Length == 0 ? null : PainterHelper.CreateImage(element.BackgroundPixels);
		}
		protected virtual void UpdatePen(ElementBase element, Rect rect)
		{
			if (PenBrush != null && !PenBrush.IsFrozen)
				PenBrush.Color = element.BorderColor;
			if (Pen != null && !Pen.IsFrozen)
				Pen.Thickness = element.BorderThickness;
		}
		protected virtual void UpdateVisible()
		{
			if (GeometryDrawing != null)
			{
				GeometryDrawing.Brush = IsVisible ? Brush : null;
				GeometryDrawing.Pen = IsVisible ? Pen : null;
			}
		}

		#region IGeometryPainter Members

		Geometry IGeometryPainter.Geometry
		{
			get { return Geometry; }
		}

		#endregion

	}
}
