using Infrastructure.Automation;
using RubezhAPI.Automation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ArithmeticStepViewModel : BaseStepViewModel
	{
		public ArithmeticArguments ArithmeticArguments { get; private set; }
		public ArgumentViewModel Argument1 { get; set; }
		public ArgumentViewModel Argument2 { get; set; }
		public ArgumentViewModel ResultArgument { get; set; }

		public ArithmeticStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ArithmeticArguments = stepViewModel.Step.ArithmeticArguments;
			ResultArgument = new ArgumentViewModel(ArithmeticArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
			Argument1 = new ArgumentViewModel(ArithmeticArguments.Argument1, stepViewModel.Update, UpdateContent);
			Argument2 = new ArgumentViewModel(ArithmeticArguments.Argument2, stepViewModel.Update, UpdateContent);
			ExplicitTypes = new ObservableCollection<ExplicitType>(AutomationHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object && x != ExplicitType.Enum));
			TimeTypes = AutomationHelper.GetEnumObs<TimeType>();
			SelectedExplicitType = ArithmeticArguments.ExplicitType;
		}

		public override void UpdateContent()
		{
			switch (SelectedExplicitType)
			{
				case ExplicitType.DateTime:
					Argument1.Update(Procedure, SelectedExplicitType, isList: false);
					Argument2.Update(Procedure, ExplicitType.Integer, isList: false);
					break;
				case ExplicitType.String:
					Argument1.Update(Procedure, AutomationHelper.GetEnumList<ExplicitType>(), isList: false);
					Argument1.ExplicitType = ExplicitType.String;
					Argument2.Update(Procedure, AutomationHelper.GetEnumList<ExplicitType>(), isList: false);
					Argument2.ExplicitType = ExplicitType.String;
					break;
				case ExplicitType.Float:
					var explicitTypes = new List<ExplicitType> { ExplicitType.Integer, ExplicitType.Float };
					Argument1.Update(Procedure, explicitTypes, isList: false);
					Argument1.ExplicitType = ExplicitType.Float;
					Argument2.Update(Procedure, explicitTypes, isList: false);
					Argument2.ExplicitType = ExplicitType.Float;
					break;
				default:
					Argument1.Update(Procedure, SelectedExplicitType, isList: false);
					Argument2.Update(Procedure, SelectedExplicitType, isList: false);
					break;
			}
			ResultArgument.Update(Procedure, SelectedExplicitType, isList: false);
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
				ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType>();
				if (value == ExplicitType.Boolean)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.And, ArithmeticOperationType.Or };
				if (value == ExplicitType.DateTime)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add, ArithmeticOperationType.Sub };
				if (value == ExplicitType.String)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add };
				if (value == ExplicitType.Integer || value == ExplicitType.Float)
					ArithmeticOperationTypes = new ObservableCollection<ArithmeticOperationType> { ArithmeticOperationType.Add, ArithmeticOperationType.Sub, ArithmeticOperationType.Multi, ArithmeticOperationType.Div };
				OnPropertyChanged(() => ArithmeticOperationTypes);
				OnPropertyChanged(() => SelectedExplicitType);
				UpdateContent();
			}
		}
	}
}