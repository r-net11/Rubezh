using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public abstract class GeometryPainter<T> : IPainter
		where T : Geometry
	{
		protected T Geometry { get; private set; }
		protected Rect Rect { get; private set; }
		protected SolidColorBrush SolidColorBrush { get; private set; }
		protected ImageBrush ImageBrush { get; private set; }
		protected SolidColorBrush PenBrush { get; private set; }
		protected Pen Pen { get; private set; }

		#region IPainter Members

		public bool CanTransform
		{
			get { return true; }
		}

		public virtual void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
			Rect = rect;
			Geometry = CreateShape();
			InitializeBrushes(element, rect);
			Transform(element, rect);
			drawingContext.DrawGeometry(SolidColorBrush, null, Geometry);
			drawingContext.DrawGeometry(ImageBrush, Pen, Geometry);
		}

		public void Transform(ElementBase element, Rect rect)
		{
			UpdateImageBrush(element, rect);
			UpdateSolidColorBrush(element, rect);
			UpdatePen(element, rect);
			if (!Geometry.IsFrozen && Geometry.Bounds != rect)
				InnerTransform(element, rect);
		}

		#endregion

		protected abstract T CreateShape();
		protected abstract void InnerTransform(ElementBase element, Rect rect);
		protected virtual void InitializeBrushes(ElementBase element, Rect rect)
		{
			SolidColorBrush = new SolidColorBrush();
			ImageBrush = new ImageBrush();
		}
		protected virtual void InitializePen(ElementBase element, Rect rect)
		{
			PenBrush = new SolidColorBrush();
			Pen = new Pen();
			Pen.Brush = PenBrush;
		}
		protected virtual void UpdateSolidColorBrush(ElementBase element, Rect rect)
		{
			if (SolidColorBrush != null && !SolidColorBrush.IsFrozen && SolidColorBrush.Color != element.BackgroundColor)
				SolidColorBrush.Color = element.BackgroundColor;
		}
		protected virtual void UpdateImageBrush(ElementBase element, Rect rect)
		{
			if (ImageBrush != null && !ImageBrush.IsFrozen)
				ImageBrush.ImageSource = element.BackgroundPixels == null || element.BackgroundPixels.Length == 0 ? null : PainterHelper.CreateImage(element.BackgroundPixels);
		}
		protected virtual void UpdatePen(ElementBase element, Rect rect)
		{
			if (PenBrush != null && !PenBrush.IsFrozen && PenBrush.Color != element.BorderColor)
				PenBrush.Color = element.BorderColor;
			if (Pen != null && !Pen.IsFrozen && Pen.Thickness != element.BorderThickness)
				Pen.Thickness = element.BorderThickness;
		}
	}
}
