using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Infrastructure.Plans.InstrumentAdorners
{
	public class PolylineAdorner : PolygonAdorner
	{
		public PolylineAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Shape CreateRubberband()
		{
			return new Polyline();
		}
		protected override PointCollection Points
		{
			get { return ((Polyline)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement(RubezhAPI.PointCollection points)
		{
			return new ElementPolyline() { Points = points };
		}
	}
}