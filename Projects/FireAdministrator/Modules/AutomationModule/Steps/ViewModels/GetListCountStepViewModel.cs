using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class GetListCountStepViewModel : BaseStepViewModel
	{
		GetListCountArgument GetListCountArgument { get; set; }
		public ArgumentViewModel ListArgument { get; set; }
		public ArgumentViewModel CountArgument { get; set; }

		public GetListCountStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetListCountArgument = stepViewModel.Step.GetListCountArgument;
			ListArgument = new ArgumentViewModel(GetListCountArgument.ListArgument, stepViewModel.Update, false);
			CountArgument = new ArgumentViewModel(GetListCountArgument.CountArgument, stepViewModel.Update, false);
			CountArgument.ExplicitType = ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			ListArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.IsList));
			CountArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
		}

		public override string Description
		{
			get
			{
				return "Список: " + ListArgument.Description + "Размер: " + CountArgument.Description;
			}
		}
	}
}