using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using PlansModule.Designer;
using PlansModule.ViewModels;

namespace PlansModule.InstrumentAdorners
{
	public class ZoneRectangleAdorner : RectangleAdorner
	{
		public ZoneRectangleAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}