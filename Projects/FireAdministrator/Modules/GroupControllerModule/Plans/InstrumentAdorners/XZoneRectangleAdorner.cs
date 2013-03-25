using FiresecAPI.Models;
using GKModule.Plans.Designer;
using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class XZoneRectangleAdorner : BaseRectangleAdorner
	{
		public XZoneRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleXZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetXZone(element);
			return element;
		}
	}
}