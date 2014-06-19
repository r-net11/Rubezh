using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }
		public StepsViewModel StepsViewModel { get; private set; }
		public VariablesViewModel VariablesViewModel { get; private set; }
		public ArgumentsViewModel ArgumentsViewModel { get; private set; }
		public ConditionsViewModel ConditionsViewModel { get; private set; }

		public ProcedureViewModel(Procedure procedure)
		{
			ShowStepsCommand = new RelayCommand(OnShowSteps);
			ShowVariablesCommand = new RelayCommand(OnShowVariables);
			ShowArgumentsCommand = new RelayCommand(OnShowArguments);
			ShowConditionsCommand = new RelayCommand(OnShowConditions);

			Procedure = procedure;
			StepsViewModel = new StepsViewModel(procedure);
			VariablesViewModel = new VariablesViewModel(procedure);
			ArgumentsViewModel = new ArgumentsViewModel(procedure);
			ConditionsViewModel = new ConditionsViewModel(procedure);
			OnShowSteps();
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

		public RelayCommand ShowStepsCommand { get; private set; }
		void OnShowSteps()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			StepsViewModel.UpdateContent();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			IsStepsVisible = true;
			IsVariablesVisible = false;
			IsArgumentsVisible = false;
			IsConditionsVisible = false;
		}

		public RelayCommand ShowVariablesCommand { get; private set; }
		void OnShowVariables()
		{
			IsStepsVisible = false;
			IsVariablesVisible = true;
			IsArgumentsVisible = false;
			IsConditionsVisible = false;
		}

		public RelayCommand ShowArgumentsCommand { get; private set; }
		void OnShowArguments()
		{
			IsStepsVisible = false;
			IsVariablesVisible = false;
			IsArgumentsVisible = true;
			IsConditionsVisible = false;
		}

		public RelayCommand ShowConditionsCommand { get; private set; }
		void OnShowConditions()
		{
			IsStepsVisible = false;
			IsVariablesVisible = false;
			IsArgumentsVisible = false;
			IsConditionsVisible = true;
		}

		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		bool _isStepsVisible;
		public bool IsStepsVisible
		{
			get { return _isStepsVisible; }
			set
			{
				_isStepsVisible = value;
				OnPropertyChanged(() => IsStepsVisible);
			}
		}

		bool _isVariablesVisible;
		public bool IsVariablesVisible
		{
			get { return _isVariablesVisible; }
			set
			{
				_isVariablesVisible = value;
				OnPropertyChanged(() => IsVariablesVisible);
			}
		}

		bool _isArgumentsVisible;
		public bool IsArgumentsVisible
		{
			get { return _isArgumentsVisible; }
			set
			{
				_isArgumentsVisible = value;
				OnPropertyChanged(() => IsArgumentsVisible);
			}
		}

		bool _isConditionsVisible;
		public bool IsConditionsVisible
		{
			get { return _isConditionsVisible; }
			set
			{
				_isConditionsVisible = value;
				OnPropertyChanged(() => IsConditionsVisible);
			}
		}
	}
}