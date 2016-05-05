using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace AutomationModule.ViewModels
{
	public class StepsViewModel : MenuViewPartViewModel, ISelectable<Guid>
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
			DownIntoCommand = new RelayCommand(OnDownInto, CanDownInto);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			CutCommand = new RelayCommand(OnCut, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			Procedure = procedure;
			BuildTree();
			foreach (var step in AllSteps)
			{
				step.ExpandToThis();
			}
			SelectedStep = AllSteps.FirstOrDefault();
			OnPropertyChanged(() => RootSteps);
			OnPropertyChanged(() => SelectedStep);
		}

		#region Tree
		public List<StepViewModel> AllSteps
		{
			get
			{
				var allSteps = new List<StepViewModel>();
				foreach (var rootStep in RootSteps)
				{
					AddChildPlainSteps(rootStep, allSteps);
				}
				return allSteps;
			}
		}

		void AddChildPlainSteps(StepViewModel parentViewModel, List<StepViewModel> allSteps)
		{
			allSteps.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainSteps(childViewModel, allSteps);
		}

		public void Select(Guid stepUID)
		{
			if (stepUID != Guid.Empty)
			{
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
				OnPropertyChanged(() => RootSteps);
			}
		}

		public void UpdateContent()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			foreach (var step in AllSteps)
			{
				step.UpdateContent();
			}
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		StepViewModel _selectedStep;
		public StepViewModel SelectedStep
		{
			get { return _selectedStep; }
			set
			{
				var automationChanged = ServiceFactory.SaveService.AutomationChanged;
				_selectedStep = value;
				if (_selectedStep != null)
					_selectedStep.UpdateContent();
				ServiceFactory.SaveService.AutomationChanged = automationChanged;
				OnPropertyChanged(() => SelectedStep);
			}
		}
#region TEST

		bool TestValidate()
		{
			int i = 0;
			foreach (var step in AllSteps)
			{
				if (AllProcedureSteps[i] != step.Step)
					return false;
				i++;
			}
			return true;
		}

		public List<ProcedureStep> AllProcedureSteps
		{
			get
			{
				var allSteps = new List<ProcedureStep>();
				foreach (var rootStep in Procedure.Steps)
				{
					AddChildPlainProcedureSteps(rootStep, allSteps);
				}
				return allSteps;
			}
		}

		void AddChildPlainProcedureSteps(ProcedureStep parentViewModel, List<ProcedureStep> allSteps)
		{
			allSteps.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainProcedureSteps(childViewModel, allSteps);
		}
#endregion

		void BuildTree()
		{
			RootSteps = new ObservableCollection<StepViewModel>();
			foreach (var step in Procedure.Steps)
			{
				var stepViewModel = AddStepInternal(step, null);
				RootSteps.Add(stepViewModel);
			}
		}

		public StepViewModel AddStep(ProcedureStep step, StepViewModel parentStepViewModel)
		{
			var stepViewModel = AddStepInternal(step, parentStepViewModel);
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

		ProcedureStep _stepToCopy;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_stepToCopy = Utils.Clone(SelectedStep.Step);
		}

		public RelayCommand CutCommand { get; private set; }
		void OnCut()
		{
			OnCopy();
			OnDelete();
		}

		bool CanCopy()
		{
			return SelectedStep != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			_stepToCopy.UID = new Guid();
			var stepViewModel = new StepViewModel(this, Utils.Clone(_stepToCopy), Procedure);
			Add(stepViewModel);
			foreach (var childStep in stepViewModel.Step.Children)
			{
				PasteRecursively(childStep, stepViewModel);
			}
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		bool CanPaste()
		{
			return _stepToCopy != null;
		}

		void PasteRecursively(ProcedureStep step, StepViewModel parentStepViewModel = null)
		{
			var stepViewModel = new StepViewModel(this, step, Procedure);
			if (parentStepViewModel == null)
			{
				RootSteps.Add(stepViewModel);
			}
			else
			{
				parentStepViewModel.AddChild(stepViewModel);
			}
			foreach (var childStep in step.Children)
			{
				PasteRecursively(childStep, stepViewModel);
			}
		}

		void Add(StepViewModel stepViewModel)
		{
			if (SelectedStep == null || SelectedStep.Parent == null)
			{
				if (SelectedStep == null)
				{
					Procedure.Steps.Add(stepViewModel.Step);
					RootSteps.Add(stepViewModel);
				}
				else
				{
					Procedure.Steps.Insert(RootSteps.IndexOf(SelectedStep) + 1, stepViewModel.Step);
					RootSteps.Insert(RootSteps.IndexOf(SelectedStep) + 1, stepViewModel);
				}
			}
			else
			{
				if (SelectedStep.Step.ProcedureStepType == ProcedureStepType.IfNo || SelectedStep.Step.ProcedureStepType == ProcedureStepType.IfYes || SelectedStep.Step.ProcedureStepType == ProcedureStepType.ForeachBody)
				{
					SelectedStep.Step.Children.Insert(0, stepViewModel.Step);
					SelectedStep.AddChildFirst(stepViewModel); ;
				}
				else if (SelectedStep.Parent.Step.ProcedureStepType == ProcedureStepType.IfYes || SelectedStep.Parent.Step.ProcedureStepType == ProcedureStepType.IfNo ||
					SelectedStep.Parent.Step.ProcedureStepType == ProcedureStepType.ForeachBody)
				{
					SelectedStep.Parent.Step.Children.Insert(SelectedStep.Parent.Step.Children.IndexOf(SelectedStep.Step) + 1, stepViewModel.Step);
					SelectedStep.Parent[SelectedStep.Index].InsertChild(stepViewModel);
				}
			}
			SelectedStep = stepViewModel;
			SelectedStep.ExpandToThis();
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand AddStepCommand { get; private set; }
		void OnAddStep()
		{
#if DEBUG
			if (!TestValidate())
				MessageBoxService.ShowError("Debug Warning: StepViewModels/Steps Discrepancy");
#endif
			var stepTypeSelectationViewModel = new StepTypeSelectationViewModel();
			if (DialogService.ShowModalWindow(stepTypeSelectationViewModel))
			{
				if (stepTypeSelectationViewModel.SelectedStepType != null && !stepTypeSelectationViewModel.SelectedStepType.IsFolder)
				{
					var procedureStep = new ProcedureStep();
					procedureStep.ProcedureStepType = stepTypeSelectationViewModel.SelectedStepType.ProcedureStepType;
					var stepViewModel = new StepViewModel(this, procedureStep, Procedure);
					if (procedureStep.ProcedureStepType == ProcedureStepType.For || procedureStep.ProcedureStepType == ProcedureStepType.While)
					{
						stepViewModel.IsExpanded = true;
						AddСycleBody(stepViewModel);
					}
					Add(stepViewModel);
					ServiceFactory.SaveService.AutomationChanged = true;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (SelectedStep.Parent == null)
			{
				var index = RootSteps.IndexOf(SelectedStep) - 1;
				if (RootSteps.Count > index && index >= 0 && (RootSteps[index].Step.ProcedureStepType == ProcedureStepType.IfNo || RootSteps[index].Step.ProcedureStepType == ProcedureStepType.IfYes || RootSteps[index].Step.ProcedureStepType == ProcedureStepType.ForeachBody))
				{
					index = index - 1;
				}
				Procedure.Steps.Remove(SelectedStep.Step);
				RootSteps.Remove(SelectedStep);
				if (RootSteps.Count > index && index >= 0)
					SelectedStep = RootSteps[index];
				else
					SelectedStep = RootSteps.FirstOrDefault();
			}
			else
			{
				var index = SelectedStep.Index - 1;
				var parent = SelectedStep.Parent;
				parent.Step.Children.Remove(SelectedStep.Step);
				parent.RemoveChild(SelectedStep);
				SelectedStep = parent.Children.FirstOrDefault(x => x.Index == index);
				if (SelectedStep == null)
					SelectedStep = parent.Parent;
			}
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

			var procedureStepIfYes = new ProcedureStep();
			procedureStepIfYes.ProcedureStepType = ProcedureStepType.IfYes;
			procedureStep.Children.Add(procedureStepIfYes);
			var stepIfYesViewModel = new StepViewModel(this, procedureStepIfYes, Procedure);
			stepViewModel.AddChild(stepIfYesViewModel);

			var procedureStepIfNo = new ProcedureStep();
			procedureStepIfNo.ProcedureStepType = ProcedureStepType.IfNo;
			procedureStep.Children.Add(procedureStepIfNo);
			var stepIfNoViewModel = new StepViewModel(this, procedureStepIfNo, Procedure);
			stepViewModel.AddChild(stepIfNoViewModel);

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
			AddСycleBody(stepViewModel);
			Add(stepViewModel);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		void AddСycleBody(StepViewModel stepViewModel)
		{
			var procedureStepForeachBody = new ProcedureStep();
			procedureStepForeachBody.ProcedureStepType = ProcedureStepType.ForeachBody;
			stepViewModel.Step.Children.Add(procedureStepForeachBody);
			var stepForeachBodyViewModel = new StepViewModel(this, procedureStepForeachBody, Procedure);
			stepViewModel.AddChild(stepForeachBodyViewModel);
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
				return true;
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
				return true;
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
				var parentIndex = parentViewModel.Parent.Index;
				parentViewModel.RemoveChild(SelectedStep);
				parentViewModel.Step.Children.Remove(stepViewModel.Step);
				if (delta == 1)
				{
					if (parentViewModel.ChildrenCount <= (index + delta - 1))
					{
						if (parentViewModel.Step.ProcedureStepType == ProcedureStepType.IfYes)
						{
							var targetStepViewModel = parentViewModel.Parent.Children.LastOrDefault();
							if (targetStepViewModel.ChildrenCount == 0)
							{
								targetStepViewModel.AddChild(stepViewModel);
								stepViewModel.ExpandToThis();
							}
							else
								targetStepViewModel[0].InsertTo(stepViewModel);
							targetStepViewModel.Step.Children.Insert(0, stepViewModel.Step);
						}
						else if ((parentViewModel.Parent == null) || ((parentViewModel.Parent.Parent == null)))
						{
							RootSteps.Insert(parentIndex + delta, stepViewModel);
							Procedure.Steps.Insert(parentIndex + delta, stepViewModel.Step);
						}
						else
						{
							parentViewModel.Parent.Parent[parentIndex + delta - 1].InsertChild(stepViewModel);
							parentViewModel.Parent.Parent.Step.Children.Insert(parentIndex + delta - 1, stepViewModel.Step);
						}
					}
					else
					{
						parentViewModel[index + delta - 1].InsertChild(stepViewModel);
						parentViewModel.Step.Children.Insert(index + delta - 1, stepViewModel.Step);
					}
				}
				else
				{
					if (index == 0)
					{
						if (parentViewModel.Step.ProcedureStepType == ProcedureStepType.IfNo)
						{
							var targetStepViewModel = parentViewModel.Parent.Children.FirstOrDefault();
							targetStepViewModel.Step.Children.Insert(targetStepViewModel.ChildrenCount, stepViewModel.Step);
							targetStepViewModel.AddChild(stepViewModel);
						}
						else if ((parentViewModel.Parent == null) || ((parentViewModel.Parent.Parent == null)))
						{
							RootSteps.Insert(parentIndex + delta + 1, stepViewModel);
							Procedure.Steps.Insert(parentIndex + delta + 1, stepViewModel.Step);
						}
						else
						{
							parentViewModel.Parent.Parent[parentIndex + delta + 1].InsertTo(stepViewModel);
							parentViewModel.Parent.Parent.Step.Children.Insert(parentIndex + delta + 1, stepViewModel.Step);
						}
					}
					else
					{
						parentViewModel[index + delta].InsertTo(stepViewModel);
						parentViewModel.Step.Children.Insert(index + delta, stepViewModel.Step);
					}
				}
				SelectedStep = stepViewModel;
			}
		}

		public RelayCommand DownIntoCommand { get; private set; }
		void OnDownInto()
		{
			var stepViewModel = SelectedStep;
			StepViewModel targetStepViewModel;

			if (SelectedStep.Parent == null)
			{
				targetStepViewModel = RootSteps[SelectedStep.Index + 1].Children.FirstOrDefault();
				RootSteps.Remove(SelectedStep);
				Procedure.Steps.Remove(stepViewModel.Step);
			}
			else
			{
				targetStepViewModel = SelectedStep.Parent[SelectedStep.Index + 1].Children.FirstOrDefault();
				SelectedStep.Parent.Step.Children.Remove(SelectedStep.Step);
				SelectedStep.Parent.RemoveChild(SelectedStep);
			}

			if (targetStepViewModel.ChildrenCount == 0)
			{
				targetStepViewModel.AddChild(stepViewModel);
				stepViewModel.ExpandToThis();
			}
			else
				targetStepViewModel[0].InsertTo(stepViewModel);
			targetStepViewModel.Step.Children.Insert(0, stepViewModel.Step);
			SelectedStep = stepViewModel;
		}

		bool CanDownInto()
		{
			if (SelectedStep == null)
				return false;
			var nextStep = new ProcedureStep();
			nextStep.ProcedureStepType = ProcedureStepType.Pause;
			if (SelectedStep.Parent == null)
			{
				if (RootSteps.Count <= SelectedStep.Index + 1)
					return false;
				nextStep = RootSteps[SelectedStep.Index + 1].Step;
			}
			else
			{
				if (SelectedStep.Parent.ChildrenCount > SelectedStep.Index + 1)
					nextStep = SelectedStep.Parent[SelectedStep.Index + 1].Step;
			}

			return (CanDown() && (nextStep.ProcedureStepType == ProcedureStepType.If || nextStep.ProcedureStepType == ProcedureStepType.IfNo || nextStep.ProcedureStepType == ProcedureStepType.Foreach
				|| nextStep.ProcedureStepType == ProcedureStepType.For || nextStep.ProcedureStepType == ProcedureStepType.While));
		}
	}
}