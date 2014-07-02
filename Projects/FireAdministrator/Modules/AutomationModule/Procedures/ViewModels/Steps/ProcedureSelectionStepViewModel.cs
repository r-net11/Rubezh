using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureSelectionStepViewModel : BaseViewModel, IStepViewModel
	{
		ProcedureSelectionArguments ProcedureSelectionArguments { get; set; }
		Procedure Procedure { get; set; }

		public ProcedureSelectionStepViewModel(ProcedureSelectionArguments procedureSelectionArguments, Procedure procedure)
		{
			ProcedureSelectionArguments = procedureSelectionArguments;
			Procedure = procedure;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDeleted);
			UpdateContent();
		}

		public void UpdateContent()
		{
			ScheduleProcedures = new ObservableCollection<ScheduleProcedureViewModel>();
			foreach (var scheduleProcedure in ProcedureSelectionArguments.ScheduleProcedures)
			{
				var procedure = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == scheduleProcedure.ProcedureUid);
				if (procedure != null)
				{
					var scheduleProcedureViewModel = new ScheduleProcedureViewModel(scheduleProcedure);
					scheduleProcedureViewModel.UpdateArguments(procedure);
					ScheduleProcedures.Add(scheduleProcedureViewModel);
				}
			}
			SelectedScheduleProcedure = ScheduleProcedures.FirstOrDefault();
			OnPropertyChanged(() => SelectedScheduleProcedure);
			OnPropertyChanged(() => ScheduleProcedures);
		}

		public ObservableCollection<ScheduleProcedureViewModel> ScheduleProcedures { get; private set; }
		private ScheduleProcedureViewModel _selectedScheduleProcedure;
		public ScheduleProcedureViewModel SelectedScheduleProcedure
		{
			get { return _selectedScheduleProcedure; }
			set
			{
				_selectedScheduleProcedure = value;
				OnPropertyChanged(() => SelectedScheduleProcedure);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var procedureSelectionViewModel = new ProcedureSelectionViewModel(Procedure.Uid);
			if (DialogService.ShowModalWindow(procedureSelectionViewModel))
			{
				if (procedureSelectionViewModel.SelectedProcedure != null)
				{
					var scheduleProcedure = new ScheduleProcedure();
					scheduleProcedure.ProcedureUid = procedureSelectionViewModel.SelectedProcedure.Procedure.Uid;
					scheduleProcedure.Arguments = new List<Argument>();
					foreach (var variable in procedureSelectionViewModel.SelectedProcedure.Procedure.Arguments)
					{
						var argument = new Argument(variable);
						scheduleProcedure.Arguments.Add(argument);
					}
					var scheduleProcedureViewModel = new ScheduleProcedureViewModel(scheduleProcedure);
					ScheduleProcedures.Add(scheduleProcedureViewModel);
					ProcedureSelectionArguments.ScheduleProcedures.Add(scheduleProcedureViewModel.ScheduleProcedure);
					SelectedScheduleProcedure = scheduleProcedureViewModel;
					ServiceFactory.SaveService.AutomationChanged = true;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			ProcedureSelectionArguments.ScheduleProcedures.Remove(SelectedScheduleProcedure.ScheduleProcedure);
			ScheduleProcedures.Remove(SelectedScheduleProcedure);
			SelectedScheduleProcedure = ScheduleProcedures.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDeleted()
		{
			return SelectedScheduleProcedure != null;
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
