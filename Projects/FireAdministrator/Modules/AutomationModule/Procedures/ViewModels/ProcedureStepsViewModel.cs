using System.Collections.ObjectModel;
using FiresecAPI.XModels.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureStepsViewModel : BaseViewModel
	{
		public ObservableCollection<ProcedureStepViewModel> ProcedureSteps { get; private set; }
		public ProcedureStepsViewModel()
		{
			DeleteCommand = new RelayCommand(OnDelete, CanDeleted);
			AddCommand = new RelayCommand(OnAdd);
		}

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

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			ProcedureSteps.Remove(SelectedProcedureStep);
		}
		bool CanDeleted()
		{
			return SelectedProcedureStep != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var procedureStepViewModel = new ProcedureStepViewModel(new ProcedureStep());
			ProcedureSteps.Add(procedureStepViewModel);
		}
	}
}
