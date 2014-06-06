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
		public StepsViewModel StepsViewModel { get; private set; }

		public ProcedureViewModel(Procedure procedure)
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDeleted);

			Procedure = procedure;
			StepsViewModel = new StepsViewModel(procedure);
			InputObjects = new ProcedureInputObjectsViewModel(procedure);
			ProcedureSteps = new ObservableCollection<ProcedureStepViewModel>();			
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
					var stepViewModel = new StepViewModel(StepsViewModel, procedureStep);
					StepsViewModel.RootSteps.Add(stepViewModel);
					//ProcedureSteps.Add(procedureStepViewModel);
					//SelectedProcedureStep = procedureStepViewModel;
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