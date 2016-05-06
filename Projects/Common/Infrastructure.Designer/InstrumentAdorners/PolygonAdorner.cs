using System.Windows.Media;
using System.Windows.Shapes;
using StrazhAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class PolygonAdorner : BasePolygonAdorner
	{
		public PolygonAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Shape CreateRubberband()
		{
			return new Polygon();
		}
		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement()
		{
			return new ElementPolygon();
		}
	}
}