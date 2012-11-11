using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using System;
namespace PlansModule.ViewModels
{
	public class PlanTreeViewModel : BaseViewModel
	{
		public event EventHandler SelectedPlanChanged;

		public PlanTreeViewModel()
		{
			Plans = new ObservableCollection<PlanViewModel>();
			AddPlans(null, FiresecManager.PlansConfiguration.Plans);
			if (Plans.IsNotNullOrEmpty())
				SelectedPlan = Plans[0];
		}

		private void AddPlans(PlanViewModel parent, List<Plan> plans)
		{
			if (plans != null)
				foreach (var plan in plans)
				{
					var planViewModel = new PlanViewModel(plan, Plans)
					{
						Parent = parent,
						IsExpanded = true
					};
					if (parent != null)
						parent.Children.Add(planViewModel);
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
				_selectedPlan = value;
				OnPropertyChanged("SelectedPlan");
				if (SelectedPlanChanged != null)
					SelectedPlanChanged(this, EventArgs.Empty);
			}
		}

		ObservableCollection<PlanViewModel> _plans;
		public ObservableCollection<PlanViewModel> Plans
		{
			get { return _plans; }
			set
			{
				_plans = value;
				OnPropertyChanged("Plans");
			}
		}
	}
}
