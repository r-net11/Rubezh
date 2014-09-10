using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GetObjectFieldStepViewModel: BaseStepViewModel
	{
		GetObjectFieldArguments GetObjectFieldArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }
		public ArithmeticParameterViewModel Result { get; private set; }

		public GetObjectFieldStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetObjectFieldArguments = stepViewModel.Step.GetObjectFieldArguments;
			Variable1 = new ArithmeticParameterViewModel(GetObjectFieldArguments.Variable1, stepViewModel.Update, false);
			Variable1.UpdateVariableHandler += UpdateProperies;
			Result = new ArithmeticParameterViewModel(GetObjectFieldArguments.Result, stepViewModel.Update, false);
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
			get { return GetObjectFieldArguments.Property; }
			set
			{
				GetObjectFieldArguments.Property = value;
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
				return Result.Description + " = " + Variable1.Description + "." + SelectedProperty; 
			}
		}

		public EnumType EnumType
		{
			get
			{
				if (SelectedProperty == Property.Type)
					return EnumType.DeviceType;
				if (SelectedProperty == Property.DeviceState)
					return EnumType.StateClass;
				return EnumType.StateClass;
			}
		}

		public ExplicitType ExplicitType
		{
			get
			{
				if (SelectedProperty == Property.Description)
					return ExplicitType.String;
				if ((SelectedProperty == Property.Type) || (SelectedProperty == Property.DeviceState))
					return ExplicitType.Enum;
				return ExplicitType.Integer;
			}
		}
	}
}