using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using RubezhAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using RubezhClient;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class ProceduresViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static ProceduresViewModel Current { get; private set; }
		public ProceduresViewModel()
		{
			Current = this;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CutCommand = new RelayCommand(OnCut, CanCopy);

			Menu = new ProceduresMenuViewModel(this);
			RegisterShortcuts();
			SetRibbonItems();
			IsRightPanelEnabled = true;
			IsRightPanelVisible = false;
		}

		public void Initialize()
		{
			Procedures = new SortableObservableCollection<ProcedureViewModel>();
			if (ClientManager.SystemConfiguration.AutomationConfiguration.Procedures == null)
				ClientManager.SystemConfiguration.AutomationConfiguration.Procedures = new List<Procedure>();

			foreach (var procedure in ClientManager.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				var procedureViewModel = new ProcedureViewModel(procedure);
				Procedures.Add(procedureViewModel);
			}

			Procedures.Sort(x => x.Name);
			SelectedProcedure = Procedures.FirstOrDefault();
		}

		SortableObservableCollection<ProcedureViewModel> _procedures;
		public SortableObservableCollection<ProcedureViewModel> Procedures
		{
			get { return _procedures; }
			set
			{
				_procedures = value;
				OnPropertyChanged(() => Procedures);
			}
		}

		ProcedureViewModel _selectedProcedure;
		public ProcedureViewModel SelectedProcedure
		{
			get { return _selectedProcedure; }
			set
			{
				_selectedProcedure = value;
				OnPropertyChanged(() => SelectedProcedure);
				if (value != null)
				{
					value.Update();
					value.StepsViewModel.SelectedStep = value.StepsViewModel.RootSteps.FirstOrDefault();
					value.StepsViewModel.UpdateContent();
				}
			}
		}

		Procedure _procedureToCopy;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_procedureToCopy = SelectedProcedure.Procedure;
		}

		bool CanCopy()
		{
			return SelectedProcedure != null;
		}

		public RelayCommand CutCommand { get; private set; }
		private void OnCut()
		{
			OnCopy();
			OnDelete();
		}
		void ReplaceStepUids(ProcedureStep step, Dictionary<Guid, Guid> dictionary)
		{
			step.UID = Guid.NewGuid();
			switch (step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					break;

				case ProcedureStepType.ShowMessage:
					ReplaceVariableUid(step.ShowMessageArguments.MessageArgument, dictionary);
					ReplaceVariableUid(step.ShowMessageArguments.ConfirmationValueArgument, dictionary);
					break;

				case ProcedureStepType.Arithmetics:
					ReplaceVariableUid(step.ArithmeticArguments.Argument1, dictionary);
					ReplaceVariableUid(step.ArithmeticArguments.Argument2, dictionary);
					ReplaceVariableUid(step.ArithmeticArguments.ResultArgument, dictionary);
					break;

				case ProcedureStepType.If:
				case ProcedureStepType.While:
					var conditionArguments = step.ConditionArguments;
					foreach (var condition in conditionArguments.Conditions)
					{
						ReplaceVariableUid(condition.Argument1, dictionary);
						ReplaceVariableUid(condition.Argument2, dictionary);
					}
					break;

				case ProcedureStepType.AddJournalItem:
					ReplaceVariableUid(step.JournalArguments.MessageArgument, dictionary);
					break;

				case ProcedureStepType.FindObjects:
					ReplaceVariableUid(step.FindObjectArguments.ResultArgument, dictionary);
					foreach (var findObjectCondition in step.FindObjectArguments.FindObjectConditions)
						ReplaceVariableUid(findObjectCondition.SourceArgument, dictionary);
					break;

				case ProcedureStepType.Foreach:
					ReplaceVariableUid(step.ForeachArguments.ItemArgument, dictionary);
					ReplaceVariableUid(step.ForeachArguments.ListArgument, dictionary);
					break;

				case ProcedureStepType.For:
					ReplaceVariableUid(step.ForArguments.IndexerArgument, dictionary);
					ReplaceVariableUid(step.ForArguments.InitialValueArgument, dictionary);
					ReplaceVariableUid(step.ForArguments.ValueArgument, dictionary);
					ReplaceVariableUid(step.ForArguments.IteratorArgument, dictionary);
					break;

				case ProcedureStepType.Pause:
					ReplaceVariableUid(step.PauseArguments.PauseArgument, dictionary);
					break;

				case ProcedureStepType.ProcedureSelection:
					foreach (var argument in step.ProcedureSelectionArguments.ScheduleProcedure.Arguments)
						ReplaceVariableUid(argument, dictionary);
					break;

				case ProcedureStepType.Exit:
					ReplaceVariableUid(step.ExitArguments.ExitCodeArgument, dictionary);
					break;

				case ProcedureStepType.SetValue:
					ReplaceVariableUid(step.SetValueArguments.SourceArgument, dictionary);
					ReplaceVariableUid(step.SetValueArguments.TargetArgument, dictionary);
					break;

				case ProcedureStepType.IncrementValue:
					ReplaceVariableUid(step.IncrementValueArguments.ResultArgument, dictionary);
					break;

				case ProcedureStepType.ControlGKDevice:
					ReplaceVariableUid(step.ControlGKDeviceArguments.GKDeviceArgument, dictionary);
					break;

				case ProcedureStepType.ControlGKFireZone:
					ReplaceVariableUid(step.ControlGKFireZoneArguments.GKFireZoneArgument, dictionary);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					ReplaceVariableUid(step.ControlGKGuardZoneArguments.GKGuardZoneArgument, dictionary);
					break;

				case ProcedureStepType.ControlDirection:
					ReplaceVariableUid(step.ControlDirectionArguments.DirectionArgument, dictionary);
					break;

				case ProcedureStepType.ControlGKDoor:
					ReplaceVariableUid(step.ControlGKDoorArguments.DoorArgument, dictionary);
					break;

				case ProcedureStepType.ControlDelay:
					ReplaceVariableUid(step.ControlDelayArguments.DelayArgument, dictionary);
					break;

				case ProcedureStepType.GetObjectProperty:
					ReplaceVariableUid(step.GetObjectPropertyArguments.ObjectArgument, dictionary);
					ReplaceVariableUid(step.GetObjectPropertyArguments.ResultArgument, dictionary);
					break;

				case ProcedureStepType.SendEmail:
					ReplaceVariableUid(step.SendEmailArguments.EMailAddressFromArgument, dictionary);
					ReplaceVariableUid(step.SendEmailArguments.EMailAddressToArgument, dictionary);
					ReplaceVariableUid(step.SendEmailArguments.EMailContentArgument, dictionary);
					ReplaceVariableUid(step.SendEmailArguments.EMailTitleArgument, dictionary);
					ReplaceVariableUid(step.SendEmailArguments.SmtpArgument, dictionary);
					ReplaceVariableUid(step.SendEmailArguments.LoginArgument, dictionary);
					ReplaceVariableUid(step.SendEmailArguments.PasswordArgument, dictionary);
					ReplaceVariableUid(step.SendEmailArguments.PortArgument, dictionary);
					break;

				case ProcedureStepType.RunProgram:
					ReplaceVariableUid(step.RunProgramArguments.ParametersArgument, dictionary);
					ReplaceVariableUid(step.RunProgramArguments.PathArgument, dictionary);
					break;

				case ProcedureStepType.Random:
					ReplaceVariableUid(step.RandomArguments.MaxValueArgument, dictionary);
					break;

				case ProcedureStepType.ChangeList:
					ReplaceVariableUid(step.ChangeListArguments.ItemArgument, dictionary);
					ReplaceVariableUid(step.ChangeListArguments.ListArgument, dictionary);
					break;

				case ProcedureStepType.CheckPermission:
					ReplaceVariableUid(step.CheckPermissionArguments.PermissionArgument, dictionary);
					ReplaceVariableUid(step.CheckPermissionArguments.ResultArgument, dictionary);
					break;

				case ProcedureStepType.GetJournalItem:
					ReplaceVariableUid(step.GetJournalItemArguments.ResultArgument, dictionary);
					break;

				case ProcedureStepType.GetListCount:
					ReplaceVariableUid(step.GetListCountArguments.ListArgument, dictionary);
					ReplaceVariableUid(step.GetListCountArguments.CountArgument, dictionary);
					break;

				case ProcedureStepType.GetListItem:
					ReplaceVariableUid(step.GetListItemArguments.ListArgument, dictionary);
					ReplaceVariableUid(step.GetListItemArguments.ItemArgument, dictionary);
					ReplaceVariableUid(step.GetListItemArguments.IndexArgument, dictionary);
					break;
				case ProcedureStepType.ControlVisualGet:
				case ProcedureStepType.ControlVisualSet:
					ReplaceVariableUid(step.ControlVisualArguments.Argument, dictionary);
					break;
				case ProcedureStepType.ControlPlanGet:
				case ProcedureStepType.ControlPlanSet:
					ReplaceVariableUid(step.ControlPlanArguments.ValueArgument, dictionary);
					break;
				case ProcedureStepType.ShowDialog:
					break;
				case ProcedureStepType.GenerateGuid:
					ReplaceVariableUid(step.GenerateGuidArguments.ResultArgument, dictionary);
					break;
				case ProcedureStepType.SetJournalItemGuid:
					ReplaceVariableUid(step.SetJournalItemGuidArguments.ValueArgument, dictionary);
					break;
				case ProcedureStepType.Ptz:
					ReplaceVariableUid(step.PtzArguments.CameraArgument, dictionary);
					ReplaceVariableUid(step.PtzArguments.PtzNumberArgument, dictionary);
					break;
				case ProcedureStepType.StartRecord:
					ReplaceVariableUid(step.StartRecordArguments.CameraArgument, dictionary);
					ReplaceVariableUid(step.StartRecordArguments.EventUIDArgument, dictionary);
					ReplaceVariableUid(step.StartRecordArguments.TimeoutArgument, dictionary);
					break;
				case ProcedureStepType.StopRecord:
					ReplaceVariableUid(step.StopRecordArguments.CameraArgument, dictionary);
					ReplaceVariableUid(step.StopRecordArguments.EventUIDArgument, dictionary);
					break;
				case ProcedureStepType.RviAlarm:
					ReplaceVariableUid(step.RviAlarmArguments.NameArgument, dictionary);
					break;
				case ProcedureStepType.Now:
					ReplaceVariableUid(step.NowArguments.ResultArgument, dictionary);
					break;
			}
			foreach (var childStep in step.Children)
				ReplaceStepUids(childStep, dictionary);
		}
		void ReplaceVariableUid(Argument argument, Dictionary<Guid, Guid> dictionary)
		{
			if (argument.VariableScope == VariableScope.LocalVariable)
				argument.VariableUid = dictionary.ContainsKey(argument.VariableUid) ? dictionary[argument.VariableUid] : Guid.Empty;
		}
		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var clone = Utils.Clone(_procedureToCopy);
			clone.Uid = Guid.NewGuid();

			var dictionary = new Dictionary<Guid, Guid>();
			foreach (var variable in clone.Variables)
				dictionary.Add(variable.Uid, variable.Uid = Guid.NewGuid());
			foreach (var argument in clone.Arguments)
				dictionary.Add(argument.Uid, argument.Uid = Guid.NewGuid());

			foreach (var step in clone.Steps)
				ReplaceStepUids(step, dictionary);

			var procedureViewModel = new ProcedureViewModel(clone);
			ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.Add(procedureViewModel.Procedure);
			Procedures.Add(procedureViewModel);
			SelectedProcedure = procedureViewModel;
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		bool CanPaste()
		{
			return _procedureToCopy != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var procedureDetailsViewModel = new ProcedureDetailsViewModel();
			if (DialogService.ShowModalWindow(procedureDetailsViewModel))
			{
				ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.Add(procedureDetailsViewModel.Procedure);
				var procedureViewModel = new ProcedureViewModel(procedureDetailsViewModel.Procedure);
				Procedures.Add(procedureViewModel);
				SelectedProcedure = procedureViewModel;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var procedureDetailsViewModel = new ProcedureDetailsViewModel(SelectedProcedure.Procedure);
			if (DialogService.ShowModalWindow(procedureDetailsViewModel))
			{
				SelectedProcedure.Update(procedureDetailsViewModel.Procedure);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedProcedure != null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = Procedures.IndexOf(SelectedProcedure);
			ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.Remove(SelectedProcedure.Procedure);
			Procedures.Remove(SelectedProcedure);
			index = Math.Min(index, Procedures.Count - 1);
			if (index > -1)
				SelectedProcedure = Procedures[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDelete()
		{
			return SelectedProcedure != null;
		}

		public void Select(Guid inputUid)
		{
			if (inputUid == Guid.Empty)
				return;
			var variables = new List<Variable>();
			var arguments = new List<Variable>();
			var procedureSteps = new List<ProcedureStep>();

			foreach (var procedure in Procedures)
			{
				variables.AddRange(procedure.Procedure.Variables);
				arguments.AddRange(procedure.Procedure.Arguments);
				procedureSteps.AddRange(GetAllSteps(procedure.Procedure.Steps, new List<ProcedureStep>()));
			}

			if (Procedures.Any(item => item.Procedure.Uid == inputUid))
			{
				SelectedProcedure = Procedures.FirstOrDefault(item => item.Procedure.Uid == inputUid);
			}
			else if (variables.Any(item => item.Uid == inputUid))
			{
				var selectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Variables.Any(z => z.Uid == inputUid));
				if (selectedProcedure != null)
				{
					SelectedProcedure = selectedProcedure;
					SelectedProcedure.ShowVariablesCommand.Execute();
				}
				SelectedProcedure.VariablesViewModel.SelectedVariable = SelectedProcedure.VariablesViewModel.Variables.FirstOrDefault(item => item.Variable.Uid == inputUid);
			}
			else if (arguments.Any(item => item.Uid == inputUid))
			{
				var selectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Arguments.Any(z => z.Uid == inputUid));
				if (selectedProcedure != null)
				{
					SelectedProcedure = selectedProcedure;
					SelectedProcedure.ShowArgumentsCommand.Execute();
				}
				SelectedProcedure.ArgumentsViewModel.SelectedVariable = SelectedProcedure.ArgumentsViewModel.Variables.FirstOrDefault(item => item.Variable.Uid == inputUid);
			}

			else if (procedureSteps.Any(item => item.UID == inputUid))
			{
				var selectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Steps.Any(z => z.UID == inputUid));
				if (selectedProcedure != null)
				{
					SelectedProcedure = selectedProcedure;
					SelectedProcedure.ShowStepsCommand.Execute();
				}
				SelectedProcedure.StepsViewModel.SelectedStep = SelectedProcedure.StepsViewModel.AllSteps.FirstOrDefault(item => item.Step.UID == inputUid);
			}
		}

		List<ProcedureStep> GetAllSteps(List<ProcedureStep> steps, List<ProcedureStep> resultSteps)
		{
			foreach (var step in steps)
			{
				resultSteps.Add(step);
				GetAllSteps(step.Children, resultSteps);
			}
			return resultSteps;
		}

		public override void OnShow()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;

			if (SelectedProcedure != null)
			{
				SelectedProcedure.Update();
				SelectedProcedure.StepsViewModel.UpdateContent();
			}

			ServiceFactory.SaveService.AutomationChanged = automationChanged;

			if (Procedures != null)
				Procedures = new SortableObservableCollection<ProcedureViewModel>(Procedures.OrderBy(x => x.Name));

			base.OnShow();
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.X, ModifierKeys.Control), CutCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.E, ModifierKeys.Control), EditCommand);
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel("Вырезать", CutCommand, "BCut"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "BPaste"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}