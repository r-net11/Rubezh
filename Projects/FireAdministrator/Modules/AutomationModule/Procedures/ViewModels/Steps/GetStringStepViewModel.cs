using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GetStringStepViewModel : BaseViewModel, IStepViewModel
	{
		private GetStringArguments GetStringArguments { get; set; }
		private Procedure Procedure { get; set; }

		public GetStringStepViewModel(GetStringArguments getStringArguments, Procedure procedure)
		{
			GetStringArguments = getStringArguments;
			Procedure = procedure;
			UpdateContent();
			ChangeStringOperationCommand = new RelayCommand(OnChangeStringOperation);
		}

		public StringOperation StringOperation
		{
			get { return GetStringArguments.StringOperation; }
			set
			{
				GetStringArguments.StringOperation = value;
				OnPropertyChanged(() => StringOperation);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand ChangeStringOperationCommand { get; private set; }

		private void OnChangeStringOperation()
		{
			StringOperation = StringOperation == StringOperation.Is ? StringOperation.Add : StringOperation.Is;
		}

		public void UpdateContent()
		{
			ResultVariables = new ObservableCollection<VariableViewModel>();
			Variables = new ObservableCollection<VariableViewModel>();
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);

			foreach (var variable in variablesAndArguments.FindAll(x => ((x.ValueType == ValueType.String) && (x.IsList))))
			{
				var variableViewModel = new VariableViewModel(variable);
				ResultVariables.Add(variableViewModel);
			}
			foreach (var variable in variablesAndArguments.FindAll(x => ((x.ValueType == ValueType.Object) && (!x.IsList))))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedResultVariable = ResultVariables.FirstOrDefault(x => x.Variable.Uid == GetStringArguments.ResultVariableUid);
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == GetStringArguments.VariableUid);
			StringOperation = GetStringArguments.StringOperation;
			OnPropertyChanged(() => ResultVariables);
			OnPropertyChanged(() => SelectedResultVariable);
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => SelectedVariable);
		}

		public string Description
		{
			get { return ""; }
		}


		public ObservableCollection<VariableViewModel> ResultVariables { get; private set; }
		private VariableViewModel _selectedResultVariable;
		public VariableViewModel SelectedResultVariable
		{
			get { return _selectedResultVariable; }
			set
			{
				_selectedResultVariable = value;
				if (value != null)
					GetStringArguments.ResultVariableUid = value.Variable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedResultVariable);
			}
		}

		public static ObservableCollection<Property> Properties { get; set; }
		public Property SelectedProperty
		{
			get { return GetStringArguments.Property; }
			set
			{
				GetStringArguments.Property = value;
				OnPropertyChanged(() => SelectedProperty);
				ServiceFactory.SaveService.AutomationChanged = true;
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
				{
					Properties = new ObservableCollection<Property>(AutomationConfiguration.ObjectTypeToProperiesList(value.ObjectType));
					if (GetStringArguments.VariableUid != value.Variable.Uid)
					{
						GetStringArguments.VariableUid = value.Variable.Uid;
						SelectedProperty = Properties.FirstOrDefault();
						OnPropertyChanged(()=>SelectedProperty);
					}
					OnPropertyChanged(() => Properties);
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}
	}
}