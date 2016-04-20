using Common;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrustructure.Plans.Events;
using PlansModule.Designer;
using RubezhAPI.Models;
using RubezhAPI.Plans.Interfaces;
using RubezhClient;
using System;
using System.Linq;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel
	{
		private PlansClipboard clipboard = new PlansClipboard();

		private void InitializeCopyPaste()
		{
			PlanCopyCommand = new RelayCommand(OnPlanCopy, CanPlanCopyCut);
			PlanCutCommand = new RelayCommand(OnPlanCut, CanPlanCopyCut);
			PlanPasteCommand = new RelayCommand(OnPlanPaste, CanPlanPaste);
		}

		public RelayCommand PlanCopyCommand { get; private set; }
		private void OnPlanCopy()
		{
			using (new WaitWrapper())
			{
				this.clipboard.Buffer = Utils.Clone(SelectedPlan.Plan);
				this.clipboard.SourceAction = ClipboardSourceAction.Copy;
			}
		}
		public RelayCommand PlanCutCommand { get; private set; }
		private void OnPlanCut()
		{
			using (new WaitWrapper())
			{
				this.clipboard.Buffer = SelectedPlan.Plan;
				this.clipboard.SourceAction = ClipboardSourceAction.Cut;
				OnPlanRemove(true);
			}
		}
		private bool CanPlanCopyCut()
		{
			return SelectedPlan != null;
		}

		public RelayCommand PlanPasteCommand { get; private set; }
		private void OnPlanPaste()
		{
			var isRoot = SelectedPlan == null;
			var copy = Utils.Clone(this.clipboard.Buffer);
			if (this.clipboard.SourceAction == ClipboardSourceAction.Copy)
			{
				var elements = copy.ElementGKDevices.Cast<IElementReference>()
					.Union(copy.ElementGKDoors)
					.Union(copy.ElementPolygonGKDelays)
					.Union(copy.ElementPolygonGKPumpStations)
					.Union(copy.ElementPolygonGKDirections)
					.Union(copy.ElementPolygonGKGuardZones)
					.Union(copy.ElementPolygonGKMPTs)
					.Union(copy.ElementPolygonGKSKDZones)
					.Union(copy.ElementPolygonGKZones)
					.Union(copy.ElementRectangleGKDelays)
					.Union(copy.ElementRectangleGKPumpStations)
					.Union(copy.ElementRectangleGKDirections)
					.Union(copy.ElementRectangleGKGuardZones)
					.Union(copy.ElementRectangleGKMPTs)
					.Union(copy.ElementRectangleGKSKDZones)
					.Union(copy.ElementRectangleGKZones);
				foreach (var element in elements)
					element.ItemUID = Guid.Empty;
			}
			RenewPlan(copy);
			OnPlanPaste(copy, isRoot);
		}
		private bool CanPlanPaste()
		{
			return (this.clipboard.Buffer != null);
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
						else SelectedPlan = parent.Children.ToArray()[oldIndex - 1];
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