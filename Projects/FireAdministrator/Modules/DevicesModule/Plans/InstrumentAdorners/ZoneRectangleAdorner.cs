using DevicesModule.Plans.Designer;
using DevicesModule.Plans.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;

namespace DevicesModule.Plans.InstrumentAdorners
{
	public class ZoneRectangleAdorner : BaseRectangleAdorner
	{
		public ZoneRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetZone(element);
			return element;
		}
	}
}