using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.TreeList;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Common;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeNodeViewModel<PlanViewModel>
	{
		PlansViewModel _plansViewModel;
		public Plan Plan { get; private set; }
		public PlanFolder PlanFolder { get; private set; }

		public PlanViewModel(PlansViewModel plansViewModel, Plan plan)
		{
			_selfStateClass = new StateTypeName<XStateClass>() { StateType = XStateClass.No, Name = "Нет" };
			_stateClass = new StateTypeName<XStateClass>() { StateType = XStateClass.No, Name = "Нет" };
			_plansViewModel = plansViewModel;
			Plan = plan;
			PlanFolder = plan as PlanFolder;
		}

		StateTypeName<XStateClass> _stateClass;
		public StateTypeName<XStateClass> StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
				ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(Plan.UID);
				OnPropertyChanged(() => StateClass);
			}
		}

		StateTypeName<XStateClass> _selfStateClass;
		public StateTypeName<XStateClass> SelfStateClass
		{
			get { return _selfStateClass; }
			set
			{
				_selfStateClass = value;
				OnPropertyChanged(() => SelfStateClass);
				UpdateState();
			}
		}

		void UpdateState()
		{
			StateClass = SelfStateClass;
			foreach (var child in Children)
			{
				if (child.StateClass.StateType < StateClass.StateType)
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
		void StateChanged()
		{
			var minStateTypeName = new StateTypeName<XStateClass>() { StateType = XStateClass.No, Name = "Нет" };
			foreach (var planPresenter in _plansViewModel.PlanPresenters)
			{
				var presenterState = (StateTypeName<XStateClass>)planPresenter.GetStateTypeName(Plan);
				if (presenterState.StateType < minStateTypeName.StateType)
				{
					minStateTypeName = presenterState;
				}
			}
			if (minStateTypeName.StateType == XStateClass.No || minStateTypeName.StateType == XStateClass.Off)
			{
				minStateTypeName.StateType = XStateClass.Norm;
				minStateTypeName.Name = "Норма";
			}
			SelfStateClass = minStateTypeName;
		}

		public bool IsFolder
		{
			get { return PlanFolder != null; }
		}
	}
}