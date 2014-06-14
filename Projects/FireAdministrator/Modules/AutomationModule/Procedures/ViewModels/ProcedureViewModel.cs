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
		public VariablesViewModel VariablesViewModel { get; private set; }
		public ConditionsViewModel ConditionsViewModel { get; private set; }

		public ProcedureViewModel(Procedure procedure)
		{
			ShowStepsCommand = new RelayCommand(OnShowSteps);
			ShowVariablesCommand = new RelayCommand(OnShowVariables);
			ShowConditionsCommand = new RelayCommand(OnShowConditions);

			Procedure = procedure;
			StepsViewModel = new StepsViewModel(procedure);
			VariablesViewModel = new VariablesViewModel(procedure);
			ConditionsViewModel = new ConditionsViewModel(procedure);
			InputObjects = new ProcedureInputObjectsViewModel(procedure);

			IsStepsVisible = true;
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
			IsStepsVisible = true;
			IsVariablesVisible = false;
			IsConditionsVisible = false;
		}

		public RelayCommand ShowVariablesCommand { get; private set; }
		void OnShowVariables()
		{
			IsStepsVisible = false;
			IsVariablesVisible = true;
			IsConditionsVisible = false;
		}

		public RelayCommand ShowConditionsCommand { get; private set; }
		void OnShowConditions()
		{
			IsStepsVisible = false;
			IsVariablesVisible = false;
			IsConditionsVisible = true;
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

		public ProcedureInputObjectsViewModel InputObjects { get; private set; }
	}
}