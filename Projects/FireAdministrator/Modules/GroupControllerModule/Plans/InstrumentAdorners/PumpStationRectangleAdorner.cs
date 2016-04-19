using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class PumpStationRectangleAdorner : BaseRectangleAdorner
	{
		public PumpStationRectangleAdorner(CommonDesignerCanvas designerCanvas, PumpStationsViewModel pumpStationsViewModel)
			: base(designerCanvas)
		{
			this.pumpStationsViewModel = pumpStationsViewModel;
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKPumpStation();
			var propertiesViewModel = new PumpStationPropertiesViewModel(element, this.pumpStationsViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKPumpStation>(element);
			return element;
		}

		#region Fields

		private PumpStationsViewModel pumpStationsViewModel = null;

		#endregion
	}
}
