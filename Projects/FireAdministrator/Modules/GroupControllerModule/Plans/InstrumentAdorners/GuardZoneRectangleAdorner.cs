using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class GuardZoneRectangleAdorner : BaseRectangleAdorner
	{
		GuardZonesViewModel _guardZonesViewModel;

		public GuardZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, GuardZonesViewModel guardZonesViewModel)
			: base(designerCanvas)
		{
			_guardZonesViewModel = guardZonesViewModel;
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKGuardZone();
			var propertiesViewModel = new GuardZonePropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
			{
				_guardZonesViewModel.UpdateZones(element.ZoneUID);
				return element;
			}
			return null;
		}
	}
}