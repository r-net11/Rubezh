using RubezhAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class GuardZoneViewModel : BaseViewModel
	{
		public GKGuardZone Zone { get; private set; }

		public GuardZoneViewModel(GKGuardZone zone)
		{
			Zone = zone;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Zone);
			if (Zone.PlanElementUIDs == null)
				return;
			_visualizetionState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}

		VisualizationState _visualizetionState;
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
	}
}