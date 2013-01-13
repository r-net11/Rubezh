using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
namespace PlansModule.ViewModels
{
	public class PlanTreeViewModel : BaseViewModel
	{
		public event EventHandler SelectedPlanChanged;
		public List<PlanViewModel> AllPlans { get; private set; }
		private PlansViewModel _plansViewModel;

		public PlanTreeViewModel(PlansViewModel plansViewModel)
		{
			_plansViewModel = plansViewModel;
		}

		public void Initialize()
		{
			AllPlans = new List<PlanViewModel>();
			Plans = new ObservableCollection<PlanViewModel>();
			AddPlans(null, FiresecManager.PlansConfiguration.Plans);
			_plansViewModel.PlanPresenters.ForEach(planPresenter => AddPlanPresenter(planPresenter));
			if (Plans.IsNotNullOrEmpty())
				SelectedPlan = Plans[0];
		}

		private void AddPlans(PlanViewModel parent, List<Plan> plans)
		{
			if (plans != null)
				foreach (var plan in plans)
				{
					var planViewModel = new PlanViewModel(_plansViewModel, plan);
					AllPlans.Add(planViewModel);
					planViewModel.IsExpanded = true;
					if (parent != null)
						parent.Children.Add(planViewModel);
					else
						Plans.Add(planViewModel);
					AddPlans(planViewModel, plan.Children);
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
				OnPropertyChanged("Plans");
			}
		}

		public PlanViewModel FindPlan(Guid planUID)
		{
			return AllPlans.FirstOrDefault(item => item.Plan.UID == planUID);
		}
		public void AddPlanPresenter(IPlanPresenter<Plan> planPresenter)
		{
			AllPlans.ForEach(planViewModel => planViewModel.RegisterPresenter(planPresenter));
		}
	}
}
