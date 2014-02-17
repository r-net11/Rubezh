using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.TreeList;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using XFiresecAPI;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeNodeViewModel<PlanViewModel>
	{
		private PlansViewModel _plansViewModel;
		public Plan Plan { get; private set; }
		public PlanFolder PlanFolder { get; private set; }

		public PlanViewModel(PlansViewModel plansViewModel, Plan plan)
		{
			_selfStateClass = XStateClass.No;
			_stateClass = XStateClass.No;
			_plansViewModel = plansViewModel;
			Plan = plan;
			PlanFolder = plan as PlanFolder;
		}

		private XStateClass _stateClass;
		public XStateClass StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
				ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(Plan.UID);
				OnPropertyChanged("StateClass");
			}
		}

		private XStateClass _selfStateClass;
		public XStateClass SelfStateClass
		{
			get { return _selfStateClass; }
			set
			{
				_selfStateClass = value;
				OnPropertyChanged(() => SelfStateClass);
				UpdateState();
			}
		}

		private void UpdateState()
		{
			StateClass = SelfStateClass;
			foreach (var child in Children)
			{
				if (child.StateClass < StateClass)
					StateClass = child.StateClass;
			}
			if (Parent != null)
				Parent.UpdateState();
		}

		public void RegisterPresenter(IPlanPresenter<Plan, XStateClass> planPresenter)
		{
			planPresenter.SubscribeStateChanged(Plan, StateChanged);
			StateChanged();
		}
		private void StateChanged()
		{
			var state = XStateClass.No;
			foreach (var planPresenter in _plansViewModel.PlanPresenters)
			{
				var presenterState = (XStateClass)planPresenter.GetState(Plan);
				if (presenterState < state)
					state = presenterState;
			}
			SelfStateClass = state;
		}

		public bool IsFolder
		{
			get { return PlanFolder != null; }
		}
	}
}