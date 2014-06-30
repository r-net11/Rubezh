using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using SKDModule.Plans.ViewModels;
using SKDModule.ViewModels;

namespace SKDModule.Plans.InstrumentAdorners
{
	public class SKDZoneRectangleAdorner : BaseRectangleAdorner
	{
		private ZonesViewModel _zonesViewModel;
		public SKDZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, ZonesViewModel zonesViewModel)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleSKDZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			SKDPlanExtension.Instance.SetItem<SKDZone>(element);
			return element;
		}
	}
}