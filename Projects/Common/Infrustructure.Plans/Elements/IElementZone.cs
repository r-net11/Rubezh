using System;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZone : IElementZLayer
	{
		int ZLayerIndex { get; set; }
        Guid ZoneUID { get; set; }
		Color BackgroundColor { get; set; }
	}
}