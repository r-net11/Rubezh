using RubezhAPI.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

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