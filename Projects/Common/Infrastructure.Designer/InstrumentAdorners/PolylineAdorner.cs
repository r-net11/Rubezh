using System.Windows.Media;
using System.Windows.Shapes;
using StrazhAPI.Models;
using Infrustructure.Plans.Elements;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class PolylineAdorner : PolygonAdorner
	{
		public PolylineAdorner(DesignerCanvas designerCanvas)
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
		protected override ElementBaseShape CreateElement()
		{
			return new ElementPolyline();
		}
	}
}