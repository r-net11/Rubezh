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
		public ArithmeticParameterViewModel EMailAddress { get; private set; }
		public ArithmeticParameterViewModel EMailTitle { get; private set; }
		public ArithmeticParameterViewModel EMailContent { get; private set; }
		public ArithmeticParameterViewModel Host { get; private set; }
		public ArithmeticParameterViewModel Port { get; private set; }
		public ArithmeticParameterViewModel Login { get; private set; }
		public ArithmeticParameterViewModel Password { get; private set; }

		public SendEmailStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SendEmailArguments = stepViewModel.Step.SendEmailArguments;
			EMailAddress = new ArithmeticParameterViewModel(SendEmailArguments.EMailAddress, stepViewModel.Update);
			EMailAddress.ExplicitType = ExplicitType.String;
			EMailTitle = new ArithmeticParameterViewModel(SendEmailArguments.EMailTitle, stepViewModel.Update);
			EMailTitle.ExplicitType = ExplicitType.String;
			EMailContent = new ArithmeticParameterViewModel(SendEmailArguments.EMailContent, stepViewModel.Update);
			EMailContent.ExplicitType = ExplicitType.String;
			Host = new ArithmeticParameterViewModel(SendEmailArguments.Host, stepViewModel.Update);
			Host.ExplicitType = ExplicitType.String;
			Port = new ArithmeticParameterViewModel(SendEmailArguments.Port, stepViewModel.Update);
			Port.ExplicitType = ExplicitType.Integer;
			Login = new ArithmeticParameterViewModel(SendEmailArguments.Login, stepViewModel.Update);
			Login.ExplicitType = ExplicitType.String;
			Password = new ArithmeticParameterViewModel(SendEmailArguments.Password, stepViewModel.Update);
			Password.ExplicitType = ExplicitType.String;
			UpdateContent();
		}

		public override void UpdateContent()
		{
			EMailAddress.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			EMailTitle.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			EMailContent.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			Host.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			Port.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
			Login.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			Password.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
		}

		public override string Description
		{
			get
			{
				return "Email: " + EMailAddress.Description + " Заголовок: " + EMailTitle.Description + " Текст: " + EMailContent.Description;
			}
		}
	}
}
