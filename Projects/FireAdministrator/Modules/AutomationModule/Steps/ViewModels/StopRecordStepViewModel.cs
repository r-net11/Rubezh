using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class StopRecordStepViewModel : BaseStepViewModel
	{
		StopRecordArguments StopRecordArguments { get; set; }
		public ArgumentViewModel CameraArgument { get; private set; }
		public ArgumentViewModel EventUIDArgument { get; set; }

		public StopRecordStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			StopRecordArguments = stepViewModel.Step.StopRecordArguments;
			EventUIDArgument = new ArgumentViewModel(StopRecordArguments.EventUIDArgument, stepViewModel.Update, UpdateContent);
			CameraArgument = new ArgumentViewModel(StopRecordArguments.CameraArgument, stepViewModel.Update, null);
		}


		public override void UpdateContent()
		{
			CameraArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.VideoDevice);
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