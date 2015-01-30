using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlCameraStepViewModel : BaseStepViewModel
	{
		ControlCameraArguments ControlCameraArguments { get; set; }
		public ArgumentViewModel CameraArgument { get; private set; }
		public ArgumentViewModel EventUIDArgument { get; set; }
		public ArgumentViewModel TimeoutArgument { get; set; }

		public ControlCameraStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlCameraArguments = stepViewModel.Step.ControlCameraArguments;
			EventUIDArgument = new ArgumentViewModel(ControlCameraArguments.EventUIDArgument, stepViewModel.Update, UpdateContent);
			TimeoutArgument = new ArgumentViewModel(ControlCameraArguments.TimeoutArgument, stepViewModel.Update, UpdateContent);
			Commands = ProcedureHelper.GetEnumObs<CameraCommandType>();
			CameraArgument = new ArgumentViewModel(ControlCameraArguments.CameraArgument, stepViewModel.Update, null);
			SelectedCommand = ControlCameraArguments.CameraCommandType;
		}

		public ObservableCollection<CameraCommandType> Commands { get; private set; }
		public CameraCommandType SelectedCommand
		{
			get { return ControlCameraArguments.CameraCommandType; }
			set
			{
				ControlCameraArguments.CameraCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			CameraArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.VideoDevice, isList: false);
			EventUIDArgument.Update(Procedure, ExplicitType.String);
			TimeoutArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description
		{
			get
			{
				return "Камера: " + CameraArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}