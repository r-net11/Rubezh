using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementTextBlock : IElementBackground, IElementBorder
	{	
		string Text { get; set; }
		Color ForegroundColor { get; set; }
		double FontSize { get; set; }
		string FontFamilyName { get; set; }
		int TextAlignment { get; set; }
		int VerticalAlignment { get; set; }
		bool Stretch { get; set; }
		bool FontItalic { get; set; }
		bool FontBold { get; set; }
		bool WordWrap { get; set; }
	}
}