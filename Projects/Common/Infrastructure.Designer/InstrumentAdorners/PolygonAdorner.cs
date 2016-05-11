using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class PolygonAdorner : BasePolygonAdorner
	{
		public PolygonAdorner(BaseDesignerCanvas designerCanvas)
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
		protected override ElementBaseShape CreateElement(RubezhAPI.PointCollection points)
		{
			return new ElementPolygon { Points = points };
		}
	}
}