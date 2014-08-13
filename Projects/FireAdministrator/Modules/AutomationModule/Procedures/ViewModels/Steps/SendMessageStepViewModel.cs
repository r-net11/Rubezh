using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Linq;
using ValueType = FiresecAPI.Automation.ValueType;

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

		public void UpdateContent()
		{
			Variables = new ObservableCollection<VariableViewModel>();
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			foreach (var variable in variablesAndArguments)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			ValueTypes = new ObservableCollection<ValueType> { ValueType.IsLocalVariable, ValueType.IsValue };
			SelectedValueType = SendMessageArguments.ValueType;
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == SendMessageArguments.VariableUid);
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => SelectedVariable);
		}

		public string Description
		{
			get { return Message; }
		}
	}
}