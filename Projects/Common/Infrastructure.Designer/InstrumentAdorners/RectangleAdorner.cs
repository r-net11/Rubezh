using StrazhAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class RectangleAdorner : BaseRectangleAdorner
	{
		public RectangleAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement()
		{
			return new ElementRectangle();
		}
	}
}