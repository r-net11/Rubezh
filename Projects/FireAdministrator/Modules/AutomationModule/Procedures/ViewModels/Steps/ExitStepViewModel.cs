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
			VariableTypes = new ObservableCollection<VariableType>
			{
				VariableType.IsGlobalVariable, VariableType.IsLocalVariable, VariableType.IsValue
			};
		}

		public void Update(List<Variable> localVariables)
		{
			Variables = new ObservableCollection<VariableViewModel>();
			if (localVariables == null)
				localVariables = new List<Variable>();
			foreach (var variable in localVariables.FindAll(x => (x.ValueType == ValueType.Integer) && (!x.IsList)))
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
			SelectedVariableType = ExitCode.VariableType;
			OnPropertyChanged(() => GlobalVariables);
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => ExitCodeTypes);
		}

		public ObservableCollection<VariableType> VariableTypes { get; private set; }
		public VariableType SelectedVariableType
		{
			get { return ExitCode.VariableType; }
			set
			{
				ExitCode.VariableType = value;
				if (value == VariableType.IsGlobalVariable)
					DescriptionValue = SelectedGlobalVariable != null ? SelectedGlobalVariable.Name : "";
				if (value == VariableType.IsLocalVariable)
					DescriptionValue = SelectedVariable != null ? SelectedVariable.Name : "";
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariableType);
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
				else if (SelectedVariableType == VariableType.IsGlobalVariable)
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
				else if (SelectedVariableType == VariableType.IsLocalVariable)
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
				if (SelectedVariableType == VariableType.IsValue)
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
