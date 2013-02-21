using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;
using PlansModule.Designer;

namespace PlansModule.InstrumentAdorners
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