using AutomationModule.Plans;
using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ProceduresViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static ProcedureStep StepToCopy { get; set; }
		public static ProceduresViewModel Current { get; private set; }
		public ProceduresViewModel()
		{
			Current = this;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);

			Menu = new ProceduresMenuViewModel(this);
			SetRibbonItems();
			IsRightPanelEnabled = true;
			IsRightPanelVisible = true;
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

		void ReplaceStepUids(ProcedureStep step, Dictionary<Guid, Guid> dictionary)
		{
			step.UID = Guid.NewGuid();

			foreach (var argument in step.GetType().GetProperties().Where(x => x.PropertyType == typeof(Argument)))
			{
				var value = (Argument)argument.GetValue(step, null);
				ReplaceVariableUid(value, dictionary);
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
			AutomationPlanExtension.Instance.Cache.BuildSafe<Procedure>();
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
				AutomationPlanExtension.Instance.Cache.BuildSafe<Procedure>();
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
				procedureDetailsViewModel.Procedure.OnChanged();
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
			if (MessageBoxService.ShowQuestion("Вы действительно хотите удалить процедуру?"))
				DoDelete();
		}
		void DoDelete()
		{
			var index = Procedures.IndexOf(SelectedProcedure);
			ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.Remove(SelectedProcedure.Procedure);
			SelectedProcedure.Procedure.OnChanged();
			Procedures.Remove(SelectedProcedure);
			index = Math.Min(index, Procedures.Count - 1);
			if (index > -1)
				SelectedProcedure = Procedures[index];
			ServiceFactory.SaveService.AutomationChanged = true;
			AutomationPlanExtension.Instance.Cache.BuildSafe<Procedure>();
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

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "BPaste"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}