using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlDoorStepViewModel : BaseStepViewModel
	{
		ControlDoorArguments ControlDoorArguments { get; set; }
		public ArgumentViewModel DoorParameter { get; private set; }

		public ControlDoorStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlDoorArguments = stepViewModel.Step.ControlDoorArguments;
			Commands = ProcedureHelper.GetEnumObs<DoorCommandType>();
			DoorParameter = new ArgumentViewModel(ControlDoorArguments.DoorParameter, stepViewModel.Update);
			DoorParameter.ObjectType = ObjectType.ControlDoor;
			DoorParameter.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlDoorArguments.DoorCommandType;
		}

		public ObservableCollection<DoorCommandType> Commands { get; private set; }

		DoorCommandType _selectedCommand;
		public DoorCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlDoorArguments.DoorCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}
		
		public override void UpdateContent()
		{
			DoorParameter.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.ControlDoor, false));
		}

		public override string Description
		{
			get
			{
				return "Точка доступа: " + DoorParameter.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
