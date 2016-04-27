using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class GuardZoneViewModel : BaseViewModel
	{
		public GKGuardZone Zone { get; private set; }

		public GuardZoneViewModel(GKGuardZone zone)
		{
			Zone = zone;
			zone.PlanElementUIDsChanged += Update;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Zone);
			if (Zone.PlanElementUIDs == null)
				return;
			_visualizationState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}

		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
		}
	}
}