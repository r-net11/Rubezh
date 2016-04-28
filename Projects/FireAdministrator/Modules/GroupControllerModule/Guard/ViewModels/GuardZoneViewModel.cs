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
			zone.PlanElementUIDsChanged += UpdateVisualizationState;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Zone);
			UpdateVisualizationState();

		}
		void UpdateVisualizationState()
		{
			VisualizationState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
		}
		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
			private set
			{
				_visualizationState = value;
				OnPropertyChanged(() => VisualizationState);
			}
		}
	}
}