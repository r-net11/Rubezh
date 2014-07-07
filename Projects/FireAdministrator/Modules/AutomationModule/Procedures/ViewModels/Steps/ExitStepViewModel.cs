using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ExitStepViewModel: BaseViewModel, IStepViewModel
	{
		Procedure Procedure { get; set; }
		ExitArguments ExitArguments { get; set; }
		public ExitCodeViewModel ExitCode { get; private set; }
		public ExitStepViewModel(ExitArguments exitArguments, Procedure procedure)
		{
			ExitArguments = exitArguments;
			Procedure = procedure;
			ExitCode = new ExitCodeViewModel(exitArguments.ExitCode, procedure);
			UpdateContent();
		}

		public void UpdateContent()
		{
			ExitCode.Update(Procedure.Variables);
		}

		public string Message
		{
			get { return ExitArguments.Message; }
			set
			{
				ExitArguments.Message = value;
				OnPropertyChanged(()=>Message);
			}
		}

		public string Description
		{
			get { return ""; }
		}
	}

	public class ExitCodeViewModel : BaseViewModel
	{
		public Action UpdateDescriptionHandler { get; set; }
		ExitCode ExitCode { get; set; }

		public ExitCodeViewModel(ExitCode exitCode, Procedure procedure)
		{
			ExitCode = exitCode;
			ValueTypes = new ObservableCollection<ValueType>
			{
				ValueType.IsGlobalVariable, ValueType.IsLocalVariable, ValueType.IsValue
			};
		}

		public void Update(List<Variable> localVariables)
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			Variables = new ObservableCollection<VariableViewModel>();
			if (localVariables == null)
				localVariables = new List<Variable>();
			foreach (var variable in localVariables.FindAll(x => (x.VariableType == VariableType.Integer) && (!x.IsList)))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}

			if (localVariables.Any(x => x.Uid == ExitCode.VariableUid))
				SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == ExitCode.VariableUid);
			else
				SelectedVariable = null;
			GlobalVariables = new ObservableCollection<GlobalVariableViewModel>();
			foreach (var globalVariable in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				var globalVariableViewModel = new GlobalVariableViewModel(globalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			if (GlobalVariables.Any(x => x.GlobalVariable.Uid == ExitCode.GlobalVariableUid))
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.GlobalVariable.Uid == ExitCode.GlobalVariableUid);
			else
				SelectedGlobalVariable = null;

			ExitCodeTypes = new ObservableCollection<ExitCodeType>
			{
				ExitCodeType.Normal, ExitCodeType.Interrupt
			};
			SelectedValueType = ExitCode.ValueType;
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			OnPropertyChanged(() => GlobalVariables);
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => ExitCodeTypes);
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		public ValueType SelectedValueType
		{
			get { return ExitCode.ValueType; }
			set
			{
				ExitCode.ValueType = value;
				if (value == ValueType.IsGlobalVariable)
					DescriptionValue = SelectedGlobalVariable != null ? SelectedGlobalVariable.Name : "";
				if (value == ValueType.IsLocalVariable)
					DescriptionValue = SelectedVariable != null ? SelectedVariable.Name : "";
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedValueType);
			}
		}
		
		public ObservableCollection<GlobalVariableViewModel> GlobalVariables { get; private set; }
		GlobalVariableViewModel _selectedGlobalVariable;
		public GlobalVariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				if (_selectedGlobalVariable != null)
				{
					DescriptionValue = value.Name;
					ExitCode.VariableUid = Guid.Empty;
					ExitCode.GlobalVariableUid = value.GlobalVariable.Uid;
				}
				else if (SelectedValueType == ValueType.IsGlobalVariable)
					DescriptionValue = "";
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedGlobalVariable);
			}
		}

		public ObservableCollection<VariableViewModel> Variables { get; private set; }
		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (_selectedVariable != null)
				{
					DescriptionValue = value.Name;
					ExitCode.GlobalVariableUid = Guid.Empty;
					ExitCode.VariableUid = value.Variable.Uid;
				}
				else if (SelectedValueType == ValueType.IsLocalVariable)
					DescriptionValue = "";
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		public ObservableCollection<ExitCodeType> ExitCodeTypes { get; private set; }
		public ExitCodeType SelectedExitCodeType
		{
			get { return ExitCode.ExitCodeType; }
			set
			{
				ExitCode.ExitCodeType = value;
				ExitCode.GlobalVariableUid = Guid.Empty;
				ExitCode.VariableUid = Guid.Empty;
				if (SelectedValueType == ValueType.IsValue)
					DescriptionValue = value.ToString();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedExitCodeType);
			}
		}

		string _desctriptionValue;
		public string DescriptionValue
		{
			get { return _desctriptionValue; }
			private set
			{
				_desctriptionValue = value;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
			}
		}
	
	}
}
