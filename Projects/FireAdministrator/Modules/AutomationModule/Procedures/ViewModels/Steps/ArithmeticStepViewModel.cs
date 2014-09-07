using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;
using System.Linq.Expressions;
using FiresecAPI;

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
			Result = new ArithmeticParameterViewModel(ArithmeticArguments.Result, false);
			Variable1 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable1);
			Variable2 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable2);
			ValueTypes = new ObservableCollection<ValueType>(ProcedureHelper.GetEnumList<ValueType>().FindAll(x => x != ValueType.Object));
			TimeTypes = ProcedureHelper.GetEnumObs<TimeType>();
			Variable1.UpdateDescriptionHandler = updateDescriptionHandler;
			Variable2.UpdateDescriptionHandler = updateDescriptionHandler;
			Result.UpdateDescriptionHandler = updateDescriptionHandler;
			SelectedValueType = ArithmeticArguments.ValueType;
		}

		public void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == SelectedValueType);
			var allVariables2 = new List<Variable>(allVariables);
			if (SelectedValueType == ValueType.DateTime)
				allVariables2 = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == ValueType.Integer);
			Variable1.Update(allVariables);
			Variable2.Update(allVariables2);
			Result.Update(allVariables);
			SelectedArithmeticOperationType = ArithmeticOperationTypes.Contains(ArithmeticArguments.ArithmeticOperationType) ? ArithmeticArguments.ArithmeticOperationType : ArithmeticOperationTypes.FirstOrDefault();
		}

		public string Description
		{
			get
			{
				var var1 = "пусто";
				var var2 = "пусто";
				var res = "пусто";
				switch(SelectedValueType)
				{
					case ValueType.Boolean:
						var1 = Variable1.SelectedVariableType == VariableType.IsValue ? Variable1.BoolValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.BoolValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ValueType.DateTime:
						var1 = Variable1.SelectedVariableType == VariableType.IsValue ? Variable1.DateTimeValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.IntValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						var2 = var2 + " " + SelectedTimeType.ToDescription();;
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ValueType.Integer:
						var1 = Variable1.SelectedVariableType == VariableType.IsValue ? Variable1.IntValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.IntValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ValueType.String:
						var1 = Variable1.SelectedVariableType == VariableType.IsValue ? Variable1.StringValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.StringValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
				}

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
					case ArithmeticOperationType.And:
						op = "И";
						break;
					case ArithmeticOperationType.Or:
						op = "Или";
						break;
				}

				return "<" + res + ">" + " = " + "<" + var1 + "> " + op + " <" + var2 + ">";
			}
		}

		public ObservableCollection<ArithmeticOperationType> ArithmeticOperationTypes { get; private set; }
		public ArithmeticOperationType SelectedArithmeticOperationType
		{
			get { return ArithmeticArguments.ArithmeticOperationType; }
			set
			{
				ArithmeticArguments.ArithmeticOperationType = value;
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
				OnPropertyChanged(() => SelectedTimeType);
			}
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		public ValueType SelectedValueType
		{
			get { return ArithmeticArguments.ValueType; }
			set
			{
				ArithmeticArguments.ValueType = value;
				ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType>();
				if (value == ValueType.Boolean)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.And, ArithmeticOperationType.Or };
				if (value == ValueType.DateTime)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add, ArithmeticOperationType.Sub };
				if (value == ValueType.String)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add };
				if (value == ValueType.Integer)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add, ArithmeticOperationType.Sub, ArithmeticOperationType.Multi, ArithmeticOperationType.Div};
				OnPropertyChanged(() => ArithmeticOperationTypes);
				OnPropertyChanged(() => SelectedValueType);
				UpdateContent();
			}
		}

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			base.OnPropertyChanged(propertyExpression);
			ServiceFactory.SaveService.AutomationChanged = true;
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
		}
	}

	public class ArithmeticParameterViewModel : BaseViewModel
	{
		public ArithmeticParameter ArithmeticParameter { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }
		public Action UpdateVariableTypeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }

		public ArithmeticParameterViewModel(ArithmeticParameter arithmeticParameter, bool allowImplicitValue = true)
		{
			var availableVariableTypes = new List<VariableType>();
			availableVariableTypes.Add(VariableType.IsGlobalVariable);
			availableVariableTypes.Add(VariableType.IsLocalVariable);
			if (allowImplicitValue)
			{
				availableVariableTypes.Add(VariableType.IsValue);
			}

			ArithmeticParameter = arithmeticParameter;
			VariableTypes = new ObservableCollection<VariableType>(availableVariableTypes);
			OnPropertyChanged(() => VariableTypes);
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

		public ObservableCollection<VariableType> VariableTypes { get; set; }

		public VariableType SelectedVariableType
		{
			get { return ArithmeticParameter.VariableType; }
			set
			{
				ArithmeticParameter.VariableType = value;
				if (UpdateVariableTypeHandler != null)
					UpdateVariableTypeHandler();
				OnPropertyChanged(() => SelectedVariableType);
			}
		}

		public bool BoolValue
		{
			get { return ArithmeticParameter.BoolValue; }
			set
			{
				ArithmeticParameter.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		public DateTime DateTimeValue
		{
			get { return ArithmeticParameter.DateTimeValue; }
			set
			{
				ArithmeticParameter.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return ArithmeticParameter.IntValue; }
			set
			{
				ArithmeticParameter.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public string StringValue
		{
			get { return ArithmeticParameter.StringValue; }
			set
			{
				ArithmeticParameter.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public Guid UidValue
		{
			get { return ArithmeticParameter.UidValue; }
			set
			{
				ArithmeticParameter.UidValue = value;
				OnPropertyChanged(() => UidValue);
			}
		}

		public string TypeValue
		{
			get { return ArithmeticParameter.TypeValue; }
			set
			{
				ArithmeticParameter.TypeValue = value;
				OnPropertyChanged(() => TypeValue);
			}
		}

		List<VariableViewModel> Variables { get; set; }

		public ObservableCollection<VariableViewModel> LocalVariables 
		{ 
			get { return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => !x.IsGlobal)); }
		}

		public ObservableCollection<VariableViewModel> GlobalVariables
		{
			get { return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => x.IsGlobal)); }
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
					if (UpdateVariableHandler != null)
						UpdateVariableHandler();
				}
				else
				{
					ArithmeticParameter.VariableUid = Guid.Empty;
				}
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			ServiceFactory.SaveService.AutomationChanged = true;
			base.OnPropertyChanged(propertyExpression);
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
		}
	}
}