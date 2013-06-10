using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public interface IGeometryPainter : IPainter
	{
		Geometry Geometry { get; }
	}
}
