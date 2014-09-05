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
		public ArithmeticParameterViewModel EMailAddress { get; private set; }
		public ArithmeticParameterViewModel EMailTitle { get; private set; }
		public ArithmeticParameterViewModel EMailContent { get; private set; }

		public SendEmailStepViewModel(SendEmailArguments sendEmailArguments, Procedure procedure, Action updateDescriptionHandler)
		{
			SendEmailArguments = sendEmailArguments;
			UpdateDescriptionHandler = updateDescriptionHandler;
			Procedure = procedure;
			EMailAddress = new ArithmeticParameterViewModel(SendEmailArguments.EMailAddress, ProcedureHelper.GetEnumList<VariableType>());
			EMailTitle = new ArithmeticParameterViewModel(SendEmailArguments.EMailTitle, ProcedureHelper.GetEnumList<VariableType>());
			EMailContent = new ArithmeticParameterViewModel(SendEmailArguments.EMailContent, ProcedureHelper.GetEnumList<VariableType>());
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
					EMailContent.UidValue = _selectedVariableItem.VariableItem.ObjectUid;
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

		public string Host
		{
			get { return SendEmailArguments.Host; }
			set
			{
				SendEmailArguments.Host = value;
				OnPropertyChanged(() => Host);
			}
		}

		public string Port
		{
			get { return SendEmailArguments.Port; }
			set
			{
				SendEmailArguments.Port = value;
				OnPropertyChanged(() => Port);
			}
		}

		public string UserName
		{
			get { return SendEmailArguments.UserName; }
			set
			{
				SendEmailArguments.UserName = value;
				OnPropertyChanged(() => UserName);
			}
		}

		public string Password
		{
			get { return SendEmailArguments.Password; }
			set
			{
				SendEmailArguments.Password = value;
				OnPropertyChanged(() => Password);
			}
		}

		public void UpdateContent()
		{
			EMailAddress.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.String && !x.IsList));
			EMailTitle.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList));
			EMailContent.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList));
		}

		public string Description
		{
			get
			{
				if (EMailContent.SelectedVariableType == VariableType.IsValue && EMailContent.SelectedVariable != null)
					return "<" + EMailContent.SelectedVariable.Name + ">";
				else if (EMailContent.SelectedVariable != null)
					return "<" + EMailContent.SelectedVariable.Name + ">";
				return "";
			}
		}
	}
}
