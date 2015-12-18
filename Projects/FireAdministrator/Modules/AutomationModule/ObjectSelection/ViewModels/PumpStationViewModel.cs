using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class PumpStationViewModel : BaseViewModel
	{
		public GKPumpStation PumpStation { get; private set; }

		public PumpStationViewModel(GKPumpStation pumpStation)
		{
			PumpStation = pumpStation;
		}
	}
}
