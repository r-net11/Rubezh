using System.Windows.Shapes;
using FiresecAPI.Models;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class ElipseAdorner : RectangleAdorner
	{
		public ElipseAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Shape CreateRubberband()
		{
			return new Ellipse();
		}
		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			return new ElementEllipse();
		}
	}
}