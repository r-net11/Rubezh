using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class StopRecordStepViewModel : BaseStepViewModel
	{
		StopRecordStep StopRecordStep { get; set; }
		public ArgumentViewModel EventUIDArgument { get; set; }

		public StopRecordStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			StopRecordStep = (StopRecordStep)stepViewModel.Step;
			EventUIDArgument = new ArgumentViewModel(StopRecordStep.EventUIDArgument, stepViewModel.Update, UpdateContent, false);
		}


		public override void UpdateContent()
		{
			EventUIDArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
				return " Идентификатор: " + EventUIDArgument.Description;
			}
		}
	}
}