using StrazhAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class RviAlarmStopStepViewModel : BaseStepViewModel
	{
		public RviAlarmStopArguments RviAlarmStopArguments { get; private set; }
		public ArgumentViewModel NameArgument { get; set; }

        public RviAlarmStopStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			RviAlarmStopArguments = stepViewModel.Step.RviAlarmStopArguments;
			NameArgument = new ArgumentViewModel(RviAlarmStopArguments.NameArgument, stepViewModel.Update, null);
			NameArgument.ExplicitValue.MinIntValue = 0;
		}

		public override void UpdateContent()
		{
			NameArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
                return string.Format(StepCommonViewModel.Value, NameArgument.Description);
			}
		}
	}
}