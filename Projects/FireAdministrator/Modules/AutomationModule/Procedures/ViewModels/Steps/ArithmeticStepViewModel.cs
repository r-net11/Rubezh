using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ArithmeticStepViewModel : BaseStepViewModel
	{
		public ArithmeticArguments ArithmeticArguments { get; private set; }
		public ArithmeticParameterViewModel Variable1 { get; set; }
		public ArithmeticParameterViewModel Variable2 { get; set; }
		public ArithmeticParameterViewModel Result { get; set; }

		public ArithmeticStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ArithmeticArguments = stepViewModel.Step.ArithmeticArguments;
			Result = new ArithmeticParameterViewModel(ArithmeticArguments.Result, stepViewModel.Update, false);
			Variable1 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable1, stepViewModel.Update);
			Variable2 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable2, stepViewModel.Update);
			ExplicitTypes = new ObservableCollection<ExplicitType>(ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object && x != ExplicitType.Enum));
			TimeTypes = ProcedureHelper.GetEnumObs<TimeType>();
			SelectedExplicitType = ArithmeticArguments.ExplicitType;
		}

		public override void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == SelectedExplicitType);
			var allVariables2 = new List<Variable>(allVariables);
			if (SelectedExplicitType == ExplicitType.DateTime)
				allVariables2 = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == ExplicitType.Integer);
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
				switch(SelectedExplicitType)
				{
					case ExplicitType.Boolean:
						var1 = Variable1.SelectedVariableScope == VariableScope.ExplicitValue ? Variable1.CurrentVariableItem.VariableItem.BoolValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableScope == VariableScope.ExplicitValue ? Variable2.CurrentVariableItem.VariableItem.BoolValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ExplicitType.DateTime:
						var1 = Variable1.SelectedVariableScope == VariableScope.ExplicitValue ? Variable1.CurrentVariableItem.VariableItem.DateTimeValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableScope == VariableScope.ExplicitValue ? Variable2.CurrentVariableItem.VariableItem.IntValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						var2 = var2 + " " + SelectedTimeType.ToDescription();;
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ExplicitType.Integer:
						var1 = Variable1.SelectedVariableScope == VariableScope.ExplicitValue ? Variable1.CurrentVariableItem.VariableItem.IntValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableScope == VariableScope.ExplicitValue ? Variable2.CurrentVariableItem.VariableItem.IntValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						res = Result.SelectedVariable != null ? Result.SelectedVariable.Name : "пусто";
						break;
					case ExplicitType.String:
						var1 = Variable1.SelectedVariableScope == VariableScope.ExplicitValue ? Variable1.CurrentVariableItem.VariableItem.StringValue.ToString() : (Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто");
						var2 = Variable2.SelectedVariableScope == VariableScope.ExplicitValue ? Variable2.CurrentVariableItem.VariableItem.StringValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
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

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType SelectedExplicitType
		{
			get { return ArithmeticArguments.ExplicitType; }
			set
			{
				ArithmeticArguments.ExplicitType = value;
				Variable1.ExplicitType = value;
				Variable2.ExplicitType = value == ExplicitType.DateTime ? ExplicitType.Integer : value;
				ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType>();
				if (value == ExplicitType.Boolean)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.And, ArithmeticOperationType.Or };
				if (value == ExplicitType.DateTime)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add, ArithmeticOperationType.Sub };
				if (value == ExplicitType.String)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add };
				if (value == ExplicitType.Integer)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add, ArithmeticOperationType.Sub, ArithmeticOperationType.Multi, ArithmeticOperationType.Div};
				OnPropertyChanged(() => ArithmeticOperationTypes);
				OnPropertyChanged(() => SelectedExplicitType);
				UpdateContent();
			}
		}
	}
}