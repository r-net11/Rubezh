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
	public class GetObjectFieldStepViewModel: BaseViewModel, IStepViewModel
	{
		GetObjectFieldArguments GetObjectFieldArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }
		public ArithmeticParameterViewModel Result { get; private set; }

		public GetObjectFieldStepViewModel(GetObjectFieldArguments getObjectFieldArguments, Procedure procedure)
		{
			GetObjectFieldArguments = getObjectFieldArguments;
			Procedure = procedure;
			Variable1 = new ArithmeticParameterViewModel(getObjectFieldArguments.Variable1, false);
			Variable1.UpdateVariableHandler += UpdateProperies;
			Result = new ArithmeticParameterViewModel(getObjectFieldArguments.Result, false);
			UpdateContent();
		}

		void UpdateProperies()
		{
			Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(Variable1.SelectedVariable.ObjectType));
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
				Result.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType && !x.IsList));
			}
		}

		public void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Object && !x.IsList));
			Result.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType && !x.IsList));
		}

		public string Description
		{
			get { return ""; }
		}

		public ValueType ValueType
		{
			get
			{
				if (SelectedProperty == Property.Description)
					return ValueType.String;
				if (SelectedProperty == Property.Type)
					return ValueType.Object;
				return ValueType.Integer;
			}
		}
	}
}