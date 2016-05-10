using System.Linq;
using AutomationModule.Procedures;
using StrazhAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;

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

			MenuTypes = new ObservableRangeCollection<MenuType>
			{
				MenuType.IsSteps,
				MenuType.IsVariables,
				MenuType.IsArguments,
				MenuType.IsConditions
			};
			SelectedMenuType = MenuTypes.FirstOrDefault();
		}

		public ObservableRangeCollection<MenuType> MenuTypes { get; private set; }
		MenuType _selectedMenuType;
		public MenuType SelectedMenuType
		{
			get { return _selectedMenuType; }
			set
			{
				_selectedMenuType = value;
				OnPropertyChanged(() => SelectedMenuType);
			}
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

		public string Description
		{
			get { return Procedure.Description; }
			set
			{
				Procedure.Description = value;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update(Procedure procedure)
		{
			Procedure = procedure;
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => IsActive);
		}

		public RelayCommand ShowStepsCommand { get; private set; }
		void OnShowSteps()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			StepsViewModel.UpdateContent();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			SelectedMenuType = MenuType.IsSteps;
		}

		public RelayCommand ShowVariablesCommand { get; private set; }
		void OnShowVariables()
		{
			SelectedMenuType = MenuType.IsVariables;
		}

		public RelayCommand ShowArgumentsCommand { get; private set; }
		void OnShowArguments()
		{
			SelectedMenuType = MenuType.IsArguments;
		}

		public RelayCommand ShowConditionsCommand { get; private set; }
		void OnShowConditions()
		{
			SelectedMenuType = MenuType.IsConditions;
		}

		public bool IsActive
		{
			get { return Procedure.IsActive; }
			set
			{
				Procedure.IsActive = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => IsActive);
			}
		}

		public void Update()
		{
			_visualizetionState = 
				Procedure.PlanElementUIDs.Count == 0 
				? VisualizationState.NotPresent 
				: (Procedure.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}
		public bool IsOnPlan
		{
			get { return Procedure.PlanElementUIDs.Count > 0; }
		}
		VisualizationState _visualizetionState;
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
	}
}