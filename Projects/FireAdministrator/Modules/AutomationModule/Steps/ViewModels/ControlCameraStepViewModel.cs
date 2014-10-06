using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlCameraStepViewModel: BaseStepViewModel
	{
		ControlCameraArguments ControlCameraArguments { get; set; }
		public ArgumentViewModel CameraParameter { get; private set; }

		public ControlCameraStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlCameraArguments = stepViewModel.Step.ControlCameraArguments;
			Commands = ProcedureHelper.GetEnumObs<CameraCommandType>();
			CameraParameter = new ArgumentViewModel(ControlCameraArguments.CameraParameter, stepViewModel.Update);
			CameraParameter.ObjectType = ObjectType.VideoDevice;
			CameraParameter.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlCameraArguments.CameraCommandType;
		}

		public ObservableCollection<CameraCommandType> Commands { get; private set; }
		public CameraCommandType SelectedCommand
		{
			get { return ControlCameraArguments.CameraCommandType; }
			set
			{
				ControlCameraArguments.CameraCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			CameraParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Object && x.ObjectType == ObjectType.VideoDevice && !x.IsList));
		}

		public override string Description
		{
			get 
			{ 
				return "Камера: " + CameraParameter.Description + " Команда: " + SelectedCommand.ToDescription(); 
			}
		}
	}
}
