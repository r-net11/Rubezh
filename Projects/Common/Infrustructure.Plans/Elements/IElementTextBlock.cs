using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementTextBlock
	{
		string Text { get; set; }
		Color ForegroundColor { get; set; }
		double FontSize { get; set; }
		string FontFamilyName { get; set; }
		bool Stretch { get; set; }
	}
}