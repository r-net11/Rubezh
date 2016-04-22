using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class PumpStationRectangleAdorner : BaseRectangleAdorner
	{
		private PumpStationsViewModel _pumpStationsViewModel = null;
		public PumpStationRectangleAdorner(CommonDesignerCanvas designerCanvas, PumpStationsViewModel pumpStationsViewModel)
			: base(designerCanvas)
		{
			_pumpStationsViewModel = pumpStationsViewModel;
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKPumpStation();
			var propertiesViewModel = new PumpStationPropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
			{
				_pumpStationsViewModel.UpdatePumpStations(element.PumpStationUID);
				return element;
			}
			return null;
		}
	}
}
