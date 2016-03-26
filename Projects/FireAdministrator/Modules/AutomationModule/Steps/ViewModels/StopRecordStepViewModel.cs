using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class StopRecordStepViewModel : BaseStepViewModel
	{
		StopRecordStep StopRecordStep { get; set; }
		public ArgumentViewModel CameraArgument { get; private set; }
		public ArgumentViewModel EventUIDArgument { get; set; }

		public StopRecordStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			StopRecordStep = (StopRecordStep)stepViewModel.Step;
			EventUIDArgument = new ArgumentViewModel(StopRecordStep.EventUIDArgument, stepViewModel.Update, UpdateContent);
			CameraArgument = new ArgumentViewModel(StopRecordStep.CameraArgument, stepViewModel.Update, null);
		}


		public override void UpdateContent()
		{
			CameraArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.VideoDevice, isList: false);
			EventUIDArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
				return "Камера: " + CameraArgument.Description + " Идентификатор: " + EventUIDArgument.Description;
			}
		}
	}
}