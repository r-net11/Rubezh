using System.Collections.ObjectModel;
using Localization.Plans.ViewModels;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public class SubPlanPropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementSubPlan _elementSubPlan;

		public SubPlanPropertiesViewModel(ElementSubPlan elementSubPlan)
		{
			Title = CommonViewModels.FigurePropertiesReferenceToPlan;
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
		private void AddPlan(Plan planFolder, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(planFolder);
			if (parentPlanViewModel == null)
				Plans.Add(planViewModel);
			else
				parentPlanViewModel.AddChild(planViewModel);
			var plan = planFolder as Plan;
			if (plan != null && plan.UID == _elementSubPlan.PlanUID)
				SelectedPlan = planViewModel;

			foreach (var childPlan in planFolder.Children)
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
				OnPropertyChanged(() => SelectedPlan);
			}
		}

		protected override bool CanSave()
		{
			return base.CanSave() && (SelectedPlan == null || SelectedPlan.PlanFolder == null);
		}
		protected override bool Save()
		{
			Helper.SetSubPlan(_elementSubPlan, SelectedPlan == null ? null : SelectedPlan.Plan);
			return base.Save();
		}
	}
}