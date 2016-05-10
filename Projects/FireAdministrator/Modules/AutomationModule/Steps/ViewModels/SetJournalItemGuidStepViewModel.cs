using FiresecAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SetJournalItemGuidStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		public SetJournalItemGuidArguments SetJournalItemGuidArguments { get; private set; }

		public SetJournalItemGuidStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SetJournalItemGuidArguments = stepViewModel.Step.SetJournalItemGuidArguments;
			ValueArgument = new ArgumentViewModel(SetJournalItemGuidArguments.ValueArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			ValueArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
                return string.Format(StepCommonViewModel.Value, ValueArgument.Description);
			}
		}
	}
}
