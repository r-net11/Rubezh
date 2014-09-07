using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class SetValueStepViewModel : BaseViewModel, IStepViewModel
	{
		SetValueArguments SetValueArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }
		public ArithmeticParameterViewModel Result { get; private set; }
		Procedure Procedure { get; set; }

		public SetValueStepViewModel(SetValueArguments setValueArguments, Procedure procedure)
		{
			SetValueArguments = setValueArguments;
			Procedure = procedure;
			Variable1 = new ArithmeticParameterViewModel(SetValueArguments.Variable1);
			Result = new ArithmeticParameterViewModel(SetValueArguments.Result, false);
			ValueTypes = new ObservableCollection<ValueType>(Enum.GetValues(typeof(ValueType)).Cast<ValueType>().ToList().FindAll(x => x != ValueType.Object));
			SelectedValueType = SetValueArguments.ValueType;
			UpdateContent();
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }

		public ValueType SelectedValueType
		{
			get { return SetValueArguments.ValueType; }
			set
			{
				SetValueArguments.ValueType = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedValueType);
				UpdateContent();
			}
		}

		public void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure);
			allVariables = allVariables.FindAll(x => !x.IsList && x.ValueType == SelectedValueType);
			Variable1.Update(allVariables);
			Result.Update(allVariables);
		}

		public string Description { get { return ""; } }
	}
}