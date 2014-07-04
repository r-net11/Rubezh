using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using FiresecAPI.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class StepsViewModel : BaseViewModel, ISelectable<Guid>
	{
		public Procedure Procedure { get; private set; }

		public StepsViewModel(Procedure procedure)
		{
			AddStepCommand = new RelayCommand(OnAddStep, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDeleted);
			AddIfCommand = new RelayCommand(OnAddIf, CanAdd);
			AddForeachCommand = new RelayCommand(OnAddForeach, CanAdd);
			UpCommand = new RelayCommand(OnUp, CanUp);
			DownCommand = new RelayCommand(OnDown, CanDown);

			Procedure = procedure;

			BuildTree();
			foreach (var step in AllSteps)
			{
				step.ExpandToThis();
			}
			SelectedStep = AllSteps.FirstOrDefault();
			OnPropertyChanged("RootSteps");
			OnPropertyChanged("SelectedStep");
		}

		#region Tree
		public List<StepViewModel> AllSteps;

		public void FillAllSteps()
		{
			AllSteps = new List<StepViewModel>();
			foreach (var rootStep in RootSteps)
			{
				AddChildPlainSteps(rootStep);
			}
		}

		void AddChildPlainSteps(StepViewModel parentViewModel)
		{
			AllSteps.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainSteps(childViewModel);
		}

		public void Select(Guid stepUID)
		{
			if (stepUID != Guid.Empty)
			{
				FillAllSteps();
				var stepViewModel = AllSteps.FirstOrDefault(x => x.Step.UID == stepUID);
				if (stepViewModel != null)
					stepViewModel.ExpandToThis();
				SelectedStep = stepViewModel;
			}
		}

		ObservableCollection<StepViewModel> _rootSteps;
		public ObservableCollection<StepViewModel> RootSteps
		{
			get { return _rootSteps; }
			private set
			{
				_rootSteps = value;
				OnPropertyChanged("RootSteps");
			}
		}

		public void UpdateContent()
		{
			foreach (var step in AllSteps)
			{
				step.UpdateContent();
			}
		}

		StepViewModel _selectedStep;
		public StepViewModel SelectedStep
		{
			get { return _selectedStep; }
			set
			{
				_selectedStep = value;
				if (_selectedStep != null)
					_selectedStep.UpdateContent();
				OnPropertyChanged(() => SelectedStep);
			}
		}

		void BuildTree()
		{
			RootSteps = new ObservableCollection<StepViewModel>();
			foreach (var step in Procedure.Steps)
			{
				var stepViewModel = AddStepInternal(step, null);
				RootSteps.Add(stepViewModel);
			}
			FillAllSteps();
		}

		public StepViewModel AddStep(ProcedureStep step, StepViewModel parentStepViewModel)
		{
			var stepViewModel = AddStepInternal(step, parentStepViewModel);
			FillAllSteps();
			return stepViewModel;
		}
		private StepViewModel AddStepInternal(ProcedureStep step, StepViewModel parentStepViewModel)
		{
			var stepViewModel = new StepViewModel(this, step, Procedure);
			if (parentStepViewModel != null)
				parentStepViewModel.AddChild(stepViewModel);

			foreach (var childStep in step.Children)
				AddStepInternal(childStep, stepViewModel);
			return stepViewModel;
		}
		#endregion

		void Add(StepViewModel stepViewModel)
		{
			if (SelectedStep == null || SelectedStep.Parent == null)
			{
				Procedure.Steps.Add(stepViewModel.Step);
				RootSteps.Add(stepViewModel);
			}
			else if (SelectedStep != null && (SelectedStep.Step.ProcedureStepType == ProcedureStepType.IfNo || SelectedStep.Step.ProcedureStepType == ProcedureStepType.IfYes || SelectedStep.Step.ProcedureStepType == ProcedureStepType.ForeachBody))
			{
				SelectedStep.Step.Children.Add(stepViewModel.Step);
				SelectedStep.AddChild(stepViewModel);
			}
			SelectedStep = stepViewModel;
		}

		bool CanAdd()
		{
			if (SelectedStep == null)
				return true;
			if (SelectedStep.Step.ProcedureStepType == ProcedureStepType.If ||
				SelectedStep.Step.ProcedureStepType == ProcedureStepType.Foreach ||
				SelectedStep.Step.ProcedureStepType == ProcedureStepType.ForeachList ||
				SelectedStep.Step.ProcedureStepType == ProcedureStepType.ForeachElement)
				return false;
			return true;
		}

		public RelayCommand AddStepCommand { get; private set; }
		void OnAddStep()
		{
			var stepTypeSelectationViewModel = new StepTypeSelectationViewModel();
			if (DialogService.ShowModalWindow(stepTypeSelectationViewModel))
			{
				if (stepTypeSelectationViewModel.SelectedStepType != null && !stepTypeSelectationViewModel.SelectedStepType.IsFolder)
				{
					var procedureStep = new ProcedureStep();
					procedureStep.ProcedureStepType = stepTypeSelectationViewModel.SelectedStepType.ProcedureStepType;
					var stepViewModel = new StepViewModel(this, procedureStep, Procedure);
					Add(stepViewModel);
					FillAllSteps();
					ServiceFactory.SaveService.AutomationChanged = true;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			AllSteps.Remove(SelectedStep);
			if (SelectedStep.Parent == null)
			{
				Procedure.Steps.Remove(SelectedStep.Step);
				RootSteps.Remove(SelectedStep);
			}
			else
			{
				SelectedStep.Parent.Step.Children.Remove(SelectedStep.Step);
				SelectedStep.Parent.RemoveChild(SelectedStep);
			}
			SelectedStep = RootSteps.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDeleted()
		{
			return SelectedStep != null && !SelectedStep.IsVirtual;
		}

		public RelayCommand AddIfCommand { get; private set; }
		void OnAddIf()
		{
			var procedureStep = new ProcedureStep();
			procedureStep.ProcedureStepType = ProcedureStepType.If;
			var stepViewModel = new StepViewModel(this, procedureStep, Procedure);
			stepViewModel.IsExpanded = true;
			AllSteps.Add(stepViewModel);

			var procedureStepIfYes = new ProcedureStep();
			procedureStepIfYes.ProcedureStepType = ProcedureStepType.IfYes;
			procedureStep.Children.Add(procedureStepIfYes);
			var stepIfYesViewModel = new StepViewModel(this, procedureStepIfYes, Procedure);
			stepViewModel.AddChild(stepIfYesViewModel);
			AllSteps.Add(stepIfYesViewModel);

			var procedureStepIfNo = new ProcedureStep();
			procedureStepIfNo.ProcedureStepType = ProcedureStepType.IfNo;
			procedureStep.Children.Add(procedureStepIfNo);
			var stepIfNoViewModel = new StepViewModel(this, procedureStepIfNo, Procedure);
			stepViewModel.AddChild(stepIfNoViewModel);
			AllSteps.Add(stepIfNoViewModel);

			Add(stepViewModel);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand AddForeachCommand { get; private set; }
		void OnAddForeach()
		{
			var procedureStep = new ProcedureStep();
			procedureStep.ProcedureStepType = ProcedureStepType.Foreach;
			var stepViewModel = new StepViewModel(this, procedureStep, Procedure);
			stepViewModel.IsExpanded = true;
			AllSteps.Add(stepViewModel);

			var procedureStepForeachBody = new ProcedureStep();
			procedureStepForeachBody.ProcedureStepType = ProcedureStepType.ForeachBody;
			procedureStep.Children.Add(procedureStepForeachBody);
			var stepForeachBodyViewModel = new StepViewModel(this, procedureStepForeachBody, Procedure);
			stepViewModel.AddChild(stepForeachBodyViewModel);
			AllSteps.Add(stepForeachBodyViewModel);

			var procedureStepForeachList = new ProcedureStep();
			procedureStepForeachList.ProcedureStepType = ProcedureStepType.ForeachList;
			procedureStep.Children.Add(procedureStepForeachList);
			var stepForeachListViewModel = new StepViewModel(this, procedureStepForeachList, Procedure);
			stepViewModel.AddChild(stepForeachListViewModel);
			AllSteps.Add(stepForeachListViewModel);

			var procedureStepForeachElement = new ProcedureStep();
			procedureStepForeachElement.ProcedureStepType = ProcedureStepType.ForeachElement;
			procedureStep.Children.Add(procedureStepForeachElement);
			var stepForeachElementViewModel = new StepViewModel(this, procedureStepForeachElement, Procedure);
			stepViewModel.AddChild(stepForeachElementViewModel);
			AllSteps.Add(stepForeachElementViewModel);

			Add(stepViewModel);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand UpCommand { get; private set; }
		void OnUp()
		{
			Mode(-1);
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanUp()
		{
			if (SelectedStep == null)
				return false;
			if (SelectedStep.IsVirtual)
				return false;
			if (SelectedStep.Parent == null)
			{
				var index = RootSteps.IndexOf(SelectedStep);
				if (index < 1)
					return false;
			}
			else
			{
				var index = SelectedStep.Index;
				if (index < 2)
					return false;
			}
			return true;
		}

		public RelayCommand DownCommand { get; private set; }
		void OnDown()
		{
			Mode(+1);
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDown()
		{
			if (SelectedStep == null)
				return false;
			if (SelectedStep.IsVirtual)
				return false;
			if (SelectedStep.Parent == null)
			{
				var index = RootSteps.IndexOf(SelectedStep);
				if (index > RootSteps.Count - 2)
					return false;
			}
			else
			{
				var index = SelectedStep.Index;
				if (index > SelectedStep.Parent.Children.Count() - 2)
					return false;
			}
			return true;
		}

		void Mode(int delta)
		{
			if (SelectedStep.Parent == null)
			{
				var stepViewModel = SelectedStep;
				var index = RootSteps.IndexOf(SelectedStep);
				RootSteps.Remove(SelectedStep);
				RootSteps.Insert(index + delta, stepViewModel);
				SelectedStep = stepViewModel;

				var step = stepViewModel.Step;
				Procedure.Steps.Remove(stepViewModel.Step);
				Procedure.Steps.Insert(index + delta, step);
			}
			else
			{
				var stepViewModel = SelectedStep;
				var parentViewModel = SelectedStep.Parent;
				var index = SelectedStep.Index;
				parentViewModel.RemoveChild(SelectedStep);
				parentViewModel[index + delta - 1].InsertChild(stepViewModel);
				SelectedStep = stepViewModel;

				var step = stepViewModel.Step;
				parentViewModel.Step.Children.Remove(stepViewModel.Step);
				parentViewModel.Step.Children.Insert(index + delta, step);
			}
		}
	}
}