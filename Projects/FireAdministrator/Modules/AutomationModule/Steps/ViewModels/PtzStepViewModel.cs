using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class PtzStepViewModel : BaseStepViewModel
	{
		PtzStep PtzStep { get; set; }
		public ArgumentViewModel CameraArgument { get; private set; }
		public ArgumentViewModel PtzNumberArgument { get; private set; }

		public PtzStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			PtzStep = (PtzStep)stepViewModel.Step;
			CameraArgument = new ArgumentViewModel(PtzStep.CameraArgument, stepViewModel.Update, null);
			PtzNumberArgument = new ArgumentViewModel(PtzStep.PtzNumberArgument, stepViewModel.Update, null);
			PtzNumberArgument.ExplicitValue.MinIntValue = 1;
			PtzNumberArgument.ExplicitValue.MaxIntValue = 100;
		}

		public override void UpdateContent()
		{
			CameraArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.VideoDevice, isList: false);
			PtzNumberArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description
		{
			get
			{
				return "Камера: " + CameraArgument.Description + " Номер команды: " + PtzNumberArgument.Description;
			}
		}
	}
}