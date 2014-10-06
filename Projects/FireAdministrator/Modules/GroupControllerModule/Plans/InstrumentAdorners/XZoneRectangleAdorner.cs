using FiresecAPI.GK;
using FiresecAPI.Models;
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

		public XZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, ZonesViewModel zonesViewModel)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKZone>(element);
			return element;
		}
	}
}