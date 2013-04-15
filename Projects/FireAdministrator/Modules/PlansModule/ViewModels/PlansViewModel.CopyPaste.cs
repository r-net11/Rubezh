using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using PlansModule.Designer;
using FiresecAPI.Models;
using FiresecClient;
using System.Runtime.Serialization;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel
	{
		private List<ElementBase> _buffer;
		private Plan _planBuffer;

		private void InitializeCopyPaste()
		{
			CopyCommand = new RelayCommand(OnCopy, CanCopyCut);
			CutCommand = new RelayCommand(OnCut, CanCopyCut);
			PasteCommand = new RelayCommand<IInputElement>(OnPaste, CanPaste);
			_buffer = new List<ElementBase>();
			PlanCopyCommand = new RelayCommand(OnPlanCopy, CanPlanCopyCut);
			PlanCutCommand = new RelayCommand(OnPlanCut, CanPlanCopyCut);
			PlanPasteCommand = new RelayCommand<bool>(OnPlanPaste, CanPlanPaste);
			_planBuffer = null;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			using (new WaitWrapper())
			{
				PlanDesignerViewModel.Save();
				_buffer = new List<ElementBase>();
				foreach (var designerItem in DesignerCanvas.SelectedItems)
				{
					designerItem.UpdateElementProperties();
					_buffer.Add(designerItem.Element.Clone());
				}
			}
		}
		public RelayCommand CutCommand { get; private set; }
		private void OnCut()
		{
			using (new WaitWrapper())
			{
				OnCopy();
				DesignerCanvas.RemoveAllSelected();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		private bool CanCopyCut(object obj)
		{
			return DesignerCanvas.SelectedItems.Count() > 0;
		}

		public RelayCommand<IInputElement> PasteCommand { get; private set; }
		private void OnPaste(IInputElement container)
		{
			using (new WaitWrapper())
			using (new TimeCounter("Command.Paste: {0}"))
				if (NormalizeBuffer(container))
				{
					var designerItems = new List<DesignerItem>();
					DesignerCanvas.Toolbox.SetDefault();
					DesignerCanvas.DeselectAll();
					foreach (var elementBase in _buffer)
					{
						var element = elementBase.Clone();
						element.UID = Guid.NewGuid();
						var designerItem = DesignerCanvas.AddElement(element);
						designerItems.Add(designerItem);
						designerItem.IsSelected = true;
					}
					PlanDesignerViewModel.MoveToFrontCommand.Execute();
					ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(DesignerCanvas.SelectedElements.ToList());
					ServiceFactory.SaveService.PlansChanged = true;
				}
		}
		private bool CanPaste(IInputElement obj)
		{
			return _buffer.Count > 0;
		}

		private bool NormalizeBuffer(IInputElement container)
		{
			if (_buffer.Count > 0)
			{
				Point? point = container == null ? null : (Point?)Mouse.GetPosition(container);
				double minLeft = double.MaxValue;
				double minTop = double.MaxValue;
				double maxRight = 0;
				double maxBottom = 0;
				foreach (var elementBase in _buffer)
				{
					Rect rect = elementBase.GetRectangle();
					if (minLeft > rect.Left)
						minLeft = rect.Left;
					if (minTop > rect.Top)
						minTop = rect.Top;
					if (maxBottom < rect.Top + rect.Height)
						maxBottom = rect.Top + rect.Height;
					if (maxRight < rect.Left + rect.Width)
						maxRight = rect.Left + rect.Width;
				}
				Rect border = new Rect(minLeft, minTop, maxRight - minLeft, maxBottom - minTop);
				if (border.Width > PlanDesignerViewModel.Plan.Width || border.Height > PlanDesignerViewModel.Plan.Height)
				{
					MessageBoxService.Show("Размер вставляемого содержимого больше размеров плана");
					return false;
				}
				if (border.X < 0)
					border.X = 0;
				if (border.Y < 0)
					border.Y = 0;
				if (border.X + border.Width > PlanDesignerViewModel.Plan.Width)
					border.X = PlanDesignerViewModel.Plan.Width - border.Width;
				if (border.Y + border.Height > PlanDesignerViewModel.Plan.Height)
					border.Y = PlanDesignerViewModel.Plan.Height - border.Height;
				if (point.HasValue)
				{
					border.X = point.Value.X + border.Width <= PlanDesignerViewModel.Plan.Width ? point.Value.X : PlanDesignerViewModel.Plan.Width - border.Width;
					border.Y = point.Value.Y + border.Height <= PlanDesignerViewModel.Plan.Height ? point.Value.Y : PlanDesignerViewModel.Plan.Height - border.Height;
				}
				Vector shift = new Vector(border.X - minLeft, border.Y - minTop);
				//if (shift.X == 0 && shift.Y == 0)
				//    shift = new Vector(-minLeft / 2, -minTop / 2);
				foreach (var elementBase in _buffer)
					elementBase.Position += shift;
			}
			return true;
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
			}
		}
		private void OnPlanRemove(bool withChild)
		{
			using (new WaitWrapper())
			{
				var selectedPlan = SelectedPlan;
				var parent = selectedPlan.Parent;
				var plan = SelectedPlan.Plan;
				ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(DesignerCanvas.Items.Select(item => item.Element).ToList());
				if (parent == null)
				{
					Plans.Remove(selectedPlan);
					FiresecManager.PlansConfiguration.Plans.Remove(plan);
					if (!withChild)
						foreach (var childPlanViewModel in selectedPlan.Children)
						{
							Plans.Add(childPlanViewModel);
							FiresecManager.PlansConfiguration.Plans.Add(childPlanViewModel.Plan);
							childPlanViewModel.Plan.Parent = null;
							childPlanViewModel.ResetParent();
						}
				}
				else
				{
					parent.Children.Remove(selectedPlan);
					parent.Plan.Children.Remove(plan);
					if (!withChild)
						foreach (var childPlanViewModel in selectedPlan.Children)
						{
							parent.Children.Add(childPlanViewModel);
							parent.Plan.Children.Add(childPlanViewModel.Plan);
							childPlanViewModel.Plan.Parent = parent.Plan;
						}
					parent.Update();
					parent.IsExpanded = true;
				}
				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
				ClearReferences(plan);
				SelectedPlan = parent == null ? Plans.FirstOrDefault() : parent;
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