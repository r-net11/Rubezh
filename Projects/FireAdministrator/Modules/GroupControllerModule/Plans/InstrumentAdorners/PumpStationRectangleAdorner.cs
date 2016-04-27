using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class PumpStationRectangleAdorner : BaseRectangleAdorner
	{
		public PumpStationRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}
		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKPumpStation();
			var propertiesViewModel = new PumpStationPropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
				return element;
			return null;
		}
	}
}
