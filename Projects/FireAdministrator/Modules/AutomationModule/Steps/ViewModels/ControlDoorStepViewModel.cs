using System.Collections.ObjectModel;
using StrazhAPI.Automation;
using StrazhAPI;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlDoorStepViewModel : BaseStepViewModel
	{
		ControlDoorArguments ControlDoorArguments { get; set; }
		public ArgumentViewModel DoorArgument { get; private set; }

		public ControlDoorStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlDoorArguments = stepViewModel.Step.ControlDoorArguments;
			Commands = ProcedureHelper.GetEnumObs<DoorCommandType>();
			DoorArgument = new ArgumentViewModel(ControlDoorArguments.DoorArgument, stepViewModel.Update, null);
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
			DoorArgument.Update(Procedure, ExplicitType.Object, objectType:ObjectType.Door);
		}

		public override string Description
		{
			get
			{
				return string.Format(StepCommonViewModel.ControlDoor,DoorArgument.Description,SelectedCommand.ToDescription());
			}
		}
	}
}
