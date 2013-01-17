using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class TextBlockPainter : RectanglePainter
	{
		public override void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
			base.Draw(drawingContext, element, rect);

			Rect bound = new Rect(rect.Left + element.BorderThickness / 2, rect.Top + element.BorderThickness / 2, rect.Width - element.BorderThickness, rect.Height - element.BorderThickness);
			IElementTextBlock elementText = (IElementTextBlock)element;
			var typeface = new Typeface(new FontFamily(elementText.FontFamilyName), elementText.FontItalic ? FontStyles.Italic : FontStyles.Normal, elementText.FontBold ? FontWeights.Bold : FontWeights.Normal, new FontStretch());
			var formattedText = new FormattedText(elementText.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, elementText.FontSize, PainterCache.GetBrush(elementText.ForegroundColor));
			formattedText.TextAlignment = (TextAlignment)elementText.TextAlignment;
			Point point = bound.TopLeft;
			switch (formattedText.TextAlignment)
			{
				case TextAlignment.Right:
					point = bound.TopRight;
					break;
				case TextAlignment.Center:
					point = new Point(bound.Left + bound.Width / 2, bound.Top);
					break;
			}
			if (elementText.Stretch)
				drawingContext.PushTransform(new ScaleTransform(bound.Width / formattedText.Width, bound.Height / formattedText.Height, point.X, point.Y));
			else
				//drawingContext.PushClip(new RectangleGeometry(bound));
				drawingContext.PushClip(Geometry);
			drawingContext.DrawText(formattedText, point);
			drawingContext.Pop();
		}
	}
}
