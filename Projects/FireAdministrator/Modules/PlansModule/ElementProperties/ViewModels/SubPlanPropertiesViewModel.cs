using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class SubPlanPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementSubPlan _elementSubPlan;

		public SubPlanPropertiesViewModel(ElementSubPlan elementSubPlan)
		{
			Title = "Свойства фигуры: Подплан";
			_elementSubPlan = elementSubPlan;
			Initialize();
			SelectedPlan = Plans.FirstOrDefault(x => x.Plan.UID == elementSubPlan.PlanUID);
		}

		void Initialize()
		{
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
			{
				AddPlan(plan, null);
			}

			for (int i = 0; i < Plans.Count; i++)
			{
				CollapseChild(Plans[i]);
				ExpandChild(Plans[i]);
			}
		}

		PlanViewModel AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(plan, Plans);
			planViewModel.Parent = parentPlanViewModel;

			var indexOf = Plans.IndexOf(parentPlanViewModel);
			Plans.Insert(indexOf + 1, planViewModel);

			foreach (var childPlan in plan.Children)
			{
				var childPlanViewModel = AddPlan(childPlan, planViewModel);
				planViewModel.Children.Add(childPlanViewModel);
			}

			return planViewModel;
		}

		void CollapseChild(PlanViewModel parentPlanViewModel)
		{
			parentPlanViewModel.IsExpanded = false;
			foreach (var planViewModel in parentPlanViewModel.Children)
			{
				CollapseChild(planViewModel);
			}
		}

		void ExpandChild(PlanViewModel parentPlanViewModel)
		{
			parentPlanViewModel.IsExpanded = true;
			foreach (var planViewModel in parentPlanViewModel.Children)
			{
				ExpandChild(planViewModel);
			}
		}

		public ObservableCollection<PlanViewModel> Plans { get; set; }

		PlanViewModel _selectedPlan;
		public PlanViewModel SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				_selectedPlan = value;
				OnPropertyChanged("SelectedPlan");
			}
		}

		protected override bool Save()
		{
			if (SelectedPlan != null)
				_elementSubPlan.PlanUID = SelectedPlan.Plan.UID;
			return base.Save();
		}
	}
}