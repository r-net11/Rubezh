using System;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementBackground
	{
		bool AllowTransparent { get; }
		Color BackgroundColor { get; set; }
		Guid? BackgroundImageSource { get; set; }
		string BackgroundSourceName { get; set; }
		bool IsVectorImage { get; set; }
	}
}