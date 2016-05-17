using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class GetListCountStepViewModel : BaseStepViewModel
	{
		GetListCountStep GetListCountStep { get; set; }
		public ArgumentViewModel ListArgument { get; set; }
		public ArgumentViewModel CountArgument { get; set; }

		public GetListCountStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			GetListCountStep = (GetListCountStep)stepViewModel.Step;
			ListArgument = new ArgumentViewModel(GetListCountStep.ListArgument, stepViewModel.Update, UpdateContent, false);
			CountArgument = new ArgumentViewModel(GetListCountStep.CountArgument, stepViewModel.Update, UpdateContent, false);
			CountArgument.ExplicitType = ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			ListArgument.Update(Procedure, isList: true);
			CountArgument.Update(Procedure, ExplicitType.Integer, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Список: " + ListArgument.Description + " Размер: " + CountArgument.Description;
			}
		}
	}
}