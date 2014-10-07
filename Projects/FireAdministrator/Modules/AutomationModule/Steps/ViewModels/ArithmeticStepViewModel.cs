using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ArithmeticStepViewModel : BaseStepViewModel
	{
		public ArithmeticArguments ArithmeticArguments { get; private set; }
		public ArgumentViewModel Argument1 { get; set; }
		public ArgumentViewModel Argument2 { get; set; }
		public ArgumentViewModel ResultArgument { get; set; }

		public ArithmeticStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ArithmeticArguments = stepViewModel.Step.ArithmeticArguments;
			ResultArgument = new ArgumentViewModel(ArithmeticArguments.ResultArgument, stepViewModel.Update, false);
			Argument1 = new ArgumentViewModel(ArithmeticArguments.Argument1, stepViewModel.Update);
			Argument2 = new ArgumentViewModel(ArithmeticArguments.Argument2, stepViewModel.Update);
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
			Argument1.Update(allVariables);
			Argument2.Update(allVariables2);
			ResultArgument.Update(allVariables);
			SelectedArithmeticOperationType = ArithmeticOperationTypes.Contains(ArithmeticArguments.ArithmeticOperationType) ? ArithmeticArguments.ArithmeticOperationType : ArithmeticOperationTypes.FirstOrDefault();
		}

		public override string Description
		{
			get
			{
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

				return ResultArgument.Description + " = " + Argument1.Description + " " + op + " " + Argument2.Description;
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
				Argument1.ExplicitType = value;
				Argument2.ExplicitType = value == ExplicitType.DateTime ? ExplicitType.Integer : value;
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