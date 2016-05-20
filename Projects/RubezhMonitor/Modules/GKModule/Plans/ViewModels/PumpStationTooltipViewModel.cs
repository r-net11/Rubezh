using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class PumpStationTooltipViewModel : StateTooltipViewModel<GKPumpStation>
	{
		public GKPumpStation PumpStation
		{
			get { return base.Item; }
		}

		public PumpStationTooltipViewModel(GKPumpStation pumpStation)
			: base(pumpStation)
		{
		}
	}
}
