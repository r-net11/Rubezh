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
	public class ArithmeticStepViewModel : BaseViewModel, IStepViewModel
	{
		public ArithmeticArguments ArithmeticArguments { get; private set; }
		public Procedure Procedure { get; private set; }
		public ArithmeticParameterViewModel Variable1 { get; set; }
		public ArithmeticParameterViewModel Variable2 { get; set; }
		public ArithmeticParameterViewModel Result { get; set; }
		public Action UpdateDescriptionHandler { get; set; }

		public ArithmeticStepViewModel(ArithmeticArguments arithmeticArguments, Procedure procedure, Action updateDescriptionHandler)
		{
			Procedure = procedure;
			UpdateDescriptionHandler = updateDescriptionHandler;
			ArithmeticArguments = arithmeticArguments;
			SelectedOperationType = ArithmeticArguments.OperationType;
			SelectedArithmeticType = ArithmeticArguments.ArithmeticType;
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			var variableTypes = new List<VariableType> { VariableType.IsGlobalVariable, VariableType.IsLocalVariable };
			Result = new ArithmeticParameterViewModel(ArithmeticArguments.Result, variableTypes);
			variableTypes.Add(VariableType.IsValue);
			Variable1 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable1, variableTypes);
			Variable2 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable2, variableTypes);
			OperationTypes = new ObservableCollection<OperationType>(Enum.GetValues(typeof(OperationType)).Cast<OperationType>().ToList());
			Variable1.UpdateDescriptionHandler = updateDescriptionHandler;
			Variable2.UpdateDescriptionHandler = updateDescriptionHandler;
			Result.UpdateDescriptionHandler = updateDescriptionHandler;
			UpdateContent();
		}

		public void UpdateContent()
		{
			var allVariables = new List<Variable>(FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			allVariables.AddRange(Procedure.Variables);
			allVariables.AddRange(Procedure.Arguments);
			allVariables = allVariables.FindAll(x => !x.IsList);
			if (SelectedOperationType == OperationType.BoolOperation)
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.Boolean);
			if (SelectedOperationType == OperationType.IntegerOperation)
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.Integer);
			if (SelectedOperationType == OperationType.DateTimeOperation)
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.DateTime);
			if (SelectedOperationType == OperationType.StringOperation)
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.String);
			Variable1.Update(allVariables);
			Variable2.Update(allVariables);
			Result.Update(allVariables);
		}

		public string Description
		{
			get
			{
				string var1 = Variable1.DescriptionValue;
				if (String.IsNullOrEmpty(var1))
					var1 = "пусто";
				string var2 = Variable2.DescriptionValue;
				if (String.IsNullOrEmpty(var2))
					var2 = "пусто";
				string res = Result.DescriptionValue;
				if (String.IsNullOrEmpty(res))
					res = "пусто";
				var op = "";
				switch (SelectedArithmeticType)
				{
					case ArithmeticType.Add:
						op = "+";
						break;
					case ArithmeticType.Sub:
						op = "-";
						break;
					case ArithmeticType.Div:
						op = ":";
						break;
					case ArithmeticType.Multi:
						op = "*";
						break;
				}

				return "<" + res + ">" + " = " + "<" + var1 + ">" + op + " " + "<" + var2 + ">";
			}
		}

		public ObservableCollection<ArithmeticType> ArithmeticTypes { get; private set; }
		public ArithmeticType SelectedArithmeticType
		{
			get { return ArithmeticArguments.ArithmeticType; }
			set
			{
				ArithmeticArguments.ArithmeticType = value;
				if (UpdateDescriptionHandler!=null)
					UpdateDescriptionHandler();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedArithmeticType);
			}
		}

		public ObservableCollection<OperationType> OperationTypes { get; private set; }
		public OperationType SelectedOperationType
		{
			get { return ArithmeticArguments.OperationType; }
			set
			{
				ArithmeticArguments.OperationType = value;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				ArithmeticTypes = new ObservableCollection<ArithmeticType>();
				if (value == OperationType.BoolOperation)
					ArithmeticTypes = new ObservableCollection<ArithmeticType> { ArithmeticType.And, ArithmeticType.Or };
				if (value == OperationType.DateTimeOperation)
					ArithmeticTypes = new ObservableCollection<ArithmeticType> { ArithmeticType.Add, ArithmeticType.Sub };
				if (value == OperationType.StringOperation)
					ArithmeticTypes = new ObservableCollection<ArithmeticType> { ArithmeticType.Concat};
				if (value == OperationType.IntegerOperation)
					ArithmeticTypes = new ObservableCollection<ArithmeticType> { ArithmeticType.Add, ArithmeticType.Sub, ArithmeticType.Multi, ArithmeticType.Div};
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedOperationType);
			}
		}
	}

	public class ArithmeticParameterViewModel : BaseViewModel
	{
		public ArithmeticParameter ArithmeticParameter { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }

		public ArithmeticParameterViewModel(ArithmeticParameter arithmeticParameter, List<VariableType> availableVariableTypes)
		{
			ArithmeticParameter = arithmeticParameter;
			VariableTypes = new ObservableCollection<VariableType>(availableVariableTypes);
		}

		public void Update(List<Variable> variables)
		{
			Variables = new List<VariableViewModel>();
			foreach (var variable in variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}

			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == ArithmeticParameter.VariableUid);
			SelectedVariableType = ArithmeticParameter.VariableType;
			OnPropertyChanged(() => LocalVariables);
			OnPropertyChanged(() => GlobalVariables);
		}

		public ObservableCollection<VariableType> VariableTypes { get; private set; }
		public VariableType SelectedVariableType
		{
			get { return ArithmeticParameter.VariableType; }
			set
			{
				ArithmeticParameter.VariableType = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariableType);
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

		public int Value
		{
			get { return ArithmeticParameter.Value; }
			set
			{
				ArithmeticParameter.Value = value;
				DescriptionValue = value.ToString();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Value);
			}
		}

		private List<VariableViewModel> Variables { get; set; }
		public ObservableCollection<VariableViewModel> LocalVariables 
		{ 
			get
			{
				return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => !x.IsGlobal));
			}
		}

		public ObservableCollection<VariableViewModel> GlobalVariables
		{
			get
			{
				return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => x.IsGlobal));
			}
		}

		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (_selectedVariable != null)
				{
					ArithmeticParameter.VariableUid = value.Variable.Uid;
					DescriptionValue = value.Name;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}
	}
}