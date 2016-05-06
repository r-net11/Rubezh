using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using StrazhModule.Plans.ViewModels;
using StrazhModule.ViewModels;

namespace StrazhModule.Plans.InstrumentAdorners
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