using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class TextBlockPainter : IPainter
	{
		#region IPainter Members

		public FrameworkElement Draw(ElementBase element)
		{
			IElementTextBlock elementText = (IElementTextBlock)element;
			var textBlock = new TextBlock()
			{
				Text = elementText.Text,
				TextAlignment = (TextAlignment)elementText.TextAlignment,
				Background = new SolidColorBrush(element.BackgroundColor),
				Foreground = new SolidColorBrush(elementText.ForegroundColor),
				FontSize = elementText.FontSize,
				FontWeight = elementText.FontBold ? FontWeights.Bold : FontWeights.Normal,
				FontStyle = elementText.FontItalic ? FontStyles.Italic : FontStyles.Normal,
				FontFamily = new FontFamily(elementText.FontFamilyName),
			};
			FrameworkElement frameworkElement = elementText.Stretch ?
				new Viewbox()
				{
					Stretch = Stretch.Fill,
					Child = textBlock
				} : (FrameworkElement)textBlock;
			Border border = new Border()
			{
				BorderBrush = new SolidColorBrush(element.BorderColor),
				BorderThickness = new Thickness(element.BorderThickness),
				Child = frameworkElement,
			};
			return border;
		}
		#endregion
	}
}
