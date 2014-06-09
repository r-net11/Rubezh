using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using FiresecAPI.SKD;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class StepViewModel : TreeNodeViewModel<StepViewModel>
	{
		public ProcedureStep Step { get; private set; }
		public StepsViewModel StepsViewModel { get; private set; }

		public StepViewModel(StepsViewModel stepsViewModel, ProcedureStep step)
		{
			AddCommand = new RelayCommand(OnAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove);

			StepsViewModel = stepsViewModel;
			Step = step;
		}

		void OnChanged()
		{
			OnPropertyChanged("Name");
		}

		public void Update(ProcedureStep step)
		{
			Step = step;
			OnPropertyChanged(() => Step);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => HasChildren);
		}

		public string Name
		{
			get { return Step.Name; }
		}

		public string Description
		{
			get { return Step.Description; }
		}

		public ProcedureStep AddChildStep()
		{
			var stepTypeSelectationViewModel = new StepTypeSelectationViewModel();
			if (DialogService.ShowModalWindow(stepTypeSelectationViewModel))
			{
				if (stepTypeSelectationViewModel.SelectedStepType != null && !stepTypeSelectationViewModel.SelectedStepType.IsFolder)
				{
					var procedureStep = new ProcedureStep();
					procedureStep.ProcedureStepType = stepTypeSelectationViewModel.SelectedStepType.ProcedureStepType;
					var stepViewModel = new StepViewModel(StepsViewModel, procedureStep);
					this.Step.Children.Add(procedureStep);
					this.AddChild(stepViewModel);
					IsExpanded = true;
					ServiceFactory.SaveService.AutomationChanged = true;
					Update();
					return procedureStep;
				}
			}
			return null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			AddChildStep();
		}

		public RelayCommand AddToParentCommand { get; private set; }
		void OnAddToParent()
		{
			Parent.AddCommand.Execute();
		}
		public bool CanAddToParent()
		{
			return Parent != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var parent = Parent;
			if (parent != null)
			{
				var index = StepsViewModel.SelectedStep.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();

				index = Math.Min(index, parent.ChildrenCount - 1);
				StepsViewModel.AllSteps.Remove(this);
				StepsViewModel.SelectedStep = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
			if (Step.Parent != null)
			{
				Step.Parent.Children.Remove(Step);
			}
			ServiceFactory.SaveService.AutomationChanged = true;
		}
	}
}