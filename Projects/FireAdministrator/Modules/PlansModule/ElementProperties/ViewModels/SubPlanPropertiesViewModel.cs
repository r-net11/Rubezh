using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class SubPlanPropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementSubPlan _elementSubPlan;

		public SubPlanPropertiesViewModel(ElementSubPlan elementSubPlan)
		{
			Title = "Свойства фигуры: Подплан";
			_elementSubPlan = elementSubPlan;
			Initialize();
		}

		private void Initialize()
		{
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
				AddPlan(plan, null);

			for (int i = 0; i < Plans.Count; i++)
			{
				Plans[i].CollapseChildren();
				Plans[i].ExpandChildren();
			}
		}
		private void AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(plan);
			if (parentPlanViewModel == null)
				Plans.Add(planViewModel);
			else
				parentPlanViewModel.Children.Add(planViewModel);
			if (plan.UID == _elementSubPlan.PlanUID)
				SelectedPlan = planViewModel;

			foreach (var childPlan in plan.Children)
				AddPlan(childPlan, planViewModel);
		}

		public ObservableCollection<PlanViewModel> Plans { get; set; }

		private PlanViewModel _selectedPlan;
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