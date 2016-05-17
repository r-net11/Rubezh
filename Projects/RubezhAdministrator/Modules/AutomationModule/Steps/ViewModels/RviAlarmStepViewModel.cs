using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class RviAlarmStepViewModel : BaseStepViewModel
	{
		public RviAlarmStep RviAlarmStep { get; private set; }
		public ArgumentViewModel NameArgument { get; set; }

		public RviAlarmStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			RviAlarmStep = (RviAlarmStep)stepViewModel.Step;
			NameArgument = new ArgumentViewModel(RviAlarmStep.NameArgument, stepViewModel.Update, null);
			NameArgument.ExplicitValue.MinIntValue = 0;
		}

		public override void UpdateContent()
		{
			NameArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Значение: " + NameArgument.Description;
			}
		}
	}
}