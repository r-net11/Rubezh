using Common;

namespace RubezhAPI.Plans.Elements
{
	public interface IElementTextBlock : IElementBackground, IElementBorder, IElementBase
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