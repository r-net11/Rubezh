using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class ZoneRectangleAdorner : BaseRectangleAdorner
	{
		ZonesViewModel _zonesViewModel;

		public ZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, ZonesViewModel zonesViewModel)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
			{
				_zonesViewModel.UpdateZones(element.ZoneUID);
				return element;
			}
			return null;
		}
	}
}