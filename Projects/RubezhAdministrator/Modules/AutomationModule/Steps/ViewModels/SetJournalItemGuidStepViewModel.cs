using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class SetJournalItemGuidStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		public SetJournalItemGuidStep SetJournalItemGuidStep { get; private set; }

		public SetJournalItemGuidStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			SetJournalItemGuidStep = (SetJournalItemGuidStep)stepViewModel.Step;
			ValueArgument = new ArgumentViewModel(SetJournalItemGuidStep.ValueArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			ValueArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Значение: " + ValueArgument.Description;
			}
		}
	}
}
