using System.Windows.Shapes;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

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
		protected override ElementBaseRectangle CreateElement()
		{
			return new ElementEllipse();
		}
	}
}