using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZone : IElementZLayer
	{
		int ZLayerIndex { get; set; }
		int? ZoneNo { get; set; }
		Color BackgroundColor { get; set; }
	}
}