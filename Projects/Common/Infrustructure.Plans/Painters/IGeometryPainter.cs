using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public interface IGeometryPainter : IPainter
	{
		Geometry Geometry { get; }
	}
}
