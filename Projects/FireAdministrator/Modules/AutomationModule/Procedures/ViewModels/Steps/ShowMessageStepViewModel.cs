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
	public class ShowMessageStepViewModel : BaseViewModel, IStepViewModel
	{
		ShowMessageArguments ShowMessageArguments { get; set; }
		public Action UpdateDescriptionHandler { get; set; }
		Procedure Procedure { get; set; }
		public ShowMessageStepViewModel(ShowMessageArguments showMessageArguments, Procedure procedure, Action updateDescriptionHandler)
		{
			ShowMessageArguments = showMessageArguments;
			UpdateDescriptionHandler = updateDescriptionHandler;
			Procedure = procedure;
			UpdateContent();
		}

		public string Message
		{
			get { return ShowMessageArguments.Message; }
			set
			{
				ShowMessageArguments.Message = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Message);
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
			}
		}

		public ObservableCollection<VariableType> VariableTypes { get; private set; }
		public VariableType SelectedVariableType
		{
			get { return ShowMessageArguments.VariableType; }
			set
			{
				ShowMessageArguments.VariableType = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariableType);
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
					ShowMessageArguments.VariableUid = value.Variable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		public ObservableCollection<VariableViewModel> GlobalVariables { get; private set; }
		private VariableViewModel _selectedGlobalVariable;
		public VariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				if (value != null)
					ShowMessageArguments.GlobalVariableUid = value.Variable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedGlobalVariable);
			}
		}

		public void UpdateContent()
		{
			Variables = new ObservableCollection<VariableViewModel>();
			GlobalVariables = new ObservableCollection<VariableViewModel>();
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			foreach (var variable in variablesAndArguments)
			{
				Variables.Add(new VariableViewModel(variable));
			}
			foreach (var globalVariable in FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				GlobalVariables.Add(new VariableViewModel(globalVariable));
			}
			VariableTypes = new ObservableCollection<VariableType>(Enum.GetValues(typeof(VariableType)).Cast<VariableType>().ToList());
			SelectedVariableType = ShowMessageArguments.VariableType;
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == ShowMessageArguments.VariableUid);
			SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.Variable.Uid == ShowMessageArguments.GlobalVariableUid);
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => GlobalVariables);
			OnPropertyChanged(() => SelectedVariable);
		}

		public string Description
		{
			get 
			{
				if (SelectedVariableType == VariableType.IsLocalVariable)
					return "<" + SelectedVariable.Name + ">";
				if (SelectedVariableType == VariableType.IsGlobalVariable)
					return "<" + SelectedGlobalVariable.Name + ">";
				return Message; 
			}
		}
	}
}