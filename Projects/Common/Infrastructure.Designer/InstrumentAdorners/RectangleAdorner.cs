using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

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