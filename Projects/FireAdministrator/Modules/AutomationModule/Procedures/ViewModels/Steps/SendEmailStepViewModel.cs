using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System;
using System.Collections.ObjectModel;
using Infrastructure;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class SendEmailStepViewModel : BaseStepViewModel
	{
		SendEmailArguments SendEmailArguments { get; set; }
		public ArithmeticParameterViewModel EMailAddressParameter { get; private set; }
		public ArithmeticParameterViewModel EMailTitleParameter { get; private set; }
		public ArithmeticParameterViewModel EMailContentParameter { get; private set; }
		public ArithmeticParameterViewModel HostParameter { get; private set; }
		public ArithmeticParameterViewModel PortParameter { get; private set; }
		public ArithmeticParameterViewModel LoginParameter { get; private set; }
		public ArithmeticParameterViewModel PasswordParameter { get; private set; }

		public SendEmailStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SendEmailArguments = stepViewModel.Step.SendEmailArguments;
			EMailAddressParameter = new ArithmeticParameterViewModel(SendEmailArguments.EMailAddressParameter, stepViewModel.Update);
			EMailAddressParameter.ExplicitType = ExplicitType.String;
			EMailTitleParameter = new ArithmeticParameterViewModel(SendEmailArguments.EMailTitleParameter, stepViewModel.Update);
			EMailTitleParameter.ExplicitType = ExplicitType.String;
			EMailContentParameter = new ArithmeticParameterViewModel(SendEmailArguments.EMailContentParameter, stepViewModel.Update);
			EMailContentParameter.ExplicitType = ExplicitType.String;
			HostParameter = new ArithmeticParameterViewModel(SendEmailArguments.HostParameter, stepViewModel.Update);
			HostParameter.ExplicitType = ExplicitType.String;
			PortParameter = new ArithmeticParameterViewModel(SendEmailArguments.PortParameter, stepViewModel.Update);
			PortParameter.ExplicitType = ExplicitType.Integer;
			LoginParameter = new ArithmeticParameterViewModel(SendEmailArguments.LoginParameter, stepViewModel.Update);
			LoginParameter.ExplicitType = ExplicitType.String;
			PasswordParameter = new ArithmeticParameterViewModel(SendEmailArguments.PasswordParameter, stepViewModel.Update);
			PasswordParameter.ExplicitType = ExplicitType.String;
			UpdateContent();
		}

		public override void UpdateContent()
		{
			EMailAddressParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			EMailTitleParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			EMailContentParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			HostParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			PortParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
			LoginParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			PasswordParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
		}

		public override string Description
		{
			get
			{
				return "Email: " + EMailAddressParameter.Description + " Заголовок: " + EMailTitleParameter.Description + " Текст: " + EMailContentParameter.Description;
			}
		}
	}
}
