using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class GetObjectPropertyStepViewModel: BaseStepViewModel
	{
		GetObjectPropertyArguments GetObjectPropertyArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }
		public ArithmeticParameterViewModel Result { get; private set; }

		public GetObjectPropertyStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetObjectPropertyArguments = stepViewModel.Step.GetObjectPropertyArguments;
			Variable1 = new ArithmeticParameterViewModel(GetObjectPropertyArguments.Variable1, stepViewModel.Update, false);
			Variable1.UpdateVariableHandler += UpdateProperies;
			Result = new ArithmeticParameterViewModel(GetObjectPropertyArguments.Result, stepViewModel.Update, false);
			UpdateContent();
		}

		void UpdateProperies()
		{
			Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(Variable1.SelectedVariable.Variable.ObjectType));
			OnPropertyChanged(() => Properties);
		}

		public ObservableCollection<Property> Properties { get; private set; }
		public Property SelectedProperty
		{
			get { return GetObjectPropertyArguments.Property; }
			set
			{
				GetObjectPropertyArguments.Property = value;
				OnPropertyChanged(() => SelectedProperty);
				Result.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType && x.EnumType == EnumType && !x.IsList));
			}
		}

		public override void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Object && !x.IsList));
			Result.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType && x.EnumType == EnumType && !x.IsList));
		}

		public override string Description
		{
			get 
			{ 
				return Result.Description + " = " + Variable1.Description + " Свойство: " + SelectedProperty.ToDescription(); 
			}
		}

		public EnumType EnumType
		{
			get
			{
				if (SelectedProperty == Property.Type)
					return EnumType.DriverType;
				if (SelectedProperty == Property.State)
					return EnumType.StateType;
				return EnumType.StateType;
			}
		}

		public ExplicitType ExplicitType
		{
			get
			{
				if (SelectedProperty == Property.Description)
					return ExplicitType.String;
				if ((SelectedProperty == Property.Type) || (SelectedProperty == Property.State))
					return ExplicitType.Enum;
				return ExplicitType.Integer;
			}
		}
	}
}