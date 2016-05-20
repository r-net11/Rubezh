using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class SendEmailStepViewModel : BaseStepViewModel
	{
		SendEmailStep SendEmailStep { get; set; }
		public ArgumentViewModel EMailAddressFromArgument { get; private set; }
		public ArgumentViewModel EMailAddressToArgument { get; private set; }
		public ArgumentViewModel EMailTitleArgument { get; private set; }
		public ArgumentViewModel EMailContentArgument { get; private set; }
		public ArgumentViewModel SmtpArgument { get; private set; }
		public ArgumentViewModel PortArgument { get; private set; }
		public ArgumentViewModel LoginArgument { get; private set; }
		public ArgumentViewModel PasswordArgument { get; private set; }

		public SendEmailStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			SendEmailStep = (SendEmailStep)stepViewModel.Step;
			EMailAddressFromArgument = new ArgumentViewModel(SendEmailStep.EMailAddressFromArgument, stepViewModel.Update, UpdateContent);
			EMailAddressToArgument = new ArgumentViewModel(SendEmailStep.EMailAddressToArgument, stepViewModel.Update, UpdateContent);
			EMailTitleArgument = new ArgumentViewModel(SendEmailStep.EMailTitleArgument, stepViewModel.Update, UpdateContent);
			EMailContentArgument = new ArgumentViewModel(SendEmailStep.EMailContentArgument, stepViewModel.Update, UpdateContent);
			SmtpArgument = new ArgumentViewModel(SendEmailStep.SmtpArgument, stepViewModel.Update, UpdateContent);
			PortArgument = new ArgumentViewModel(SendEmailStep.PortArgument, stepViewModel.Update, UpdateContent);
			LoginArgument = new ArgumentViewModel(SendEmailStep.LoginArgument, stepViewModel.Update, UpdateContent);
			PasswordArgument = new ArgumentViewModel(SendEmailStep.PasswordArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			EMailAddressFromArgument.Update(Procedure, ExplicitType.String, isList: false);
			EMailAddressToArgument.Update(Procedure, ExplicitType.String, isList: false);
			EMailTitleArgument.Update(Procedure, ExplicitType.String, isList: false);
			EMailContentArgument.Update(Procedure, ExplicitType.String, isList: false);
			SmtpArgument.Update(Procedure, ExplicitType.String, isList: false);
			PortArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			LoginArgument.Update(Procedure, ExplicitType.String, isList: false);
			PasswordArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				return "От кого: " + EMailAddressFromArgument.Description + " Кому: " + EMailAddressToArgument.Description + " Заголовок: " + EMailTitleArgument.Description + " Текст: " + EMailContentArgument.Description;
			}
		}
	}
}
