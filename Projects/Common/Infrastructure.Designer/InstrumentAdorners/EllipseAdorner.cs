using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Shapes;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class EllipseAdorner : RectangleAdorner
	{
		public EllipseAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Shape CreateRubberband()
		{
			return new Ellipse();
		}
		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			return new ElementEllipse() { Left = left, Top = top };
		}
	}
}