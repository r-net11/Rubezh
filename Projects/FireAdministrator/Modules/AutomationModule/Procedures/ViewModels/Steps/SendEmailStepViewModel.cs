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
		public ArgumentViewModel EMailAddressParameter { get; private set; }
		public ArgumentViewModel EMailTitleParameter { get; private set; }
		public ArgumentViewModel EMailContentParameter { get; private set; }
		public ArgumentViewModel HostParameter { get; private set; }
		public ArgumentViewModel PortParameter { get; private set; }
		public ArgumentViewModel LoginParameter { get; private set; }
		public ArgumentViewModel PasswordParameter { get; private set; }

		public SendEmailStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SendEmailArguments = stepViewModel.Step.SendEmailArguments;
			EMailAddressParameter = new ArgumentViewModel(SendEmailArguments.EMailAddressParameter, stepViewModel.Update);
			EMailAddressParameter.ExplicitType = ExplicitType.String;
			EMailTitleParameter = new ArgumentViewModel(SendEmailArguments.EMailTitleParameter, stepViewModel.Update);
			EMailTitleParameter.ExplicitType = ExplicitType.String;
			EMailContentParameter = new ArgumentViewModel(SendEmailArguments.EMailContentParameter, stepViewModel.Update);
			EMailContentParameter.ExplicitType = ExplicitType.String;
			HostParameter = new ArgumentViewModel(SendEmailArguments.HostParameter, stepViewModel.Update);
			HostParameter.ExplicitType = ExplicitType.String;
			PortParameter = new ArgumentViewModel(SendEmailArguments.PortParameter, stepViewModel.Update);
			PortParameter.ExplicitType = ExplicitType.Integer;
			LoginParameter = new ArgumentViewModel(SendEmailArguments.LoginParameter, stepViewModel.Update);
			LoginParameter.ExplicitType = ExplicitType.String;
			PasswordParameter = new ArgumentViewModel(SendEmailArguments.PasswordParameter, stepViewModel.Update);
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
