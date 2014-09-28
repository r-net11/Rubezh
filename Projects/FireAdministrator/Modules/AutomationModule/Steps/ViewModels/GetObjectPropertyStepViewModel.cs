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
		public ArgumentViewModel ObjectParameter { get; private set; }
		public ArgumentViewModel ResultParameter { get; private set; }

		public GetObjectPropertyStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetObjectPropertyArguments = stepViewModel.Step.GetObjectPropertyArguments;
			ObjectParameter = new ArgumentViewModel(GetObjectPropertyArguments.ObjectParameter, stepViewModel.Update, false);
			ObjectParameter.UpdateVariableHandler += UpdateProperies;
			ResultParameter = new ArgumentViewModel(GetObjectPropertyArguments.ResultParameter, stepViewModel.Update, false);
		}

		void UpdateProperies()
		{
			Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(ObjectParameter.SelectedVariable.Variable.ObjectType));
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
				ResultParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType && x.EnumType == EnumType && !x.IsList));
			}
		}

		public override void UpdateContent()
		{
			ObjectParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Object && !x.IsList));
			ResultParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType && x.EnumType == EnumType && !x.IsList));
		}

		public override string Description
		{
			get 
			{ 
				return ResultParameter.Description + " = " + ObjectParameter.Description + " Свойство: " + SelectedProperty.ToDescription(); 
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