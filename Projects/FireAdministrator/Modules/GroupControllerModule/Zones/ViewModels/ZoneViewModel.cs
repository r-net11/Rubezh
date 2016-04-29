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
			zone.PlanElementUIDsChanged += UpdateVisualizationState;
			Update();
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
		public void Update()
		{
			OnPropertyChanged(() => Zone);
			UpdateVisualizationState();
		}
		void UpdateVisualizationState()
		{
			VisualizationState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
		}
	}
}