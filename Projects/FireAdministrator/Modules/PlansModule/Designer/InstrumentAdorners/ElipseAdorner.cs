using System.Windows.Shapes;
using FiresecAPI.Models;
using PlansModule.Designer;

namespace PlansModule.InstrumentAdorners
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