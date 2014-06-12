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
			Procedure = procedure;
			StepsViewModel = new StepsViewModel(procedure);
			InputObjects = new ProcedureInputObjectsViewModel(procedure);
			ProcedureSteps = new ObservableCollection<ProcedureStepViewModel>();
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