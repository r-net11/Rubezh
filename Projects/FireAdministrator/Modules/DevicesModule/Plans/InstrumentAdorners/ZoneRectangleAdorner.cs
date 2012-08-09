using DevicesModule.Plans.Designer;
using DevicesModule.Plans.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using PlansModule.InstrumentAdorners;

namespace DevicesModule.Plans.InstrumentAdorners
{
	public class ZoneRectangleAdorner : BaseRectangleAdorner
	{
		public ZoneRectangleAdorner(CommonDesignerCanvas designerCanvas)
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