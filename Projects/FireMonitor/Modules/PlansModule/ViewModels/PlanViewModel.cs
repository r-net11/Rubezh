using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeItemViewModel<PlanViewModel>
	{
		private PlansViewModel _plansViewModel;
		public Plan Plan { get; private set; }
		public PlanFolder PlanFolder { get; private set; }

		public PlanViewModel(PlansViewModel plansViewModel, PlanFolder planFolder)
		{
			_selfState = StateType.No;
			_plansViewModel = plansViewModel;
			PlanFolder = planFolder;
			Plan = planFolder as Plan;
		}

		private StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				if (Plan != null)
					ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(Plan.UID);
				OnPropertyChanged("StateType");
			}
		}

		private StateType _selfState;
		public StateType SelfState
		{
			get { return _selfState; }
			set
			{
				_selfState = value;
				OnPropertyChanged(() => SelfState);
				UpdateState();
			}
		}

		private void UpdateState()
		{
			StateType = SelfState;
			foreach (var child in Children)
			{
				if (child.StateType < StateType)
					StateType = child.StateType;
			}
			if (Parent != null)
				Parent.UpdateState();
		}

		public void RegisterPresenter(IPlanPresenter<Plan> planPresenter)
		{
			if (Plan != null)
				planPresenter.SubscribeStateChanged(Plan, StateChanged);
			StateChanged();
		}
		private void StateChanged()
		{
			var state = StateType.No;
			if (Plan != null)
				foreach (var planPresenter in _plansViewModel.PlanPresenters)
				{
					var presenterState = (StateType)planPresenter.GetState(Plan);
					if (presenterState < state)
						state = presenterState;
				}
			SelfState = state;
		}
	}
}