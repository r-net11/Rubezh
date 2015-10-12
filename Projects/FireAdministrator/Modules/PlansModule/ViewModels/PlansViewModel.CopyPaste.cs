using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using PlansModule.Designer;
using Infrastructure.Common.Services;

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
			using (new WaitWrapper())
				_planBuffer = Utils.Clone(SelectedPlan.Plan);
		}
		public RelayCommand PlanCutCommand { get; private set; }
		private void OnPlanCut()
		{
			using (new WaitWrapper())
			{
				_planBuffer = SelectedPlan.Plan;
				OnPlanRemove(true);
			}
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
			using (new WaitWrapper())
			{
				var planViewModel = AddPlan(plan, isRoot ? null : SelectedPlan);
				if (isRoot)
					ClientManager.PlansConfiguration.Plans.Add(plan);
				else
				{
					SelectedPlan.Plan.Children.Add(plan);
					SelectedPlan.Update();
				}
				planViewModel.ExpandChildren();
				SelectedPlan = planViewModel;
				ClientManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
				ServiceFactoryBase.Events.GetEvent<PlansConfigurationChangedEvent>().Publish(null);
			}
		}
		private void OnPlanRemove(bool withChild)
		{
			using (new WaitWrapper())
			{
				var selectedPlan = SelectedPlan;
				var parent = selectedPlan.Parent;
				var plan = SelectedPlan.Plan;
				var index = Plans.IndexOf(selectedPlan);
				var oldIndex = selectedPlan.Index;
				
				DesignerCanvas.IsLocked = true;
				DesignerCanvas.RemoveAll();
				if (parent == null)
				{
					Plans.Remove(selectedPlan);
					ClientManager.PlansConfiguration.Plans.Remove(plan);
					if (!withChild)
						foreach (var childPlanViewModel in selectedPlan.Children.ToArray())
						{
							Plans.Add(childPlanViewModel);
							ClientManager.PlansConfiguration.Plans.Add(childPlanViewModel.Plan);
							childPlanViewModel.Plan.Parent = null;
						}
					index = Math.Min(index, Plans.Count - 1);
					if (index > -1)
						SelectedPlan = Plans[index];
				}
				else
				{
					parent.RemoveChild(selectedPlan);
					parent.Plan.Children.Remove(plan);
					if (!withChild)
					{
						foreach (var childPlanViewModel in selectedPlan.Children.ToArray())
						{
							parent.AddChild(childPlanViewModel);
							parent.Plan.Children.Add(childPlanViewModel.Plan);
							childPlanViewModel.Plan.Parent = parent.Plan;
						}
					}
					if (parent.ChildrenCount == 0)
					{
						SelectedPlan = parent;
					} 
					else
					{
						if (oldIndex == 0)
						{
						SelectedPlan = parent.Children.ToArray()[oldIndex];
						}
						else SelectedPlan = parent.Children.ToArray()[oldIndex-1];
					}
					parent.Update();
					parent.IsExpanded = true;
				}
				ClientManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
				ServiceFactoryBase.Events.GetEvent<PlansConfigurationChangedEvent>().Publish(null);
				ClearReferences(plan);
				DesignerCanvas.IsLocked = false;
			}
		}
		private void RenewPlan(Plan plan)
		{
			plan.UID = Guid.NewGuid();
			foreach (var child in plan.Children)
				RenewPlan(child);
		}
	}
}