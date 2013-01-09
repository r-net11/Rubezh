using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Globalization;

namespace Infrustructure.Plans.Painters
{
	public class TextBlockPainter : RectanglePainter
	{
		public override void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
			base.Draw(drawingContext, element, rect);
			drawingContext.DrawGeometry(GetTextBrush(element), null, Geometry);
		}
		private Brush GetTextBrush(ElementBase element)
		{
			IElementTextBlock elementText = (IElementTextBlock)element;
			var textBlock = new TextBlock()
			{
				Text = elementText.Text,
				TextAlignment = (TextAlignment)elementText.TextAlignment,
				Foreground = PainterCache.GetBrush(elementText.ForegroundColor),
				Background = null,
				FontSize = elementText.FontSize,
				FontWeight = elementText.FontBold ? FontWeights.Bold : FontWeights.Normal,
				FontStyle = elementText.FontItalic ? FontStyles.Italic : FontStyles.Normal,
				FontFamily = new FontFamily(elementText.FontFamilyName),
			};
			var brush = new VisualBrush(textBlock)
			{
				AlignmentX = AlignmentX.Center,
				AlignmentY = AlignmentY.Top,
				Stretch = elementText.Stretch ? Stretch.Fill : Stretch.None,
			};
			switch (textBlock.TextAlignment)
			{
				case TextAlignment.Left:
					brush.AlignmentX = AlignmentX.Left;
					break;
				case TextAlignment.Right:
					brush.AlignmentX = AlignmentX.Right;
					break;
			}
			return brush;
		}
	}
}
