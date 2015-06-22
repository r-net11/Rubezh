using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Events;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class SKDZoneViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public GKSKDZone Zone { get; private set; }

		public SKDZoneViewModel(GKSKDZone zone)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);

			Zone = zone;
			zone.Changed += OnChanged;
			Update();
		}

		void OnChanged()
		{
			OnPropertyChanged(() => Name);
		}

		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(GKSKDZone zone)
		{
			Zone = zone;
			OnPropertyChanged(() => Zone);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}

		public void Update()
		{
			_visualizetionState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}

		public string Name
		{
			get { return Zone.Name; }
		}

		public string Description
		{
			get { return Zone.Description; }
		}

		public bool IsOnPlan
		{
			get { return Zone.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return true; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Zone.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Zone.PlanElementUIDs);
		}

		public bool IsBold { get; set; }
	}
}