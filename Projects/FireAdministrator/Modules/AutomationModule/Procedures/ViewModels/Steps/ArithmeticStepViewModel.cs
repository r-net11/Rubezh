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
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			var variableTypes = new List<VariableType> { VariableType.IsGlobalVariable, VariableType.IsLocalVariable };
			Result = new ArithmeticParameterViewModel(ArithmeticArguments.Result, variableTypes);
			variableTypes.Add(VariableType.IsValue);
			Variable1 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable1, variableTypes);
			Variable2 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable2, variableTypes);
			ArithmeticValueTypes = new ObservableCollection<ValueType>(Enum.GetValues(typeof(ValueType)).Cast<ValueType>().ToList());
			TimeTypes = new ObservableCollection<TimeType>(Enum.GetValues(typeof(TimeType)).Cast<TimeType>().ToList());
			Variable1.UpdateDescriptionHandler = updateDescriptionHandler;
			Variable2.UpdateDescriptionHandler = updateDescriptionHandler;
			Result.UpdateDescriptionHandler = updateDescriptionHandler;
			SelectedArithmeticValueType = ArithmeticArguments.ArithmeticValueType;
		}

		public void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure);
			allVariables = allVariables.FindAll(x => !x.IsList);
			var allVariables2 = new List<Variable>(allVariables);

			if (SelectedArithmeticValueType == ValueType.Boolean)
			{
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.Boolean);
				allVariables2 = allVariables2.FindAll(x => x.ValueType == ValueType.Boolean);
			}
			if (SelectedArithmeticValueType == ValueType.Integer)
			{
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.Integer);
				allVariables2 = allVariables2.FindAll(x => x.ValueType == ValueType.Integer);
			}
			if (SelectedArithmeticValueType == ValueType.DateTime)
			{
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.DateTime);
				allVariables2 = allVariables2.FindAll(x => x.ValueType == ValueType.Integer);
			}
			if (SelectedArithmeticValueType == ValueType.String)
			{
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.String);
				allVariables2 = allVariables2.FindAll(x => x.ValueType == ValueType.String);
			}
			Variable1.Update(allVariables);
			Variable2.Update(allVariables2);
			Result.Update(allVariables);
			SelectedArithmeticOperationType = ArithmeticOperationTypes.Contains(ArithmeticArguments.ArithmeticOperationType) ? ArithmeticArguments.ArithmeticOperationType : ArithmeticOperationTypes.FirstOrDefault();
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
				switch (SelectedArithmeticOperationType)
				{
					case ArithmeticOperationType.Add:
						op = "+";
						break;
					case ArithmeticOperationType.Sub:
						op = "-";
						break;
					case ArithmeticOperationType.Div:
						op = ":";
						break;
					case ArithmeticOperationType.Multi:
						op = "*";
						break;
				}

				return "<" + res + ">" + " = " + "<" + var1 + ">" + op + " " + "<" + var2 + ">";
			}
		}

		public ObservableCollection<ArithmeticOperationType> ArithmeticOperationTypes { get; private set; }
		public ArithmeticOperationType SelectedArithmeticOperationType
		{
			get { return ArithmeticArguments.ArithmeticOperationType; }
			set
			{
				ArithmeticArguments.ArithmeticOperationType = value;
				if (UpdateDescriptionHandler!=null)
					UpdateDescriptionHandler();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedArithmeticOperationType);
			}
		}

		public ObservableCollection<TimeType> TimeTypes { get; private set; }
		public TimeType SelectedTimeType
		{
			get { return ArithmeticArguments.TimeType; }
			set
			{
				ArithmeticArguments.TimeType = value;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedTimeType);
			}
		}

		public ObservableCollection<ValueType> ArithmeticValueTypes { get; private set; }
		public ValueType SelectedArithmeticValueType
		{
			get { return ArithmeticArguments.ArithmeticValueType; }
			set
			{
				ArithmeticArguments.ArithmeticValueType = value;
				ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType>();
				if (value == ValueType.Boolean)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.And, ArithmeticOperationType.Or };
				if (value == ValueType.DateTime)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add, ArithmeticOperationType.Sub };
				if (value == ValueType.String)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Concat};
				if (value == ValueType.Integer)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add, ArithmeticOperationType.Sub, ArithmeticOperationType.Multi, ArithmeticOperationType.Div};
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				OnPropertyChanged(() => ArithmeticOperationTypes);
				UpdateContent();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedArithmeticValueType);
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