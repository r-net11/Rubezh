using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class NowStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ResultArgument { get; private set; }
		public NowStep NowStep { get; private set; }

		public NowStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			NowStep = (NowStep)stepViewModel.Step;
			ResultArgument = new ArgumentViewModel(NowStep.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			ResultArgument.Update(Procedure, ExplicitType.DateTime, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Результат: " + ResultArgument.Description;
			}
		}
	}
}
