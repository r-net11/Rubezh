using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlGKGuardZoneStepViewModel: BaseStepViewModel
	{
		ControlGKGuardZoneArguments ControlGKGuardZoneArguments { get; set; }
		public ArgumentViewModel GKGuardZoneArgument { get; private set; }

		public ControlGKGuardZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlGKGuardZoneArguments = stepViewModel.Step.ControlGKGuardZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<GuardZoneCommandType>();
			GKGuardZoneArgument = new ArgumentViewModel(ControlGKGuardZoneArguments.GKGuardZoneArgument, stepViewModel.Update, null);
			SelectedCommand = ControlGKGuardZoneArguments.GuardZoneCommandType;
		}

		public ObservableCollection<GuardZoneCommandType> Commands { get; private set; }
		GuardZoneCommandType _selectedCommand;
		public GuardZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKGuardZoneArguments.GuardZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}
	
		public override void UpdateContent()
		{
			GKGuardZoneArgument.Update(Procedure, ExplicitType.Object, objectType:ObjectType.GuardZone, isList:false);
		}

		public override string Description
		{
			get
			{
				return "Зона: " + GKGuardZoneArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
