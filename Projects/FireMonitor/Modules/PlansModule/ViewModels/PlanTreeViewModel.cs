using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
namespace PlansModule.ViewModels
{
	public class PlanTreeViewModel : BaseViewModel
	{
		public static PlanTreeViewModel Current { get; private set; }
		public event EventHandler SelectedPlanChanged;
		public List<PlanViewModel> AllPlans { get; private set; }
		private PlansViewModel _plansViewModel;
		private List<Guid> _filter;

		public PlanTreeViewModel(PlansViewModel plansViewModel, List<Guid> filter)
		{
			Current = this;
			_filter = filter;
			_plansViewModel = plansViewModel;
		}

		public void Initialize()
		{
			SelectedPlan = null;
			AllPlans = new List<PlanViewModel>();
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
				AddPlan(plan, null);
			if (SelectedPlan != null)
				SelectedPlan.ExpandToThis();
			_plansViewModel.PlanPresenters.ForEach(planPresenter => AddPlanPresenter(planPresenter));
		}
		private void AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			if (_filter == null || _filter.Contains(plan.UID))
			{
				var planViewModel = new PlanViewModel(_plansViewModel, plan);
				planViewModel.IsExpanded = true;
				AllPlans.Add(planViewModel);
				if (parentPlanViewModel == null)
					Plans.Add(planViewModel);
				else
					parentPlanViewModel.AddChild(planViewModel);
				if (SelectedPlan == null && !planViewModel.IsFolder)
					SelectedPlan = planViewModel;

				foreach (var childPlan in plan.Children)
					AddPlan(childPlan, planViewModel);
			}
		}

		PlanViewModel _selectedPlan;
		public PlanViewModel SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				if (SelectedPlan != value)
				{
					_selectedPlan = value;
					OnPropertyChanged(() => SelectedPlan);
					if (SelectedPlanChanged != null)
						SelectedPlanChanged(this, EventArgs.Empty);
				}
			}
		}

		ObservableCollection<PlanViewModel> _plans;
		public ObservableCollection<PlanViewModel> Plans
		{
			get { return _plans; }
			private set
			{
				_plans = value;
				OnPropertyChanged(() => Plans);
			}
		}

		public PlanViewModel FindPlan(Guid planUID)
		{
			return AllPlans.FirstOrDefault(item => item.Plan != null && item.Plan.UID == planUID);
		}
		public void AddPlanPresenter(IPlanPresenter<Plan, XStateClass> planPresenter)
		{
			AllPlans.ForEach(planViewModel => planViewModel.RegisterPresenter(planPresenter));
		}
		public void Select()
		{
			if (SelectedPlan == null && Plans.IsNotNullOrEmpty())
				SelectedPlan = Plans[0];
		}
	}
}