using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZone
	{
		int? ZoneNo { get; set; }
		Color BackgroundColor { get; set; }
	}
}