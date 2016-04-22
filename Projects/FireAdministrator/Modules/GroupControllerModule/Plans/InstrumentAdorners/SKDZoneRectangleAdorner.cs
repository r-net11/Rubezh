using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class SKDZoneRectangleAdorner : BaseRectangleAdorner
	{
		SKDZonesViewModel _skdZonesViewModel;

		public SKDZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, SKDZonesViewModel skdZonesViewModel)
			: base(designerCanvas)
		{
			_skdZonesViewModel = skdZonesViewModel;
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKSKDZone();
			var propertiesViewModel = new SKDZonePropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
			{
				_skdZonesViewModel.UpdateZones(element.ZoneUID);
				return element;
			}
			return null;
		}
	}
}