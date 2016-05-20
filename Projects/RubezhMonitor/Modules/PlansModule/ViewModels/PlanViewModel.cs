using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common.TreeList;
using Infrastructure.Plans;
using Infrastructure.Plans.Events;
using Common;
using RubezhAPI;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeNodeViewModel<PlanViewModel>
	{
		PlansViewModel _plansViewModel;
		public Plan Plan { get; private set; }
		public PlanFolder PlanFolder { get; private set; }

		public PlanViewModel(PlansViewModel plansViewModel, Plan plan)
		{
			_selfNamedStateClass = new NamedStateClass();
			_namedStateClass = new NamedStateClass();
			_plansViewModel = plansViewModel;
			Plan = plan;
			PlanFolder = plan as PlanFolder;
		}

		NamedStateClass _namedStateClass;
		public NamedStateClass NamedStateClass
		{
			get { return _namedStateClass; }
			set
			{
				_namedStateClass = value;
				ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(Plan.UID);
				OnPropertyChanged(() => NamedStateClass);
			}
		}

		NamedStateClass _selfNamedStateClass;
		public NamedStateClass SelfNamedStateClass
		{
			get { return _selfNamedStateClass; }
			set
			{
				_selfNamedStateClass = value;
				OnPropertyChanged(() => SelfNamedStateClass);
				UpdateState();
			}
		}

		void UpdateState()
		{
			NamedStateClass = SelfNamedStateClass;
			foreach (var child in Children)
			{
				if (child.NamedStateClass.StateClass < NamedStateClass.StateClass)
					NamedStateClass = child.NamedStateClass;
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
			var minNamedStateClass = new NamedStateClass();
			foreach (var planPresenter in _plansViewModel.PlanPresenters)
			{
				var namedStateClass = planPresenter.GetNamedStateClass(Plan);
				if (namedStateClass.StateClass < minNamedStateClass.StateClass)
				{
					minNamedStateClass = namedStateClass;
				}
			}
			if (minNamedStateClass.StateClass == XStateClass.No || minNamedStateClass.StateClass == XStateClass.Off)
			{
				minNamedStateClass.StateClass = XStateClass.Norm;
				minNamedStateClass.Name = "Норма";
			}
			SelfNamedStateClass = minNamedStateClass;
		}

		//public bool IsStateImage
		//{
		//    get
		//    {
		//        return SelfNamedStateClass != null && (SelfNamedStateClass.StateClass == XStateClass.Fire1 || SelfNamedStateClass.StateClass == XStateClass.Fire2) &&
		//               (devices.Count > 0 &&
		//                !devices.Any(
		//                    x => !x.Driver.IsAm && (x.State.StateClass == XStateClass.Fire1 || x.State.StateClass == XStateClass.Fire2)));
		//    }
		//}

		public bool IsFolder
		{
			get { return PlanFolder != null; }
		}
	}
}