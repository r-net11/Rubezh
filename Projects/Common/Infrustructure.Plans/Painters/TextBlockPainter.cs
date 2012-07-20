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
			var textBlock = new Label()
			{
				Content = elementText.Text,
				Background = new SolidColorBrush(element.BackgroundColor),
				Foreground = new SolidColorBrush(elementText.ForegroundColor),
				BorderBrush = new SolidColorBrush(element.BorderColor),
				BorderThickness = new Thickness(element.BorderThickness),
				FontSize = elementText.FontSize,
				FontFamily = new FontFamily(elementText.FontFamilyName),
			};
			FrameworkElement frameworkElement = elementText.Stretch ?
				new Viewbox()
				{
					Stretch = Stretch.Fill,
					Child = textBlock
				} : (FrameworkElement)textBlock;
			return frameworkElement;
		}
		#endregion
	}
}
