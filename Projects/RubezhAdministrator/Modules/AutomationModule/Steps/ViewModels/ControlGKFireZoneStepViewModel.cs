using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ControlGKFireZoneStepViewModel : BaseStepViewModel
	{
		ControlGKFireZoneStep ControlGKFireZoneStep { get; set; }
		public ArgumentViewModel GKFireZoneArgument { get; private set; }

		public ControlGKFireZoneStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlGKFireZoneStep = (ControlGKFireZoneStep)stepViewModel.Step;
			Commands = AutomationHelper.GetEnumObs<ZoneCommandType>();
			GKFireZoneArgument = new ArgumentViewModel(ControlGKFireZoneStep.GKFireZoneArgument, stepViewModel.Update, null);
			SelectedCommand = ControlGKFireZoneStep.ZoneCommandType;
		}

		public ObservableCollection<ZoneCommandType> Commands { get; private set; }

		ZoneCommandType _selectedCommand;
		public ZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKFireZoneStep.ZoneCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			GKFireZoneArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.Zone, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Зона: " + GKFireZoneArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
