using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Linq;
using ValueType = FiresecAPI.Automation.ValueType;
using FiresecClient;

namespace AutomationModule.ViewModels
{
	public class SendMessageStepViewModel : BaseViewModel, IStepViewModel
	{
		SendMessageArguments SendMessageArguments { get; set; }
		public Action UpdateDescriptionHandler { get; set; }
		Procedure Procedure { get; set; }
		public SendMessageStepViewModel(SendMessageArguments sendMessageArguments, Procedure procedure, Action updateDescriptionHandler)
		{
			SendMessageArguments = sendMessageArguments;
			UpdateDescriptionHandler = updateDescriptionHandler;
			Procedure = procedure;
			UpdateContent();
		}

		public string Message
		{
			get { return SendMessageArguments.Message; }
			set
			{
				SendMessageArguments.Message = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Message);
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
			}
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		public ValueType SelectedValueType
		{
			get { return SendMessageArguments.ValueType; }
			set
			{
				SendMessageArguments.ValueType = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedValueType);
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
			}
		}

		public ObservableCollection<VariableViewModel> Variables { get; private set; }
		private VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (value != null)
					SendMessageArguments.VariableUid = value.Variable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		public ObservableCollection<GlobalVariableViewModel> GlobalVariables { get; private set; }
		private GlobalVariableViewModel _selectedGlobalVariable;
		public GlobalVariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				if (value != null)
					SendMessageArguments.GlobalVariableUid = value.GlobalVariable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedGlobalVariable);
			}
		}

		public void UpdateContent()
		{
			Variables = new ObservableCollection<VariableViewModel>();
			GlobalVariables = new ObservableCollection<GlobalVariableViewModel>();
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			foreach (var variable in variablesAndArguments)
			{
				Variables.Add(new VariableViewModel(variable));
			}
			foreach (var globalVariable in FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				GlobalVariables.Add(new GlobalVariableViewModel(globalVariable));
			}
			ValueTypes = new ObservableCollection<ValueType>(Enum.GetValues(typeof(ValueType)).Cast<ValueType>().ToList());
			SelectedValueType = SendMessageArguments.ValueType;
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == SendMessageArguments.VariableUid);
			SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.GlobalVariable.Uid == SendMessageArguments.GlobalVariableUid);
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => GlobalVariables);
			OnPropertyChanged(() => SelectedVariable);
		}

		public string Description
		{
			get 
			{
				if (SelectedValueType == ValueType.IsLocalVariable)
					return "<" + SelectedVariable.Name + ">";
				if (SelectedValueType == ValueType.IsGlobalVariable)
					return "<" + SelectedGlobalVariable.Name + ">";
				return Message; 
			}
		}
	}
}