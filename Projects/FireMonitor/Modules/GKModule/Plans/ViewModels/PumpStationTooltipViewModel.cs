using RubezhAPI.GK;
using Infrastructure.Client.Plans.Presenter;

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
