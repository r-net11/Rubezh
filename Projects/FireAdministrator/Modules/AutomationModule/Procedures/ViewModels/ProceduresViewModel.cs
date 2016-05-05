using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AutomationModule.Plans;
using Common;
using FiresecAPI.Automation;
using FiresecAPI.Models.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class ProceduresViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static ProceduresViewModel Current { get; private set; }
		public ProceduresViewModel()
		{
			Current = this;
			Menu = new ProceduresMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CutCommand = new RelayCommand(OnCut, CanCopy);
			RegisterShortcuts();
			SetRibbonItems();
			IsRightPanelEnabled = true;
			IsRightPanelVisible = false;
		}

		public void Initialize()
		{
			Procedures = new SortableObservableCollection<ProcedureViewModel>();
			if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures == null)
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures = new List<Procedure>();

			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
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
					value.StepsViewModel.SelectedStep = value.StepsViewModel.AllSteps.FirstOrDefault();
					value.StepsViewModel.UpdateContent();
				}
			}
		}

		Procedure _procedureToCopy;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_procedureToCopy = Utils.Clone(SelectedProcedure.Procedure);
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

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			_procedureToCopy.Uid = Guid.NewGuid();
			var procedureViewModel = new ProcedureViewModel(Utils.Clone(_procedureToCopy));
			FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Add(procedureViewModel.Procedure);
			Procedures.Add(procedureViewModel);
			SelectedProcedure = procedureViewModel;
			ServiceFactory.SaveService.AutomationChanged = true;
			AutomationPlanExtension.Instance.Cache.BuildSafe<Procedure>();
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
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Add(procedureDetailsViewModel.Procedure);
				var procedureViewModel = new ProcedureViewModel(procedureDetailsViewModel.Procedure);
				Procedures.Add(procedureViewModel);
				SelectedProcedure = procedureViewModel;
				ServiceFactory.SaveService.AutomationChanged = true;
				AutomationPlanExtension.Instance.Cache.BuildSafe<Procedure>();
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
			FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Remove(SelectedProcedure.Procedure);
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
			var variables = new List<IVariable>();
			var arguments = new List<IVariable>();
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
			else if (variables.Any(item => item.UID == inputUid))
			{
				var selectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Variables.Any(z => z.UID == inputUid));
				if (selectedProcedure != null)
				{
					SelectedProcedure = selectedProcedure;
					SelectedProcedure.ShowVariablesCommand.Execute();
				}
				SelectedProcedure.VariablesViewModel.SelectedVariable = SelectedProcedure.VariablesViewModel.Variables.FirstOrDefault(item => item.Variable.UID == inputUid);
			}
			else if (arguments.Any(item => item.UID == inputUid))
			{
				var selectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Arguments.Any(z => z.UID == inputUid));
				if (selectedProcedure != null)
				{
					SelectedProcedure = selectedProcedure;
					SelectedProcedure.ShowArgumentsCommand.Execute();
				}
				SelectedProcedure.ArgumentsViewModel.SelectedVariable = SelectedProcedure.ArgumentsViewModel.Variables.FirstOrDefault(item => item.Variable.UID == inputUid);
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
				SelectedProcedure.StepsViewModel.UpdateContent();

			ServiceFactory.SaveService.AutomationChanged = automationChanged;

			if (Procedures != null)
				Procedures.Sort(x => x.Name);

			base.OnShow();
			SelectedProcedure = Procedures != null ? Procedures.FirstOrDefault() : null;
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
			RibbonItems = new List<RibbonMenuItemViewModel>
			{
				new RibbonMenuItemViewModel(Resources.Language.Procedures.ViewModels.ProceduresViewModel.Edition, new ObservableCollection<RibbonMenuItemViewModel>
				{
					new RibbonMenuItemViewModel(Resources.Language.Procedures.ViewModels.ProceduresViewModel.Add, AddCommand, "BAdd"),
					new RibbonMenuItemViewModel(Resources.Language.Procedures.ViewModels.ProceduresViewModel.Edit, EditCommand, "BEdit"),
					new RibbonMenuItemViewModel(Resources.Language.Procedures.ViewModels.ProceduresViewModel.Copy, CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel(Resources.Language.Procedures.ViewModels.ProceduresViewModel.Cut, CutCommand, "BCut"),
					new RibbonMenuItemViewModel(Resources.Language.Procedures.ViewModels.ProceduresViewModel.Paste, PasteCommand, "BPaste"),
					new RibbonMenuItemViewModel(Resources.Language.Procedures.ViewModels.ProceduresViewModel.Delete, DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}