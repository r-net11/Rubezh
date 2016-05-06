using Common;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Events;
using PlansModule.Designer;
using System;
using System.Linq;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel
	{
		private Plan _planBuffer;

		private void InitializeCopyPaste()
		{
			PlanCopyCommand = new RelayCommand(OnPlanCopy, CanPlanCopyCut);
			PlanCutCommand = new RelayCommand(OnPlanCut, CanPlanCopyCut);
			PlanPasteCommand = new RelayCommand<bool>(OnPlanPaste, CanPlanPaste);
			_planBuffer = null;
		}

		public RelayCommand PlanCopyCommand { get; private set; }
		private void OnPlanCopy()
		{
			_planBuffer = Utils.Clone(SelectedPlan.Plan);
		}
		public RelayCommand PlanCutCommand { get; private set; }
		private void OnPlanCut()
		{
			_planBuffer = SelectedPlan.Plan;
			OnPlanRemove(true);
		}
		private bool CanPlanCopyCut()
		{
			return SelectedPlan != null;
		}

		public RelayCommand<bool> PlanPasteCommand { get; private set; }
		private void OnPlanPaste(bool isRoot)
		{
			var copy = Utils.Clone(_planBuffer);
			RenewPlan(copy);
			OnPlanPaste(copy, isRoot);
		}
		private bool CanPlanPaste(bool isRoot)
		{
			return _planBuffer != null && SelectedPlan != null;
		}

		private void OnPlanPaste(Plan plan, bool isRoot)
		{
			var planViewModel = AddPlan(plan, isRoot ? null : SelectedPlan);
			if (isRoot)
				FiresecManager.PlansConfiguration.Plans.Add(plan);
			else
			{
				SelectedPlan.Plan.Children.Add(plan);
				SelectedPlan.Update();
			}
			planViewModel.ExpandChildren();
			SelectedPlan = planViewModel;
			FiresecManager.PlansConfiguration.Update();
			ServiceFactory.SaveService.PlansChanged = true;
			ServiceFactoryBase.Events.GetEvent<PlansConfigurationChangedEvent>().Publish(null);
		}
		private void OnPlanRemove(bool withChild)
		{
				var selectedPlan = SelectedPlan;
				var parent = selectedPlan.Parent;
				var plan = SelectedPlan.Plan;
				DesignerCanvas.IsLocked = true;
				DesignerCanvas.RemoveAll();
				if (parent == null)
				{
					Plans.Remove(selectedPlan);
					FiresecManager.PlansConfiguration.Plans.Remove(plan);
					if (!withChild)
						foreach (var childPlanViewModel in selectedPlan.Children.ToArray())
						{
							Plans.Add(childPlanViewModel);
							FiresecManager.PlansConfiguration.Plans.Add(childPlanViewModel.Plan);
							childPlanViewModel.Plan.Parent = null;
						}
				}
				else
				{
					parent.RemoveChild(selectedPlan);
					parent.Plan.Children.Remove(plan);
					if (!withChild)
						foreach (var childPlanViewModel in selectedPlan.Children.ToArray())
						{
							parent.AddChild(childPlanViewModel);
							parent.Plan.Children.Add(childPlanViewModel.Plan);
							childPlanViewModel.Plan.Parent = parent.Plan;
						}
					parent.Update();
					parent.IsExpanded = true;
				}
				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
				ServiceFactoryBase.Events.GetEvent<PlansConfigurationChangedEvent>().Publish(null);
				ClearReferences(plan);
				DesignerCanvas.IsLocked = false;
				SelectedPlan = parent ?? Plans.FirstOrDefault();
		}
		private void RenewPlan(Plan plan)
		{
			plan.UID = Guid.NewGuid();
			foreach (var child in plan.Children)
				RenewPlan(child);
		}
	}
}