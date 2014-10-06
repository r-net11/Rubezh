using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class SendEmailStepViewModel : BaseStepViewModel
	{
		SendEmailArguments SendEmailArguments { get; set; }
		public ArgumentViewModel EMailAddressArgument { get; private set; }
		public ArgumentViewModel EMailTitleArgument { get; private set; }
		public ArgumentViewModel EMailContentArgument { get; private set; }
		public ArgumentViewModel HostArgument { get; private set; }
		public ArgumentViewModel PortArgument { get; private set; }
		public ArgumentViewModel LoginArgument { get; private set; }
		public ArgumentViewModel PasswordArgument { get; private set; }

		public SendEmailStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SendEmailArguments = stepViewModel.Step.SendEmailArguments;
			EMailAddressArgument = new ArgumentViewModel(SendEmailArguments.EMailAddressArgument, stepViewModel.Update);
			EMailAddressArgument.ExplicitType = ExplicitType.String;
			EMailTitleArgument = new ArgumentViewModel(SendEmailArguments.EMailTitleArgument, stepViewModel.Update);
			EMailTitleArgument.ExplicitType = ExplicitType.String;
			EMailContentArgument = new ArgumentViewModel(SendEmailArguments.EMailContentArgument, stepViewModel.Update);
			EMailContentArgument.ExplicitType = ExplicitType.String;
			HostArgument = new ArgumentViewModel(SendEmailArguments.HostArgument, stepViewModel.Update);
			HostArgument.ExplicitType = ExplicitType.String;
			PortArgument = new ArgumentViewModel(SendEmailArguments.PortArgument, stepViewModel.Update);
			PortArgument.ExplicitType = ExplicitType.Integer;
			LoginArgument = new ArgumentViewModel(SendEmailArguments.LoginArgument, stepViewModel.Update);
			LoginArgument.ExplicitType = ExplicitType.String;
			PasswordArgument = new ArgumentViewModel(SendEmailArguments.PasswordArgument, stepViewModel.Update);
			PasswordArgument.ExplicitType = ExplicitType.String;
		}

		public override void UpdateContent()
		{
			EMailAddressArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			EMailTitleArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			EMailContentArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			HostArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			PortArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
			LoginArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			PasswordArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
		}

		public override string Description
		{
			get
			{
				return "Email: " + EMailAddressArgument.Description + " Заголовок: " + EMailTitleArgument.Description + " Текст: " + EMailContentArgument.Description;
			}
		}
	}
}
