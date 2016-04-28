using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public GKZone Zone { get; private set; }

		public ZoneViewModel(GKZone zone)
		{
			Zone = zone;
			zone.PlanElementUIDsChanged += Update;
			Update();
		}

		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
		}
		public bool IsOnPlan
		{
			get { return Zone.PlanElementUIDs != null && Zone.PlanElementUIDs.Count > 0; }
		}

		public void Update()
		{
			OnPropertyChanged(() => Zone);
			if (Zone.PlanElementUIDs == null)
				return;
			_visualizationState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}
	}
}