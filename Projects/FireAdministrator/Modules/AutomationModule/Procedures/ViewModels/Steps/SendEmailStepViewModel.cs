using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System;
using System.Collections.ObjectModel;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class SendEmailStepViewModel : BaseViewModel, IStepViewModel
	{
		SendEmailArguments SendEmailArguments { get; set; }
		public Action UpdateDescriptionHandler { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public SendEmailStepViewModel(SendEmailArguments sendEmailArguments, Procedure procedure, Action updateDescriptionHandler)
		{
			SendEmailArguments = sendEmailArguments;
			UpdateDescriptionHandler = updateDescriptionHandler;
			Procedure = procedure;
			Variable1 = new ArithmeticParameterViewModel(SendEmailArguments.Variable1, ProcedureHelper.GetEnumList<VariableType>());
			ValueTypes = new ObservableCollection<ValueType>(ProcedureHelper.GetEnumList<ValueType>().FindAll(x => x != ValueType.Object));
			UpdateContent();
		}

		VariableItemViewModel _selectedVariableItem;
		public VariableItemViewModel SelectedVariableItem
		{
			get { return _selectedVariableItem; }
			set
			{
				_selectedVariableItem = value;
				if (value != null)
					Variable1.UidValue = _selectedVariableItem.VariableItem.ObjectUid;
				OnPropertyChanged(() => SelectedVariableItem);
			}
		}

		public string Email
		{
			get { return SendEmailArguments.Email; }
			set
			{
				SendEmailArguments.Email = value;
				OnPropertyChanged(() => Email);
			}
		}

		public void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType && !x.IsList));
		}

		public string Description
		{
			get
			{
				if (Variable1.SelectedVariableType == VariableType.IsValue && Variable1.SelectedVariable != null)
					return "<" + Variable1.SelectedVariable.Name + ">";
				else if (Variable1.SelectedVariable != null)
					return "<" + Variable1.SelectedVariable.Name + ">";
				return "";
			}
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		public ValueType ValueType
		{
			get
			{
				return SendEmailArguments.ValueType;
			}
			set
			{
				SendEmailArguments.ValueType = value;
				UpdateContent();
				OnPropertyChanged(() => ValueType);
			}
		}
	}
}
