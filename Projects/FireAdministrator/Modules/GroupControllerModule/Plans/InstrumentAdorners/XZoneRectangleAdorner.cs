using FiresecAPI.Models;
using GKModule.Plans.Designer;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class XZoneRectangleAdorner : BaseRectangleAdorner
	{
		ZonesViewModel _zonesViewModel;
		GuardZonesViewModel _guardZonesViewModel;

		public XZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, ZonesViewModel zonesViewModel, GuardZonesViewModel guardZonesViewModel)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
			_guardZonesViewModel = guardZonesViewModel;
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleXZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel, _guardZonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetXZone(element);
			return element;
		}
	}
}