using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }

		public ProcedureViewModel(Procedure procedure)
		{
			Procedure = procedure;
			InputObjects = new ProcedureInputObjectsViewModel(procedure);
			ProcedureSteps = new ObservableCollection<ProcedureStepViewModel>();
			DeleteCommand = new RelayCommand(OnDelete, CanDeleted);
			AddCommand = new RelayCommand(OnAdd);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var stepTypeSelectationViewModel = new StepTypeSelectationViewModel();
			if (DialogService.ShowModalWindow(stepTypeSelectationViewModel))
			{
				if (stepTypeSelectationViewModel.SelectedStepType != null && !stepTypeSelectationViewModel.SelectedStepType.IsFolder)
				{
					var procedureStep = new ProcedureStep();
					procedureStep.ProcedureStepType = stepTypeSelectationViewModel.SelectedStepType.ProcedureStepType;
					var selectedProcedureStep = new ProcedureStepViewModel(procedureStep);
					ProcedureSteps.Add(SelectedProcedureStep);
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			ProcedureSteps.Remove(SelectedProcedureStep);
		}
		bool CanDeleted()
		{
			return SelectedProcedureStep != null;
		}

		public string Name
		{
			get { return Procedure.Name; }
			set
			{
				Procedure.Name = value;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update(Procedure procedure)
		{
			Procedure = procedure;
			OnPropertyChanged("Name");
		}

		public ObservableCollection<ProcedureStepViewModel> ProcedureSteps { get; private set; }

		ProcedureStepViewModel _selectedProcedureStep;
		public ProcedureStepViewModel SelectedProcedureStep
		{
			get { return _selectedProcedureStep; }
			set
			{
				_selectedProcedureStep = value;
				OnPropertyChanged(() => SelectedProcedureStep);
			}
		}

		public ProcedureInputObjectsViewModel InputObjects { get; private set; }
	}
}