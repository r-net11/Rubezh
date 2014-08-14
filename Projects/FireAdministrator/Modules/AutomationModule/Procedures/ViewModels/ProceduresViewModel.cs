using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
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
			IsRightPanelEnabled = true;
			IsRightPanelVisible = false;
		}

		public void Initialize()
		{
			Procedures = new ObservableCollection<ProcedureViewModel>();
			if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures == null)
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures = new List<Procedure>();
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				var procedureViewModel = new ProcedureViewModel(procedure);
				Procedures.Add(procedureViewModel);
			}
			SelectedProcedure = Procedures.FirstOrDefault();
		}

		ObservableCollection<ProcedureViewModel> _procedures;
		public ObservableCollection<ProcedureViewModel> Procedures
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
				if (value != null)
					value.StepsViewModel.SelectedStep = value.StepsViewModel.AllSteps.FirstOrDefault();
				OnPropertyChanged(() => SelectedProcedure);
			}
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
			FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Remove(SelectedProcedure.Procedure);
			Procedures.Remove(SelectedProcedure);
			SelectedProcedure = Procedures.FirstOrDefault();
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

			foreach (var procedure in Procedures)
			{
				variables.AddRange(procedure.Procedure.Variables);
				arguments.AddRange(procedure.Procedure.Arguments);
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
		}

		public override void OnShow()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			if (SelectedProcedure != null)
				SelectedProcedure.StepsViewModel.UpdateContent();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			base.OnShow();
		}

		public override void OnHide()
		{
			base.OnHide();
		}
	}
}