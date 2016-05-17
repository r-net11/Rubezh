using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ControlGKGuardZoneStepViewModel : BaseStepViewModel
	{
		ControlGKGuardZoneStep ControlGKGuardZoneStep { get; set; }
		public ArgumentViewModel GKGuardZoneArgument { get; private set; }

		public ControlGKGuardZoneStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlGKGuardZoneStep = (ControlGKGuardZoneStep)stepViewModel.Step;
			Commands = AutomationHelper.GetEnumObs<GuardZoneCommandType>();
			GKGuardZoneArgument = new ArgumentViewModel(ControlGKGuardZoneStep.GKGuardZoneArgument, stepViewModel.Update, null);
			SelectedCommand = ControlGKGuardZoneStep.GuardZoneCommandType;
		}

		public ObservableCollection<GuardZoneCommandType> Commands { get; private set; }
		GuardZoneCommandType _selectedCommand;
		public GuardZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKGuardZoneStep.GuardZoneCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			GKGuardZoneArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.GuardZone, isList: false);
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
