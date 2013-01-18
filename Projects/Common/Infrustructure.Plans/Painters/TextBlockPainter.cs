using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class TextBlockPainter : RectanglePainter
	{
		private ScaleTransform _transform;
		private Rect _rect;
		private FormattedText _formattedText;
		public override void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
			base.Draw(drawingContext, element, rect);

			Rect bound = new Rect(rect.Left + element.BorderThickness / 2, rect.Top + element.BorderThickness / 2, rect.Width - element.BorderThickness, rect.Height - element.BorderThickness);
			IElementTextBlock elementText = (IElementTextBlock)element;
			var typeface = new Typeface(new FontFamily(elementText.FontFamilyName), elementText.FontItalic ? FontStyles.Italic : FontStyles.Normal, elementText.FontBold ? FontWeights.Bold : FontWeights.Normal, new FontStretch());
			_formattedText = new FormattedText(elementText.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, elementText.FontSize, PainterCache.GetBrush(elementText.ForegroundColor));
			_formattedText.TextAlignment = (TextAlignment)elementText.TextAlignment;
			Point point = bound.TopLeft;
			switch (_formattedText.TextAlignment)
			{
				case TextAlignment.Right:
					point = bound.TopRight;
					break;
				case TextAlignment.Center:
					point = new Point(bound.Left + bound.Width / 2, bound.Top);
					break;
			}
			_rect = new Rect(point.X, point.Y, _formattedText.Width, _formattedText.Height);
			if (elementText.Stretch)
			{
				_transform = new ScaleTransform(bound.Width / _rect.Width, bound.Height / _rect.Height, _rect.Left, _rect.Top);
				drawingContext.PushTransform(_transform);
			}
			else
			{
				_transform = null;
				//drawingContext.PushClip(new RectangleGeometry(bound));
				drawingContext.PushClip(Geometry);
			}
			drawingContext.DrawText(_formattedText, point);
			drawingContext.Pop();
		}
		protected override void InnerTransform(ElementBase element, Rect rect)
		{
			if (_transform != null && Geometry.Bounds != rect)
			{
				Rect bound = new Rect(rect.Left + element.BorderThickness / 2, rect.Top + element.BorderThickness / 2, rect.Width - element.BorderThickness, rect.Height - element.BorderThickness);
				_transform.CenterX = _rect.Left;
				_transform.CenterY = _rect.Top;
				_transform.ScaleX = bound.Width / _rect.Width;
				_transform.ScaleY = bound.Height / _rect.Height;
			}
			base.InnerTransform(element, rect);
		}
	}
}
