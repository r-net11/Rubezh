using Common;
using System;

namespace RubezhAPI.Plans.Elements
{
	public interface IElementBackground
	{
		bool AllowTransparent { get; }
		Color BackgroundColor { get; set; }
		Guid? BackgroundImageSource { get; set; }
		Guid? BackgroundSVGImageSource { get; set; }
		string BackgroundSourceName { get; set; }
		ResourceType ImageType { get; set; }
	}
}