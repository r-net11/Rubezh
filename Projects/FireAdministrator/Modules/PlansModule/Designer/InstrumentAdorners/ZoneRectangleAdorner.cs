using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using PlansModule.Designer;
using PlansModule.ViewModels;
using PlansModule.Designer.Designer;

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
			DialogService.ShowModalWindow(propertiesViewModel);
			Helper.SetZone(element);
			return element;
		}
	}
}