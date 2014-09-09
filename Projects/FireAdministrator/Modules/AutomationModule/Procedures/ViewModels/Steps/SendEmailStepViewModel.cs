using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System;
using System.Collections.ObjectModel;
using ValueType = FiresecAPI.Automation.ValueType;
using Infrastructure;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class SendEmailStepViewModel : BaseStepViewModel
	{
		SendEmailArguments SendEmailArguments { get; set; }
		public Action UpdateDescriptionHandler { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel EMailAddress { get; private set; }
		public ArithmeticParameterViewModel EMailTitle { get; private set; }
		public ArithmeticParameterViewModel EMailContent { get; private set; }

		public SendEmailStepViewModel(SendEmailArguments sendEmailArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			SendEmailArguments = sendEmailArguments;
			UpdateDescriptionHandler = updateDescriptionHandler;
			Procedure = procedure;
			EMailTitleValueTypes = new ObservableCollection<ValueType>(ProcedureHelper.GetEnumList<ValueType>().FindAll(x => x != ValueType.Object && x != ValueType.Enum));
			EMailContentValueTypes = new ObservableCollection<ValueType>(ProcedureHelper.GetEnumList<ValueType>().FindAll(x => x != ValueType.Object && x != ValueType.Enum));
			EMailAddress = new ArithmeticParameterViewModel(SendEmailArguments.EMailAddress);
			EMailTitle = new ArithmeticParameterViewModel(SendEmailArguments.EMailTitle);
			EMailContent = new ArithmeticParameterViewModel(SendEmailArguments.EMailContent);
			UpdateContent();
		}

		public ObservableCollection<ValueType> EMailTitleValueTypes { get; private set; }
		public ValueType SelectedEMailTitleValueType
		{
			get { return SendEmailArguments.SelectedEMailTitleValueType; }
			set
			{
				SendEmailArguments.SelectedEMailTitleValueType = value;
				OnPropertyChanged(() => SelectedEMailTitleValueType);
				UpdateContent();
			}
		}

		public ObservableCollection<ValueType> EMailContentValueTypes { get; private set; }
		public ValueType SelectedEMailContentValueType
		{
			get { return SendEmailArguments.SelectedEMailContentValueType; }
			set
			{
				SendEmailArguments.SelectedEMailContentValueType = value;
				OnPropertyChanged(() => SelectedEMailContentValueType);
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

		public string Login
		{
			get { return SendEmailArguments.Login; }
			set
			{
				SendEmailArguments.Login = value;
				OnPropertyChanged(() => Login);
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

		public override void UpdateContent()
		{
			var allEMailTitleVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == SelectedEMailTitleValueType);
			var allEMailContentVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == SelectedEMailContentValueType);
			EMailAddress.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.String && !x.IsList));
			EMailTitle.Update(allEMailTitleVariables);
			EMailContent.Update(allEMailContentVariables);
		}

		public override string Description
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
