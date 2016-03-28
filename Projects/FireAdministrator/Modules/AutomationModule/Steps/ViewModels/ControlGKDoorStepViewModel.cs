using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ControlGKDoorStepViewModel : BaseStepViewModel
	{
		ControlGKDoorStep ControlGKDoorStep { get; set; }
		public ArgumentViewModel DoorArgument { get; private set; }

		public ControlGKDoorStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlGKDoorStep = (ControlGKDoorStep)stepViewModel.Step;
			Commands = AutomationHelper.GetEnumObs<GKDoorCommandType>();
			DoorArgument = new ArgumentViewModel(ControlGKDoorStep.DoorArgument, stepViewModel.Update, null);
			SelectedCommand = ControlGKDoorStep.DoorCommandType;
		}

		public ObservableCollection<GKDoorCommandType> Commands { get; private set; }

		GKDoorCommandType _selectedCommand;
		public GKDoorCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKDoorStep.DoorCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			DoorArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.GKDoor, isList: false);
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
