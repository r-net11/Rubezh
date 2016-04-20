using System.Windows.Media;

namespace Infrastructure.Plans.Painters
{
	public interface IGeometryPainter : IPainter
	{
		Geometry Geometry { get; }
	}
}