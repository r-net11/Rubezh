using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlGKDoorStepViewModel : BaseStepViewModel
	{
		ControlGKDoorArguments ControlGKDoorArguments { get; set; }
		public ArgumentViewModel DoorArgument { get; private set; }

		public ControlGKDoorStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlGKDoorArguments = stepViewModel.Step.ControlGKDoorArguments;
			Commands = ProcedureHelper.GetEnumObs<DoorCommandType>();
			DoorArgument = new ArgumentViewModel(ControlGKDoorArguments.DoorArgument, stepViewModel.Update, null);
			SelectedCommand = ControlGKDoorArguments.DoorCommandType;
		}

		public ObservableCollection<DoorCommandType> Commands { get; private set; }

		DoorCommandType _selectedCommand;
		public DoorCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKDoorArguments.DoorCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}
		
		public override void UpdateContent()
		{
			DoorArgument.Update(Procedure, ExplicitType.Object, objectType:ObjectType.GKDoor, isList:false);
		}

		public override string Description
		{
			get
			{
				return "Точка доступа: " + DoorArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
