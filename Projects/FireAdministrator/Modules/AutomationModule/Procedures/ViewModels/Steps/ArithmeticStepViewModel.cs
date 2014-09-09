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
using FiresecAPI.GK;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ArithmeticStepViewModel : BaseStepViewModel
	{
		public ArithmeticArguments ArithmeticArguments { get; private set; }
		public Procedure Procedure { get; private set; }
		public ArithmeticParameterViewModel Variable1 { get; set; }
		public ArithmeticParameterViewModel Variable2 { get; set; }
		public ArithmeticParameterViewModel Result { get; set; }

		public ArithmeticStepViewModel(ArithmeticArguments arithmeticArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			Procedure = procedure;
			ArithmeticArguments = arithmeticArguments;
			Result = new ArithmeticParameterViewModel(ArithmeticArguments.Result, false);
			Variable1 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable1);
			Variable2 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable2);
			ValueTypes = new ObservableCollection<ValueType>(ProcedureHelper.GetEnumList<ValueType>().FindAll(x => x != ValueType.Object && x != ValueType.Enum));
			TimeTypes = ProcedureHelper.GetEnumObs<TimeType>();
			Variable1.UpdateDescriptionHandler = updateDescriptionHandler;
			Variable2.UpdateDescriptionHandler = updateDescriptionHandler;
			Result.UpdateDescriptionHandler = updateDescriptionHandler;
			SelectedValueType = ArithmeticArguments.ValueType;
		}

		public override void UpdateContent()
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

		public override string Description
		{
			get
			{
				var var1 = "пусто";
				var var2 = "пусто";
				var res = "пусто";
				switch(SelectedValueType)
				{
					case ValueType.Boolean:
						var1 = Variable1.SelectedVariableType == VariableType.IsValue ? Variable1.CurrentVariableItem.VariableItem.BoolValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.CurrentVariableItem.VariableItem.BoolValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ValueType.DateTime:
						var1 = Variable1.SelectedVariableType == VariableType.IsValue ? Variable1.CurrentVariableItem.VariableItem.DateTimeValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.CurrentVariableItem.VariableItem.IntValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						var2 = var2 + " " + SelectedTimeType.ToDescription();;
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ValueType.Integer:
						var1 = Variable1.SelectedVariableType == VariableType.IsValue ? Variable1.CurrentVariableItem.VariableItem.IntValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.CurrentVariableItem.VariableItem.IntValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ValueType.String:
						var1 = Variable1.SelectedVariableType == VariableType.IsValue ? Variable1.CurrentVariableItem.VariableItem.StringValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.CurrentVariableItem.VariableItem.StringValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
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
				Variable1.ValueType = value;
				Variable2.ValueType = value == ValueType.DateTime ? ValueType.Integer : value;
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
	}
}