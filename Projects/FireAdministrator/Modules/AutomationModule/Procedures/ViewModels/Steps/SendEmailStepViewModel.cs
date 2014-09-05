using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System;
using System.Collections.ObjectModel;
using ValueType = FiresecAPI.Automation.ValueType;
using Infrastructure;
using System.Linq;

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
			EMailTitleValueTypes = new ObservableCollection<ValueType>(ProcedureHelper.GetEnumList<ValueType>().FindAll(x => x != ValueType.Object && x != ValueType.Enum));
			EMailContentValueTypes = new ObservableCollection<ValueType>(ProcedureHelper.GetEnumList<ValueType>().FindAll(x => x != ValueType.Object && x != ValueType.Enum));
			EMailAddress = new ArithmeticParameterViewModel(SendEmailArguments.EMailAddress, ProcedureHelper.GetEnumList<VariableType>());
			EMailTitle = new ArithmeticParameterViewModel(SendEmailArguments.EMailTitle, ProcedureHelper.GetEnumList<VariableType>());
			EMailContent = new ArithmeticParameterViewModel(SendEmailArguments.EMailContent, ProcedureHelper.GetEnumList<VariableType>());
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
				ServiceFactory.SaveService.AutomationChanged = true;
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
				ServiceFactory.SaveService.AutomationChanged = true;
			}
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

		public void UpdateContent()
		{
			var allEMailTitleVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == SelectedEMailTitleValueType);
			var allEMailContentVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == SelectedEMailContentValueType);
			EMailAddress.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.String && !x.IsList));
			EMailTitle.Update(allEMailTitleVariables);
			EMailContent.Update(allEMailContentVariables);
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
